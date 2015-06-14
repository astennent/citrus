using UnityEngine;
using System.Collections.Generic;

public class Connection {
   public ForeignKey foreignKey;
   public Node node;
   public Connection(ForeignKey foreignKey, Node node) {
      this.foreignKey = foreignKey;
      this.node = node;
   }
}

public class Node : MonoBehaviour {

   public GameObject halo;
   public Row row {get; private set;}

   private bool _isSelected;
   public bool isSelected {
      get { return _isSelected; }
      set {
         _isSelected = value;
         halo.SetActive(value);
         if (value) {
            halo.GetComponent<ParticleSystem>().startColor = color;
         }
      }
   }

   private Color _color;
   public Color color {
      get { return _color; }
      set {
         _color = value;
         GetComponent<Renderer>().material.color = value;
         if (isSelected) {
            halo.GetComponent<ParticleSystem>().startColor = value;
         }
      }
   }

   private Vector3 m_desiredPosition;

   private List<Connection> outgoingConnectionCache = new List<Connection>();
   private int outgoingCacheVersion = -1;

   private List<Connection> incomingConnectionCache = new List<Connection>();
   private Dictionary<Table, int> incomingCacheVersions = new Dictionary<Table, int>();

   private Renderer m_renderer;
   private EdgeRenderer m_edgeRenderer;

   private static float NODE_SPEED = 1f;

   public static Node Instantiate(Node prefab, Row _row)
   {
      Node node = (Node)GameObject.Instantiate(prefab, Vector3.zero, new Quaternion(0,0,0,0));
      node.row = _row;
      node.JumpToRandomPosition();
      NodeMover.AddNode(node);
      if (!node.isLinking()) {
         node.color = ColorManager.GenColor(ColorManager.Theme.BRIGHT);
      }
      node.m_renderer = node.GetComponent<Renderer>();
      node.m_edgeRenderer = node.GetComponent<EdgeRenderer>();
      return node;
   }

   public void JumpToRandomPosition()
   {
      transform.position = new Vector3(Random.value, Random.value, Random.value) * 2000 - Vector3.one*1000;
      m_desiredPosition = transform.position;
   }

   public bool isLinking()
   {
      return row.table.isLinking;
   }

   public void SetPosition(Vector3 position)
   {
      m_desiredPosition = position;
   }

   public Vector3 GetPosition()
   {
      return m_desiredPosition;
   }

   public void Update()
   {
      m_renderer.enabled = !isLinking();
      m_edgeRenderer.enabled = (outgoingConnectionCache.Count > 0);

      Vector3 currentPosition = transform.position;
      float distanceToTarget = Vector3.Distance(currentPosition, m_desiredPosition);
      if (distanceToTarget > .2) {
         transform.position = Vector3.Lerp(currentPosition, m_desiredPosition, NODE_SPEED * Time.deltaTime);
      }
      else if (distanceToTarget > 0) {
         transform.position = m_desiredPosition;
      }
   }

   public List<Connection> GetOutgoingConnections()
   {
      // Check if the cache is valid.
      if (outgoingCacheVersion == row.table.version && 
          outgoingConnectionCache.Count == row.table.foreignKeys.Count) {
         return outgoingConnectionCache;
      }
      
      // Cache is invalid, build a new one.
      outgoingCacheVersion = row.table.version;
      List<Connection> replacementCache = new List<Connection>();
      foreach (ForeignKey foreignKey in row.table.foreignKeys) {
         string sourceValue = row[foreignKey.sourceColumn];
         Row targetRow = foreignKey.targetTable.GetFirst(foreignKey.targetColumn, sourceValue, true);
         Node targetNode = NodeManager.GetNode(targetRow);
         if (targetNode) {
            Connection c = new Connection(foreignKey, targetNode);
            replacementCache.Add(c);
         }
      }

      outgoingConnectionCache = replacementCache;
      return outgoingConnectionCache;
   }

   public List<Connection> GetIncomingConnections()
   {
      // Check if the cache is valid.
      bool isCacheValid = true;
      foreach (Table table in ProjectManager.activeProject.tables) {
         if (!incomingCacheVersions.ContainsKey(table) || // This is a new key
               incomingCacheVersions[table] != table.version) { // Version out of date 
            isCacheValid = false;
            break;
         }
      }

      if (isCacheValid) {
         return incomingConnectionCache;
      }

      // Cache is invalid, build a new one.
      var replacementCache = new List<Connection>();
      bool isMissingNode = false;
      foreach (Table table in ProjectManager.activeProject.tables) {
         incomingCacheVersions[table] = table.version;

         foreach (ForeignKey foreignKey in table.foreignKeys) {

            if (foreignKey.targetTable != row.table) {
               continue;
            }

            string myValue = row[foreignKey.targetColumn];
            List<Row> matchedRows = 
                  foreignKey.sourceTable.GetAll(foreignKey.sourceColumn, myValue, true);
            foreach (Row matchedRow in matchedRows) {
               Node matchedNode = NodeManager.GetNode(matchedRow);
               if (matchedNode != null) {
                  Connection c = new Connection(foreignKey, matchedNode);
                  replacementCache.Add(c);
               } else {
                  isMissingNode = true;
               }
            }
         }
      }

      incomingConnectionCache = replacementCache;
      
      if (isMissingNode) {
         incomingCacheVersions.Clear(); // marks the cache as invalid.
      }

      return incomingConnectionCache;
   }

   /**
    * Returns a list of all nodes directly connected to this node. Directly connected can be a link
    * from a normal table to another normal table, or from a linking table connecting two normal
    * tables.
    */
   public List<Connection> GetConnectedNodes() 
   {
      var connections = new List<Connection>();
      var outgoingConnections = GetOutgoingConnections();
      foreach (Connection outgoingConnection in outgoingConnections) {
         if (!outgoingConnection.foreignKey.targetTable.isLinking) {
            connections.Add(outgoingConnection);
         }
      }

      var incomingConnections = GetIncomingConnections();
      foreach (Connection incomingConnection in incomingConnections) {
         if (incomingConnection.foreignKey.targetTable.isLinking) {
            Node linkingNode = incomingConnection.node;
            List<Connection> linkedTargets = linkingNode.GetOutgoingConnections();
            foreach (Connection linkedConnection in linkedTargets) {
               if (linkedConnection.node != this) {
                  connections.Add(linkedConnection);
               }
            }
         }
         else {
            connections.Add(incomingConnection);
         }
      }

      return connections;
   }

}
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

   public Row row {get; private set;}

   private LineRenderer lineRenderer;
   private Vector3 m_desiredPosition;

   private static float NODE_SPEED = 1f;

   public static Node Instantiate(Node prefab, Row _row)
   {
      Node node = (Node)GameObject.Instantiate(prefab, Vector3.zero, new Quaternion(0,0,0,0));
      node.row = _row;
      node.JumpToRandomPosition();
      node.lineRenderer = node.GetComponent<LineRenderer>();
      NodeMover.AddNode(node);
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
      Renderer renderer = GetComponent<Renderer>();
      renderer.enabled = !isLinking();
      transform.position = Vector3.Lerp(transform.position, m_desiredPosition, NODE_SPEED * Time.deltaTime);
   }


   public List<Connection> GetConnections()
   {
      var connections = new List<Connection>();
      foreach (ForeignKey foreignKey in row.table.foreignKeys) {
         string sourceValue = row[foreignKey.sourceColumn];
         Row targetRow = foreignKey.targetTable.Get(foreignKey.targetColumn, sourceValue);
         Node targetNode = NodeManager.GetNode(targetRow);
         if (targetNode != null) {
            connections.Add(new Connection(foreignKey, targetNode));
         }
      }
      return connections;
   }

   public void LateUpdate() 
   {
      List<Connection> connections = GetConnections();

      if (isLinking()) {
         lineRenderer.enabled = (connections.Count >= 2);

         // TODO: Make wheel spokes if this is larger than 2.
         for (int positionIndex = 0 ; positionIndex < connections.Count ; positionIndex++) {
            Node targetNode = connections[positionIndex].node;
            lineRenderer.SetPosition(positionIndex, targetNode.transform.position);
         }
      }
      else {
         //TODO.
      }

   }

}
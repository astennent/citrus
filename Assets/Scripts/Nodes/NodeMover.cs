using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class NodeMover : MonoBehaviour {

   private static Queue<Node> m_nodesToAdd = new Queue<Node>();
   private static HashSet<Node> m_workingNodes = new HashSet<Node>();

   private Thread m_movingThread;

   private static bool m_isAlive = true;

   void Start()
   {
      m_movingThread = new System.Threading.Thread(MoveNodeLoop);
      m_movingThread.Start();
   }

   void OnDestroy()
   {
      m_isAlive = false; 
   }

   public static void AddNode(Node node)
   {
      lock (m_nodesToAdd) {
         m_nodesToAdd.Enqueue(node);
      }
   }
   
   private static void MoveNodeLoop() 
   {
      while (m_isAlive) {
         lock(m_workingNodes) {
            ProcessNodesForAdding();
            if (m_workingNodes.Count == 0) {
               Thread.Sleep(1);
            }
            MoveNodes();
         }
      }
   }

   private static void ProcessNodesForAdding()
   {
      lock(m_nodesToAdd) {
         while (m_nodesToAdd.Count > 0) {
            m_workingNodes.Add(m_nodesToAdd.Dequeue());
         }
      }
   }

   private static void MoveNodes()
   {
      var futurePositions = new Dictionary<Node, Vector3>();

      foreach (Node node in m_workingNodes) {
         MoveNode(node, ref futurePositions);
      }

      foreach (KeyValuePair<Node, Vector3> kvp in futurePositions) {
         kvp.Key.SetPosition(kvp.Value);
      }
   }

   private static void MoveNode(Node node, ref Dictionary<Node,Vector3> futurePositions)
   {
      List<Connection> connections = node.GetConnections();

      // Every node moves to attempt to make itself equidistant from every other node.
      for (int i = 0 ; i < connections.Count ; i++) {
         if (node.isLinking()) {
            for (int j = 0 ; j < connections.Count ; j++) {
               if (i != j) {
                  MoveRelative(connections[i].node, connections[j], ref futurePositions);
               }
            }
         }
         else { // not linking.
            MoveRelative(node, connections[i], ref futurePositions);
         }
      }
   }

   private static void MoveRelative(Node node1, Connection connection, ref Dictionary<Node, Vector3> futurePositions) {
      float desiredDistance = 100f; //TODO: Use connection to respect foreign key weights.

      Node node2 = connection.node;

      Vector3 position1 = (futurePositions.ContainsKey(node1))
         ? futurePositions[node1]
         : node1.GetPosition();

      Vector3 position2 = (futurePositions.ContainsKey(node2))
         ? futurePositions[node2]
         : node2.GetPosition();

      Vector3 center = (position1 + position2)/2f;
      Vector3 desiredPosition1 = center + (center-position1).normalized * desiredDistance/2f;
      Vector3 desiredPosition2 = center + (center-position2).normalized * desiredDistance/2f;

      futurePositions[node1] = desiredPosition1;
      futurePositions[node2] = desiredPosition2;
   }

}
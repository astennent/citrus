using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class NodeMover : MonoBehaviour {

   private static Queue<Node> m_nodesToAdd = new Queue<Node>();
   private static HashSet<Node> m_workingNodes = new HashSet<Node>();

   private Thread m_movingThread;

   private static bool m_isAlive = true;
   private static int MAX_DEPTH = 1;
   private static float DEFAULT_DESIRED_DISTANCE = 100f;

   private static Dictionary<Node, Vector3> futurePositions = new Dictionary<Node, Vector3>();

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

   public static void SetPosition(Node node, Vector3 desiredPosition) {
      lock(futurePositions) {
         futurePositions[node] = desiredPosition;
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
      foreach (Node node in m_workingNodes) {
         MoveNode(node);
      }

      lock (futurePositions) {
         foreach (KeyValuePair<Node, Vector3> kvp in futurePositions) {
            kvp.Key.SetPosition(kvp.Value);
         }
         futurePositions = new Dictionary<Node, Vector3>();
      }
   }

   private static void MoveNode(Node node)
   {
      if (node.isLinking()) {
         return;
      }

      List<Connection> connections = node.GetConnectedNodes();

      // Every node moves to attempt to make itself equidistant from every other node.
      for (int i = 0 ; i < connections.Count ; i++) {
         MoveRelative(node, connections[i], 0);
      }
   }

   private static void MoveRelative(Node node1, Connection connection, int depth) {
      Node node2 = connection.node;
      if (node1 == node2) {
         return;
      }

      if (depth < MAX_DEPTH) {
         List<Connection> recursedConnections = node2.GetConnectedNodes();
         foreach (Connection recursedConnection in recursedConnections) {
            if (!recursedConnection.node.isLinking()) {
               MoveRelative(node1, recursedConnection, depth+1);
            }
         }
      }

      lock (futurePositions) {
         Vector3 position1 = (futurePositions.ContainsKey(node1))
            ? futurePositions[node1]
            : node1.GetPosition();

         Vector3 position2 = (futurePositions.ContainsKey(node2)) 
            ? futurePositions[node2]
            : node2.GetPosition();

         Vector3 center = (position1 + position2)/2f;


         float desiredDistance = DEFAULT_DESIRED_DISTANCE; //TODO: Use connection to respect foreign key weights.
         float currentDistance = Vector3.Distance(position1, position2);
         Vector3 desiredPosition1 = position1;
         Vector3 desiredPosition2 = position2;
         if (depth == 0) {
            desiredPosition1 = center + (position1-center).normalized * desiredDistance/2f;
            desiredPosition2 = center + (position2-center).normalized * desiredDistance/2f;
         }
         else if (currentDistance < desiredDistance/2f) {
            desiredPosition1 = position1 + (position1 - position2) / currentDistance;
            futurePositions[node1] = desiredPosition1;
            desiredPosition2 = position2 + (position2 - position1) / currentDistance;
            futurePositions[node2] = desiredPosition2;
         }
         futurePositions[node1] = desiredPosition1;
         futurePositions[node2] = desiredPosition2;
      }
   }

}
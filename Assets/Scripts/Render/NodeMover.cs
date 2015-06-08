using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class NodeMover : MonoBehaviour {

   private static Queue<Node> m_nodesToAdd = new Queue<Node>();
   private static HashSet<Node> m_workingNodes = new HashSet<Node>();

   private Thread m_movingThread;

   private static bool m_isAlive = true;

   void Start() {
      m_movingThread = new System.Threading.Thread(MoveNodes);
      m_movingThread.Start();
   }

   void OnDestroy() {
      m_isAlive = false; 
   }

   public static void AddNode(Node node) {
      lock (m_nodesToAdd) {
         m_nodesToAdd.Enqueue(node);
      }
   }
   
   public static void MoveNodes() {
      while (m_isAlive) {
         lock(m_workingNodes) {


            lock(m_nodesToAdd) {
               while (m_nodesToAdd.Count > 0) {
                  m_workingNodes.Add(m_nodesToAdd.Dequeue());
               }
            }

            if (m_workingNodes.Count == 0) {
               Thread.Sleep(1);
            }

            foreach (Node node in m_workingNodes) {
                List<Connection> connections = node.GetConnections();
      
               //TODO: Respect foreign key weights.
               float desiredDistance = 100f;

               if (node.isLinking()) {
                  // Every node moves to attempt to make itself equidistant from every other node.
                  for (int i = 0 ; i < connections.Count ; i++) {
                     Node node1 = connections[i].node; 
                     for (int j = 0 ; j < connections.Count ; j++) {
                        if (i != j) {
                           Vector3 position1 = node1.GetPosition();
                           Node node2 = connections[j].node;
                           Vector3 position2 = node2.GetPosition();

                           Vector3 center = (position1 + position2)/2f;
                           Vector3 desiredPosition1 = center + (center-position1).normalized * desiredDistance/2f;
                           node1.SetPosition(desiredPosition1);
                           Vector3 desiredPosition2 = center + (center-position2).normalized * desiredDistance/2f;
                           node2.SetPosition(desiredPosition2);
                        }
                     }
                  }
               }
               else { // not linking.
                  // TODO: Implement movement for non-linking tables.
               }
            }
         }
      }
   }

}
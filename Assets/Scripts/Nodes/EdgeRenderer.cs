using UnityEngine;
using System.Collections.Generic;

class EdgeRenderer : MonoBehaviour {

   public Node parentNode = null;
   private Queue<LineRenderer> m_lines = new Queue<LineRenderer>();

   void LateUpdate() {
      List<Connection> connections = parentNode.GetOutgoingConnections();
      UpdateLines(connections.Count);
      
      bool isLinking = parentNode.isLinking();
      if (m_lines.Count < 1 || (isLinking && m_lines.Count < 2)) {
         return;
      }

      Vector3 positionSum = Vector3.zero;
      foreach (Connection connection in  connections) {
         positionSum += connection.node.transform.position;
      }
      Vector3 center = positionSum / connections.Count;

      int connectionIndex = 0;
      foreach (LineRenderer line in m_lines) {
         Color startColor = connections[connectionIndex].node.color;
         Color endColor = (isLinking) ? Color.white : startColor;
         line.SetColors(startColor, endColor);

         Vector3 connectedNodePosition = connections[connectionIndex].node.transform.position;
         Vector3 startPosition = (isLinking) ? connectedNodePosition : parentNode.transform.position;
         Vector3 endPosition = (isLinking) ? center : connectedNodePosition;
         line.SetPosition(0, startPosition);
         line.SetPosition(1, endPosition);
         connectionIndex++;
      }
   }

   void UpdateLines(int count) {

      while (m_lines.Count < count) {
         LineRenderer line = (LineRenderer)LineRenderer.Instantiate(NodeManager.GetLinePrefab(), 
               Vector3.zero, new Quaternion(0,0,0,0));
         m_lines.Enqueue(line);
      }

      while (m_lines.Count > count) {
         LineRenderer toDestroy = m_lines.Dequeue();
         Destroy(toDestroy.gameObject);
      }

   }


}
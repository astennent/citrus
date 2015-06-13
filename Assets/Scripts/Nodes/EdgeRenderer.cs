using UnityEngine;
using System.Collections.Generic;

class EdgeRenderer : MonoBehaviour {

   public Node parentNode = null;
   private List<LineRenderer> m_lines = new List<LineRenderer>();

   void LateUpdate() {
      List<Connection> connections = parentNode.GetOutgoingConnections();

      UpdateLines(connections.Count);
      
      bool isLinking = parentNode.isLinking();
      if (m_lines.Count < 1 || (isLinking && m_lines.Count < 2)) {
         return;
      }

      Vector3 center = Vector3.zero;

      if (isLinking) {
         Vector3 positionSum = Vector3.zero;
         foreach (Connection connection in  connections) {
            positionSum += connection.node.transform.position;
            center = positionSum / connections.Count;
         }
      }

      for (var i = 0 ; i < m_lines.Count ; i++) {
         Connection connection = connections[i];
         LineRenderer line = m_lines[i];

         Color startColor = connection.node.color;
         Color endColor = (isLinking) ? Color.white : startColor;
         line.SetColors(startColor, endColor);

         Vector3 connectedNodePosition = connection.node.transform.position;
         Vector3 startPosition = (isLinking) ? connectedNodePosition : parentNode.transform.position;
         Vector3 endPosition = (isLinking) ? center : connectedNodePosition;
         line.SetPosition(0, startPosition);
         line.SetPosition(1, endPosition);
      }
   }

   void UpdateLines(int count) {

      while (m_lines.Count < count) {
         LineRenderer line = (LineRenderer)LineRenderer.Instantiate(NodeManager.GetLinePrefab(), 
               Vector3.zero, new Quaternion(0,0,0,0));
         m_lines.Add(line);
      }

      while (m_lines.Count > count) {
         LineRenderer toDestroy = m_lines[m_lines.Count-1];
         m_lines.RemoveAt(m_lines.Count-1);
         Destroy(toDestroy.gameObject);
      }

   }


}
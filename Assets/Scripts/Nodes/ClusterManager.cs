using System.Collections.Generic;

class ClusterManager {

   public static HashSet<Node> GetConnectedNodes(Node startNode) {
      var matches = new HashSet<Node>();
      var toCheck = new Queue<Node>();
      toCheck.Enqueue(startNode);

      while (toCheck.Count > 0) {
         Node node = toCheck.Dequeue();

         List<Connection> connections = node.GetConnectedNodes();
         foreach (Connection c in connections){
            Node connectedNode = c.node;
            if (!matches.Contains(connectedNode)) {
               toCheck.Enqueue(connectedNode);
            }
         }

         matches.Add(node);
      }

      return matches;
   }
}
using UnityEngine;
using System.Collections.Generic;

public class Node : MonoBehaviour {

   public Row row {get; private set;}

   private LineRenderer lineRenderer;

   public static Node Instantiate(Node prefab, Row _row)
   {
      Node node = (Node)GameObject.Instantiate(prefab, Vector3.zero, new Quaternion(0,0,0,0));
      node.row = _row;
      node.JumpToRandomPosition();
      node.lineRenderer = node.GetComponent<LineRenderer>();
      return node;
   }

   public void JumpToRandomPosition()
   {
      transform.position = new Vector3(Random.value, Random.value, Random.value) * 2000 - Vector3.one*1000;
   }

   public void Update()
   {
      bool shouldRender = !(row.table.isLinking);
      Renderer renderer = GetComponent<Renderer>();
      if (renderer.enabled && !shouldRender) {
         renderer.enabled = false;
      } 
      else if (!renderer.enabled && shouldRender) {
         renderer.enabled = true;
      }
   }

   public void LateUpdate() 
   {
      var table = row.table;
      var connectedNodes = new List<Node>();
      foreach (ForeignKey foreignKey in table.foreignKeys) {
         string sourceValue = row[foreignKey.sourceColumn];
         Row targetRow = foreignKey.targetTable.Get(foreignKey.targetColumn, sourceValue);
         Node targetNode = NodeManager.GetNode(targetRow);
         if (targetNode != null) {
            connectedNodes.Add(targetNode);
         }
      }

      if (table.isLinking) {
         lineRenderer.enabled = (connectedNodes.Count >= 2);

         // TODO: Make wheel spokes if this is larger than 2.
         for (int positionIndex = 0 ; positionIndex < connectedNodes.Count ; positionIndex++) {
            Node targetNode = connectedNodes[positionIndex];
            lineRenderer.SetPosition(positionIndex, targetNode.transform.position);
         }
      }
      else {
         //TODO.
      }

   }

}
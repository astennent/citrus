using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class NodeGenerator : MonoBehaviour {

   public Node nodePrefab;
   private static NodeGenerator s_instance;
   private static Queue<Row> rowQueue = new Queue<Row>();
   private static int nodesPerFrame = 100; // The number of nodes to process per frame.

   void Start() {
      s_instance = this;
      StartCoroutine("ConsumeNodes");
   }

   public static Node GetNodePrefab() {
      return s_instance.nodePrefab;
   }
   
   public static void Generate(Row row) {
      lock(rowQueue) {
         rowQueue.Enqueue(row);
      }
   }

   IEnumerator ConsumeNodes() {
      while(Application.isPlaying) {

         lock(rowQueue) {
            int count = 0;
            while (rowQueue.Count > 0 && count < nodesPerFrame) {
               Row row = rowQueue.Dequeue();
               Node.Instantiate(s_instance.nodePrefab, row);
               count++;
            }
         }

         yield return 0;
      }        
   }


}
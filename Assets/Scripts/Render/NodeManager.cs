using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class NodeManager : MonoBehaviour {

   public Node nodePrefab = null;
   private static NodeManager s_instance;
   private static Queue<Row> toLoad = new Queue<Row>();
   private static Queue<Row> toUnload = new Queue<Row>();
   private static int nodesPerFrame = 100; // The number of nodes to process per frame.

   private static object loaderLock = new object();

   void Start() {
      s_instance = this;
      StartCoroutine("LoadNodes");
      StartCoroutine("UnloadNodes");
   }

   public static Node GetNodePrefab() {
      return s_instance.nodePrefab;
   }
   
   public static void Load(Row row) {
      lock(loaderLock) {
         toLoad.Enqueue(row);
      }
   }

   public static void Unload(Row[] rows) {
      lock(loaderLock) {
         toLoad.Clear();
         foreach (Row row in rows) {
            toUnload.Enqueue(row);
         }
      }
   }

   IEnumerator LoadNodes() {
      while(Application.isPlaying) {

         lock(toLoad) {
            int count = 0;
            while (toLoad.Count > 0 && count < nodesPerFrame) {
               Row row = toLoad.Dequeue();
               Node.Instantiate(s_instance.nodePrefab, row);
               count++;
            }
         }

         yield return 0;
      }        
   }

   IEnumerator UnloadNodes() {
      while(Application.isPlaying) {

         lock(toUnload) {
            while (toLoad.Count > 0) {
               Row row = toLoad.Dequeue();
               row.DestroyNode();
            }
         }

         yield return 0;
      }  
   }


}
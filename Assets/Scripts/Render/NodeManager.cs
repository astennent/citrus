using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class NodeManager : MonoBehaviour {

   public Node nodePrefab = null;
   private static NodeManager s_instance;
   private static Queue<Row> toLoad = new Queue<Row>();
   private static Queue<Row> toUnload = new Queue<Row>();

   private static object loaderLock = new object();

   private static Dictionary<Table, Dictionary<Row, Node> > nodeMap =
         new Dictionary<Table, Dictionary<Row, Node> >();

   void Start()
   {
      s_instance = this;
      StartCoroutine("LoadNodes");
      StartCoroutine("UnloadNodes");
   }

   public static Node GetNodePrefab()
   {
      return s_instance.nodePrefab;
   }
   
   public static void Load(Row row)
   {
      lock(loaderLock) {
         toLoad.Enqueue(row);
      }
   }

   public static void Unload(Row[] rows)
   {
      lock(loaderLock) {
         toLoad.Clear();
         foreach (Row row in rows) {
            toUnload.Enqueue(row);
         }
      }
   }

   IEnumerator LoadNodes()
   {
      while(Application.isPlaying) {
         lock(loaderLock) {
            while (toLoad.Count > 0) {
               Row row = toLoad.Dequeue();
               Node node = Node.Instantiate(s_instance.nodePrefab, row);
               AddNodeToMap(row, node);
            }
         }
         yield return 0;
      }        
   }

   IEnumerator UnloadNodes()
   {
      while(Application.isPlaying) {
         lock(loaderLock) {
            while (toUnload.Count > 0) {
               Row row = toUnload.Dequeue();
               RemoveFromMap(row);
            }
         }
         yield return 0;
      }  
   }

   public static Node GetNode(Row row) {
      if (row == null || !nodeMap.ContainsKey(row.table) || !nodeMap[row.table].ContainsKey(row)) {
         return null;
      }
      return nodeMap[row.table][row];
   }

   private static void AddNodeToMap(Row row, Node node)
   {
      Table table = row.table;
      if (!nodeMap.ContainsKey(table)) {
         nodeMap[table] = new Dictionary<Row, Node>();
      }
      nodeMap[table][row] = node;
   }

   private static void RemoveFromMap(Row row)
   {
      Table table = row.table;
      if (!nodeMap.ContainsKey(table)) {
         return;
      }
      if (!nodeMap[table].ContainsKey(row)) {
         return;
      }
      Node node = nodeMap[table][row];
      GameObject.Destroy(node.gameObject);
      nodeMap[table].Remove(row);
   }



}
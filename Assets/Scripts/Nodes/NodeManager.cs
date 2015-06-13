using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

class NodeManager : MonoBehaviour {

   public Node nodePrefab = null;
   public LineRenderer linePrefab = null; // TODO Should this be owned by NodeManager?
   private static NodeManager s_instance;
   private static Queue<Row> toLoad = new Queue<Row>();
   private static Queue<Row> toUnload = new Queue<Row>();

   private static object loaderLock = new object();

   public static Dictionary<Table, Dictionary<Row, Node> > nodeMap {get; private set;}

   private Thread m_movementThread;

   void Start()
   {
      s_instance = this;
      StartCoroutine("LoadNodes");
      StartCoroutine("UnloadNodes");
      nodeMap = new Dictionary<Table, Dictionary<Row, Node> >();
   }

   public static Node GetNodePrefab()
   {
      return s_instance.nodePrefab;
   }

   public static LineRenderer GetLinePrefab() {
      return s_instance.linePrefab;
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
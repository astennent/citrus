using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class GraphObjectManager : MonoBehaviour {

   public GraphObject graphObjectPrefab = null;
   private static GraphObjectManager s_instance;
   private static Queue<Row> toLoad = new Queue<Row>();
   private static Queue<Row> toUnload = new Queue<Row>();
   private static int graphObjectsPerFrame = 100; // The number of graphobjects to process per frame.

   private static object loaderLock = new object();

   private static Dictionary<Table, Dictionary<Row, GraphObject> > graphObjectMap =
         new Dictionary<Table, Dictionary<Row, GraphObject> >();

   void Start()
   {
      s_instance = this;
      StartCoroutine("LoadGraphObjects");
      StartCoroutine("UnloadGraphObjects");
   }

   public static GraphObject GetGraphObjectPrefab()
   {
      return s_instance.graphObjectPrefab;
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

   IEnumerator LoadGraphObjects()
   {
      while(Application.isPlaying) {
         lock(toLoad) {
            int count = 0;
            while (toLoad.Count > 0 && count < graphObjectsPerFrame) {
               Row row = toLoad.Dequeue();
               GraphObject graphObject = GraphObject.Instantiate(s_instance.graphObjectPrefab, row);
               AddGraphObjectToMap(row, graphObject);
               count++;
            }
         }
         yield return 0;
      }        
   }

   IEnumerator UnloadGraphObjects()
   {
      while(Application.isPlaying) {
         lock(toUnload) {
            while (toLoad.Count > 0) {
               Row row = toLoad.Dequeue();
               RemoveFromMap(row);
            }
         }
         yield return 0;
      }  
   }

   private static void AddGraphObjectToMap(Row row, GraphObject graphObject)
   {
      Table table = row.table;
      if (!graphObjectMap.ContainsKey(table)) {
         graphObjectMap[table] = new Dictionary<Row, GraphObject>();
      }
      graphObjectMap[table][row] = graphObject;
   }

   private static void RemoveFromMap(Row row)
   {
      Table table = row.table;
      if (!graphObjectMap.ContainsKey(table)) {
         return;
      }
      if (!graphObjectMap[table].ContainsKey(row)) {
         return;
      }
      GraphObject graphObject = graphObjectMap[table][row];
      GameObject.Destroy(graphObject.gameObject);
      graphObjectMap[table].Remove(row);
   }



}
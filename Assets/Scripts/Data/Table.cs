using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.Serialization;

[DataContract]
class Query {
   private Dictionary<string, List<Row> > m_map = new Dictionary<string, List<Row>>();

   public List<Row> GetAll(string targetValue) {
      if (m_map.ContainsKey(targetValue)) {
         return m_map[targetValue];
      }
      return new List<Row>();
   }

   public Row GetFirst(string targetValue) {
      List<Row> fullList = GetAll(targetValue);
      if (fullList.Count > 0) {
         return fullList[0];
      }
      return null;
   }

   public void Add(string cellValue, Row row) {
      if (!m_map.ContainsKey(cellValue)) {
         m_map[cellValue] = new List<Row>();
      }
      m_map[cellValue].Add(row);
   }
}

[DataContract]
public class Table {
   private static int LINES_PER_FRAME = 1;

   /**
    * Version is used so that other classes can validate their data without Table being responsible
    * for keeping them all in sync. Anything that uses this table that caches data should check the
    * current version before using that cache. Any change to the rows of this table will iterate the
    * current version, but external classes should not rely on newer versions being strictly greater
    * than older ones.
    */
   public int version {get; private set;}

   /**
    * Id is a unique value per table used to identify tables after deserialization without using
    * circular references.
    */
   public int id {get; private set;}

   private Row[] m_rows;
   private int parsedLineIndex = -1; // linesQueue has been parsed up to and including this index.
   
   private int consumedRowIndex = -1;
   private Thread m_loadingThread;

   //[DataMember] // Should this be optional?
   private string[] linesQueue;
   private Thread m_parsingThread;
   private object threadStartAndStopper = new object(); //Used to lock around starting or ending threads.

   private Dictionary<int, Query> m_cachedQueries = new Dictionary<int, Query>();

   [DataMember]
   public Attribute[] attributes {get; private set; }

   public enum TableState {
      PARSING, // This will be set if the table is constructed using a string[]
      UNLOADED, // Indicates that all lines are parsed, but nodes are either unloaded or loading
      LOADED, // Indicates nodes are all loaded.
   }
   public TableState state {get; private set; }

   [DataMember]
   public List<ForeignKey> foreignKeys {get; private set;}

   public void AddForeignKey(int sourceIndex, Table table, int columnIndex) {
      foreignKeys.Add(new ForeignKey(id, sourceIndex, table.id, columnIndex));
      IterateVersion();
      // TODO: Menu updates
   }
   public void RemoveForeignKey(ForeignKey key) {
      foreignKeys.Remove(key);
      IterateVersion();
      //TODO: Menu updates
   }

   private bool m_usesHeaders = false;
   public bool usesHeaders { 
      get {
         return m_usesHeaders;
      } private set {
         //TODO: Check if Loaded....this would remove or add a node.
         m_usesHeaders = value;
         if (state == TableState.PARSING) {
            return;
         }

         for (int i = 0 ; i < attributes.Length ; i++) {
            Attribute attribute = attributes[i];
            attribute.name = (m_usesHeaders) ? m_rows[0][i] : ("Col" + i);
         }
         IterateVersion();
      }
   }

   /**
    * isLinking is checked by Nodes at every frame. If it is false, they will render as a node
    * and update their position. If it is true, they will render as an edge and update that edge to
    * track affiliated nodes.
    */
   public bool isLinking = false;

   public Table(string[] lines) {
      id = TableMap.Register(this);
      m_rows = new Row[lines.Length];
      foreignKeys = new List<ForeignKey>();
      linesQueue = lines;
      parsedLineIndex = -1;
      state = TableState.PARSING;
      m_parsingThread = new System.Threading.Thread(ConsumeLines);
      m_loadingThread = new System.Threading.Thread(GenerateNodes);
      m_parsingThread.Start();
   }

   public List<Row> GetAll(int columnIndex, string targetValue, bool addIndex) {
      if (state == TableState.PARSING) {
         return new List<Row>();
      }

      if (addIndex) {
         AddIndex(columnIndex);
      }

      if (m_cachedQueries.ContainsKey(columnIndex)) {
         return m_cachedQueries[columnIndex].GetAll(targetValue);
      }

      List<Row> relevantRows = new List<Row>();
      foreach (Row row in m_rows) {
         if (row != null && row[columnIndex] == targetValue) {
            relevantRows.Add(row);
         }
      }
      return relevantRows;
   }

   public Row GetFirst(int columnIndex, string targetValue, bool addIndex) {
      List<Row> fullList = GetAll(columnIndex, targetValue, addIndex);
      if (fullList.Count > 0) {
         return fullList[0];
      }
      return null;
   }

   // This is called by Get because indexing is a somewhat expensive (though one-time) cost that we
   // want to avoid for the session if it's not used.
   private void AddIndex(int columnIndex) {
      if (m_cachedQueries.ContainsKey(columnIndex)) {
         return;
      }

      var query = new Query();
      foreach (Row row in m_rows) {
         if (row == null) {
            break; // You haven't parsed beyond this.
         }
         string cellValue = row[columnIndex];
         query.Add(cellValue, row);
      }

      m_cachedQueries[columnIndex] = query;
   }

   void ConsumeLines() {
      while(true) {
         lock(linesQueue) {
            int count = 0;
            while (parsedLineIndex < linesQueue.Length - 1 && count < LINES_PER_FRAME) {
               count++;
               parsedLineIndex++;
               string[] splitLine = CSVParser.SplitCsvLine(linesQueue[parsedLineIndex]);
               m_rows[parsedLineIndex] = new Row(this, splitLine);
               version++;
            }

            if (parsedLineIndex == linesQueue.Length - 1) {
               parsedLineIndex++;
               OnFinishParse();
               return; // Ends the coroutine.
            }
         }
      }        
   }

   // Called after m_rows has been set or built by the constructor or coroutine.
   private void OnFinishParse() {
      state = TableState.UNLOADED;
      int numColumns = m_rows[0].Length();
      attributes = new Attribute[numColumns];
      for (int i = 0 ; i < numColumns ; i++) {
         string attributeName = (m_usesHeaders) ? m_rows[0][i] : "Col"+i;
         attributes[i] = new Attribute(attributeName);
      }
      IterateVersion();
   }

   public void Load() {
      lock (threadStartAndStopper) {
         JoinThread(m_loadingThread);
         m_loadingThread.Start();
      }
   }

   public void Unload() {
      // state will be parsing if we were still parsing, and we want to leave it that way.
      if (state != TableState.PARSING) {
         state = TableState.UNLOADED;
      }
      // Ensure that you are not Loading.
      lock (threadStartAndStopper) {
         JoinThread(m_loadingThread);
         lock(linesQueue) {
            NodeManager.Unload(m_rows);
            consumedRowIndex = -1;
         }
      }
   }

   private void GenerateNodes() {
      while(true) {
         lock(linesQueue) {
            while (consumedRowIndex < parsedLineIndex) {
               consumedRowIndex++;
               Row row = m_rows[consumedRowIndex];
               NodeManager.Load(row);
            }
            if (consumedRowIndex == m_rows.Length-1) {
               OnFinishLoad();
               return; // Ends the thread.
            }
         }
      }  
   }

   private void JoinThread(Thread thread) {
      try {
         thread.Join();
      } catch (ThreadStateException) {
         // This can fail if the thread hasn't been started yet, but that just means it's finished.
      }
   }

   private void OnFinishLoad() {
      state = TableState.LOADED;
   }

   private void IterateVersion() {
      m_cachedQueries = new Dictionary<int, Query>();
      version++;
   }

}

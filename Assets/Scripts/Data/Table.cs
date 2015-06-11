using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

class Query {
   private Dictionary<string, List<Row> > m_map = new Dictionary<string, List<Row> >();

   public List<Row> GetAll(string targetValue) {
      if (m_map.ContainsKey(targetValue)) {
         return m_map[targetValue];
      }
      return null;
   }

   public Row GetFirst(string targetValue) {
      List<Row> fullList = GetAll(targetValue);
      if (fullList != null) {
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

public class Table {
   private int consumedRowIndex = -1;
   private Row[] m_rows;
   private Thread m_loadingThread;

   private int parsedLineIndex = -1; // linesQueue has been parsed up to and including this index.
   private string[] linesQueue;
   private int linesPerFrame = 1;
   private Thread m_parsingThread;

   private object threadStartAndStopper = new object(); //Used to lock around starting or ending threads.

   private Dictionary<int, Query> m_cachedQueries = new Dictionary<int, Query>();

   public Attribute[] attributes {get; private set; }

   public enum TableState {
      PARSING, // This will be set if the table is constructed using a string[]
      UNLOADED, // Indicates that all lines are parsed, but nodes are either unloaded or loading
      LOADED, // Indicates nodes are all loaded.
   }
   public TableState state {get; private set; }

   public List<ForeignKey> foreignKeys {get; private set;}
   public void AddForeignKey(int sourceIndex, Table table, int columnIndex) {
      foreignKeys.Add(new ForeignKey(this, sourceIndex, table, columnIndex));
      // TODO: Menu updates
   }
   public void RemoveForeignKey(ForeignKey key) {
      foreignKeys.Remove(key);
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
      }
   }

   /**
    * isLinking is checked by Nodes at every frame. If it is false, they will render as a node
    * and update their position. If it is true, they will render as an edge and update that edge to
    * track affiliated nodes.
    */
   public bool isLinking = false;

   public Table(string[] lines) {
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
         return null;
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
      if (fullList != null && fullList.Count > 0) {
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
         string cellValue = row[columnIndex];
         query.Add(cellValue, row);
      }

      m_cachedQueries[columnIndex] = query;
   }

   void ConsumeLines() {
      while(true) {
         lock(linesQueue) {
            int count = 0;
            while (parsedLineIndex < linesQueue.Length - 1 && count < linesPerFrame) {
               count++;
               parsedLineIndex++;
               string[] splitLine = CSVParser.SplitCsvLine(linesQueue[parsedLineIndex]);
               m_rows[parsedLineIndex] = new Row(this, splitLine);
            }

            if (parsedLineIndex == linesQueue.Length - 1) {
               parsedLineIndex++;
               OnFinishParse();
               return; // Ends the coroutine.
            }
         }
      }        
   }

   public Table(string[][] contents) {
      int numRows = contents.Length;
      m_rows = new Row[numRows];
      for (int rowIndex = 0 ; rowIndex < numRows ; rowIndex++) {
         string[] rowContents = contents[rowIndex];
         m_rows[rowIndex] = new Row(this, rowContents);
      }
      parsedLineIndex = m_rows.Length - 1;
      OnFinishParse();
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

}

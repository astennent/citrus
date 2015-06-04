using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class Table {
   private int consumedRowIndex = -1;
   private Row[] m_rows;
   private Thread m_loadingThread;
   private Thread m_unloadingThread;

   private int parsedLineIndex = -1; // linesQueue has been parsed up to and including this index.
   private string[] linesQueue;
   private int linesPerFrame = 1;
   private Thread m_parsingThread;


   private object threadStartAndStopper = new object(); //Used to lock around starting or ending threads.

   public Attribute[] attributes {get; private set; }

   public enum TableState {
      PARSING, // This will be set if the table is constructed using a string[]
      UNLOADED, // Indicates that all lines are parsed, but nodes are either unloaded or loading
      LOADED, // Indicates nodes are all loaded.
   }
   public TableState state {get; private set; }

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
      linesQueue = lines;
      parsedLineIndex = -1;
      state = TableState.PARSING;
      m_parsingThread = new System.Threading.Thread(ConsumeLines);
      m_loadingThread = new System.Threading.Thread(GenerateNodes);
      m_unloadingThread = new System.Threading.Thread(DestroyNodes);
      m_parsingThread.Start();
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
         JoinThread(m_unloadingThread);
         JoinThread(m_loadingThread);
         m_loadingThread.Start();
      }
   }

   public void Unload() {
      // Ensure that you are not Loading.
      lock (threadStartAndStopper) {
         JoinThread(m_loadingThread);
         JoinThread(m_unloadingThread);
         m_unloadingThread.Start();
      }
   }

   private void GenerateNodes() {
      while(true) {
         lock(linesQueue) {
            while (consumedRowIndex < parsedLineIndex) {
               consumedRowIndex++;
               NodeManager.Load(m_rows[consumedRowIndex]);
            }
            if (consumedRowIndex == m_rows.Length-1) {
               OnFinishLoad();
               return; // Ends the thread.
            }
         }
      }  
   }

   private void DestroyNodes() {
      lock(linesQueue) {
         NodeManager.Unload(m_rows);
         consumedRowIndex = -1;
         OnFinishUnload();
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

   private void OnFinishUnload() {
      // state will be parsing if we were still parsing, and we want to leave it that way.
      if (state != TableState.PARSING) {
         state = TableState.UNLOADED;
      }
   }

}

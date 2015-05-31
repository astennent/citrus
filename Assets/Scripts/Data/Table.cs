using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class Table {
   private int consumedRowIndex = -1;
   private Row[] m_rows;

   private int parsedLineIndex = -1; // linesQueue has been parsed up to and including this index.
   private string[] linesQueue;
   private int linesPerFrame = 1;

   private Thread m_parsingThread;
   private Thread m_loadingThread;

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
         for (int i = 0 ; i < attributes.Length ; i++) {
            Attribute attribute = attributes[i];
            attribute.name = (m_usesHeaders) ? m_rows[0][i] : ("Col" + i);
         }
      }
   }

   public Table(string[] lines) {
      m_rows = new Row[lines.Length];
      linesQueue = lines;
      parsedLineIndex = -1;
      state = TableState.PARSING;
      m_parsingThread = new System.Threading.Thread(ConsumeLines);
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
      parsedLineIndex = m_rows.Length - 1; // They started out parsed!

      OnFinishParse();
   }

   // Called after m_rows has been set or built by the constructor or coroutine.
   private void OnFinishParse() {
      state = TableState.UNLOADED;
      m_usesHeaders = true;

      int numColumns = m_rows[0].Length();
      attributes = new Attribute[numColumns];
      for (int i = 0 ; i < numColumns ; i++) {
         attributes[i] = new Attribute(m_rows[0][i]);
      }
      
   }

   public void Load() {
      m_loadingThread = new System.Threading.Thread(ConsumeRows);
      m_loadingThread.Start();
   }

   void ConsumeRows() {
      while(true) {
         lock(linesQueue) {
            while (consumedRowIndex < parsedLineIndex) {
               consumedRowIndex++;
               NodeGenerator.Generate(m_rows[consumedRowIndex]);
            }
            if (consumedRowIndex == m_rows.Length-1) {
               OnFinishLoad();
               return; // Ends the thread.
            }
         }
      }        
   }

   private void OnFinishLoad() {
      state = TableState.LOADED;
   }


}

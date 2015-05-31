using UnityEngine;
using System.Collections;

public class Row {

   private string[] m_contents;

   public Table table {get; private set;}

   public Row(Table _table, string[] contents) {
      m_contents = contents;
      table = _table;
   }

   // Bracket operator.
   public string this[int index]
   {
       get {
           return m_contents[index];
       }
       set {
           m_contents[index] = value;
       }
   }

   public int Length() {
      return m_contents.Length;
   }
	
}

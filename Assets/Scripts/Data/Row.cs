using UnityEngine;
using System.Collections;

public class Row {

   private string[] m_contents;
   private Node m_node; //The node assoicated with this row. TODO: Als could be edge.

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

   public void SetNode(Node node) {
      if (!node) {
         Utils.Assert("Tried to add an empty node. To destroy a node, use DestroyNode()");
         return;
      }
      if (m_node) {
         Utils.Assert("Overwritting a node. Old node should be destroyed first.");
         return;
      }

      m_node = node;
   }

   public void DestroyNode() {
      if (m_node) {
         GameObject.Destroy(m_node.gameObject);
         m_node = null;
      }
   }



}

using System.Collections.Generic;

class TableMap {
   private static int nextId = 0;
   private static Dictionary<int, Table> s_tableMap = new Dictionary<int, Table>();

   public static Table FromId(int id) {
      return s_tableMap[id];
   }

   public static int Register(Table table) {
      int id = nextId++;
      s_tableMap[id] = table;
      return id;
   }
}
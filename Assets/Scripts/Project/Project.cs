using System.Collections.Generic;

public class Project {

   public List<Table> tables {get; private set;}

   public Project() {
      tables = new List<Table>();
   }

   public Table OpenFile(string filename) {
      string[] lines = FileReader.ReadFile(filename); // Can this be threaded?
      var table = new Table(lines);
      tables.Add(table);
      return table;
   }

}

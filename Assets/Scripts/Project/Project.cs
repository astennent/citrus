using System.Collections.Generic;
using System.Runtime.Serialization;

[DataContract]
public class Project {

   [DataMember]
   public List<Table> tables {get; private set;}

   public Project() {
      tables = new List<Table>();
   }

   public Table OpenFile(string filename) {
      var table = Table.ConstructFromFilePath(filename);
      tables.Add(table);
      return table;
   }

}

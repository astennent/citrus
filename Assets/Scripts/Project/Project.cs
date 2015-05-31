public class Project {

   public void OpenFile(string filename) {
      string[] lines = FileReader.ReadFile(filename);
      Table table = new Table(lines);
      table.Load();
   }

}

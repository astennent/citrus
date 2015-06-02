public class Project {

   public void OpenFile(string filename) {
      string[] lines = FileReader.ReadFile(filename); // Can this be threaded?
      Table table = new Table(lines);
      table.Load();
   }

}

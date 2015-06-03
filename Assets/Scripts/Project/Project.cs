public class Project {

   public Table OpenFile(string filename) {
      string[] lines = FileReader.ReadFile(filename); // Can this be threaded?
      return new Table(lines);
   }

}

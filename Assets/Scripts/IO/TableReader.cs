
/* This should be extended by classes for reading CSVs and Database Wrappers. */
public interface TableReader {
   string[] GetHeaders();
   string[,] GetContents();
}

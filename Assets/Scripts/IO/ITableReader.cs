
/* This should be used by classes for reading CSVs and Database Wrappers. */
public interface ITableReader {

   /**
   * @return An array containing the names of the columns of the table
   */ 
   //string[] GetHeaders();

   /**
   * @return A 2D array with a row for every row in the table and a column per header.
   */
   //string[,] GetContents();
}

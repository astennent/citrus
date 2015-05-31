using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

/**
* Important TODO: Make this CSV compliant. Right now it doesn't support escaped line breaks.
*/
public class CSVReader : ITableReader {

   private string m_fileName;
   private string[] m_headers;
   private string[,] m_contents;

   public CSVReader(string fileName) {
      m_fileName = fileName;
      ReadFile();
   }

   public string[,] GetContents() {
      return m_contents;
   }

   public string[] GetHeaders() {
      return m_headers;
   }

   public void ReadFile() {
      string entireFile = ReadFile(m_fileName);
      string[] lines = entireFile.Split('\n');
      m_headers = SplitCsvLine(lines[0]);

      m_contents = new string[lines.Length-1, m_headers.Length];
      for (int rowIndex = 1 ; rowIndex < lines.Length ; rowIndex++) {
         string[] row = SplitCsvLine(lines[rowIndex]);
         for (int cellIndex = 0 ; cellIndex < m_headers.Length && cellIndex < row.Length; cellIndex++) {
            m_contents[rowIndex-1, cellIndex] = row[cellIndex];
         }
      }
      Utils.Log(m_contents[0,0]);
   }

   private static string ReadFile(string filePath)
   {
      byte[] buffer;
      FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
      try
      {
         int length = (int)fileStream.Length;  // get file length
         buffer = new byte[length];            // create buffer
         int count;                            // actual number of bytes read
         int sum = 0;                          // total number of bytes read

         // read until Read method returns 0 (end of the stream has been reached)
         while ((count = fileStream.Read(buffer, sum, length - sum)) > 0)
            sum += count;  // sum is a buffer offset for next reading
      }
      finally
      {
         fileStream.Close();
      }
      return  System.Text.Encoding.Default.GetString(buffer);
   }

   // splits a CSV row 
   private string[] SplitCsvLine(string line)
   {
      string pattern = @"
      # Match one value in valid CSV string.
      (?!\s*$)                                      # Don't match empty last value.
      \s*                                           # Strip whitespace before value.
      (?:                                           # Group for value alternatives.
        '(?<val>[^'\\]*(?:\\[\S\s][^'\\]*)*)'       # Either $1: Single quoted string,
      | ""(?<val>[^""\\]*(?:\\[\S\s][^""\\]*)*)""   # or $2: Double quoted string,
      | (?<val>[^,'""\s\\]*(?:\s+[^,'""\s\\]+)*)    # or $3: Non-comma, non-quote stuff.
      )                                             # End group of value alternatives.
      \s*                                           # Strip whitespace after value.
      (?:,|$)                                       # Field ends on comma or EOS.
      ";
  
      string[] values = (from Match m in Regex.Matches(line, pattern, 
         RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline)
         select m.Groups[1].Value).ToArray();
      return values;        
   }
}

using System.Collections;
using System.IO;


/**
* Important TODO: Make this CSV compliant. Right now it doesn't support escaped line breaks.
*/
public class FileReader : ITableReader {

   private string m_fileName;

   public static string[] ReadFile(string fileName) {
      string entireFile = ReadAsString(fileName).Trim();
      string[] lines = entireFile.Split('\n');
      return lines;
   }

   private static string ReadAsString(string filePath)
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


}

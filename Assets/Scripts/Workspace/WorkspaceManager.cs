using UnityEngine;

class WorkspaceManager : MonoBehaviour {
   
   void Start() {
      CSVReader reader = new CSVReader("C:\\Users\\tenne_000\\Desktop\\Alliance Edge List - Names.csv");
      Debug.Log(reader.GetContents());
   }

}
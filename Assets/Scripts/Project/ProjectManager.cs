using System.IO;
using UnityEngine;
using System.Runtime.Serialization;

class ProjectManager : MonoBehaviour {

   public static Project activeProject {get; private set;}
   
   void Start() {

      activeProject = OpenProject("DefaultProject.ctxp");
      string filename1 = "C:\\Users\\tenne_000\\Desktop\\Full Group Attributes.csv";
      string filename2 = "C:\\Users\\tenne_000\\Desktop\\Alliance Edge List - Names.csv";
      
      Table fullGroupAttrs = activeProject.OpenFile(filename1);
      fullGroupAttrs.Load();
      
      Table edgesList = activeProject.OpenFile(filename2);
      edgesList.AddForeignKey(0, fullGroupAttrs, 1);
      edgesList.AddForeignKey(1, fullGroupAttrs, 1);
      edgesList.isLinking = true;
      edgesList.Load();


      MemoryStream stream = new MemoryStream();
      DataContractSerializer ser = new DataContractSerializer(typeof(Project));
      ser.WriteObject(stream, edgesList);

      stream.Position = 0;
      StreamReader sr = new StreamReader(stream);
      Utils.Log(sr.ReadToEnd());
   }

   public Project OpenProject(string filename) {
      // TODO Use filename. 
      return new Project();
   }


}
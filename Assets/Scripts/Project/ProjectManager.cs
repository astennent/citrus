using UnityEngine;

class ProjectManager : MonoBehaviour {
   
   void Start() {

      Project project = OpenProject("DefaultProject.ctxp");
      string filename1 = "C:\\Users\\tenne_000\\Desktop\\Full Group Attributes.csv";
      string filename2 = "C:\\Users\\tenne_000\\Desktop\\Alliance Edge List - Names.csv";
      
      Table fullGroupAttrs = project.OpenFile(filename1);
      fullGroupAttrs.Load();
      
      Table edgesList = project.OpenFile(filename2);
      edgesList.AddForeignKey(0, fullGroupAttrs, 1);
      edgesList.AddForeignKey(1, fullGroupAttrs, 1);
      edgesList.isLinking = true;
      edgesList.Load();
   }

   public Project OpenProject(string filename) {
      // TODO Use filename. 
      return new Project();
   }


}
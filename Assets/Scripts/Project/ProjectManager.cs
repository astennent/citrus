using UnityEngine;

class ProjectManager : MonoBehaviour {
   
   void Start() {

      Project project = OpenProject("DefaultProject.ctxp");
      string filename1 = "C:\\Users\\tenne_000\\Desktop\\Full Group Attributes.csv";
      string filename2 = "C:\\Users\\tenne_000\\Desktop\\Alliance Edge List - Names.csv";
      Table fullGroupAttrs = project.OpenFile(filename1);
      Table edgesList = project.OpenFile(filename2);
      fullGroupAttrs.Load();

      // edgesList.isLinking = true;
      // edgesList.Load();
      fullGroupAttrs.Unload();

   }

   public Project OpenProject(string filename) {
      // TODO Use filename. 
      Utils.Log("TODO: ignoring filename");
      return new Project();
   }


}
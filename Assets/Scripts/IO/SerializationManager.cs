using UnityEngine;
using System.IO;
using System.Runtime.Serialization;

public class SerializationManager : MonoBehaviour {
   
   void Update() {
      if (Input.GetKeyDown(KeyCode.P)) {
         Save();
      }
   }

   void Save() {
      SaveState saveState = new SaveState();
      saveState.root = PanelManager.root.GetState();
      saveState.activeProject = ProjectManager.activeProject;

      MemoryStream stream = new MemoryStream();
      DataContractSerializer ser = new DataContractSerializer(typeof(SaveState));
      ser.WriteObject(stream, saveState);

      stream.Position = 0;
      StreamReader sr = new StreamReader(stream);
      Utils.Log(sr.ReadToEnd());
   }
}

class SaveState {
   public PanelState root;
   public Project activeProject;
}
using System.Runtime.Serialization;

[DataContract]
class CameraState : AppState {
   
   public CameraState() {
      type = "Camera";
   }

}
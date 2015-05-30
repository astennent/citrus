using UnityEngine;

public class CameraController : Controller {

   public static CameraController Instantiate() {
      CameraController controller = (CameraController)GameObject.Instantiate(PanelManager.GetCameraControllerPrefab(),
            Vector3.zero, new Quaternion(0,0,0,0));
      return controller;
   }

   public override void OnDisplayStart(ClientArea clientArea) {
      base.OnDisplayStart(clientArea);
      m_clientArea.GetComponent<UnityEngine.UI.Image>().color = new Color(0,0,0,0);
   }


   public override void OnDisplayEnd() {
      base.OnDisplayEnd();
      m_clientArea.GetComponent<UnityEngine.UI.Image>().color = new Color(0,0,0,0);
   }

   public override string GetName() {
      return "Camera";
   }

}
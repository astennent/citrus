using UnityEngine;

class CameraController : Controller {

   public override void DisplayOn(ClientArea clientArea) {
      base.DisplayOn(clientArea);
      m_clientArea.GetComponent<UnityEngine.UI.Image>().color = new Color(0,0,0,0);
   }

}
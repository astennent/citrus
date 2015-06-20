using UnityEngine;

public class DummyController : Controller {

   public static DummyController Instantiate() {
      DummyController controller = (DummyController)GameObject.Instantiate(PanelManager.GetDummyControllerPrefab(),
            Vector3.zero, new Quaternion(0,0,0,0));
      return controller;
   }

   public override string GetName() {
      return "Dummy";
   }

}
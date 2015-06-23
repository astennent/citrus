using UnityEngine;

public class DummyController : Controller {

   public static DummyController Instantiate() {
      DummyController controller = (DummyController)GameObject.Instantiate(
            ControllerPrefabs.Dummy, Vector3.zero, new Quaternion(0,0,0,0));
      return controller;
   }

   public override string GetName() {
      return "Dummy";
   }

}
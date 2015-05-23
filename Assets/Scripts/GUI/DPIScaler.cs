using UnityEngine;

public class DPIScaler : MonoBehaviour  {
   
   public static float ScaleFrom96(float input) {
      var dpi = Screen.dpi;
      if (dpi != 0) {
         return input * dpi / 96.0f;
      }
      return input;
   }

}
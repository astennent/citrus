using UnityEngine;

public class DPIScaler : MonoBehaviour  {
   
   public static float ScaleFrom96(float input) {
      return input * GetScalingFactor();
   }

   public static float GetScalingFactor() {
      var dpi = Screen.dpi;
      if (dpi == 0) {
         return 1f;
      }
      return dpi/96.0f;
   }

}
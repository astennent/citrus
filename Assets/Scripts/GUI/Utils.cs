using UnityEngine;

class Utils : MonoBehaviour
{
   public static Rect GetScreenRect(RectTransform rectTransform) 
   {
         Vector3[] corners = new Vector3[4];
 
         rectTransform.GetWorldCorners(corners);
 
         float xMin = float.PositiveInfinity;
         float xMax = float.NegativeInfinity;
         float yMin = float.PositiveInfinity;
         float yMax = float.NegativeInfinity;
 
         for (int i = 0; i < 4; i++) {
             Vector3 screenCoord = RectTransformUtility.WorldToScreenPoint(null, corners[i]);
             if (screenCoord.x < xMin)
                 xMin = screenCoord.x;
             if (screenCoord.x > xMax)
                 xMax = screenCoord.x;
             if (screenCoord.y < yMin)
                 yMin = screenCoord.y;
             if (screenCoord.y > yMax)
                 yMax = screenCoord.y;
         }
 
         Rect result = new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
         return result;
     }

     public static void Log(object obj) {
        Debug.Log(obj);
     }

     public static void Log(object[] array) {
        string output = "[";
        foreach(object obj in array) {
            output += obj + ", ";
        }
        Debug.Log(output + "]");
     }

}
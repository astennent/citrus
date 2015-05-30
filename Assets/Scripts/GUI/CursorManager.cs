using UnityEngine;
using System.Collections;

public class CursorManager : MonoBehaviour {

   public Texture2D leftRightArrow;
   public Texture2D upDownArrow;

   public Texture2D dragImage;


   static bool isDrawingResize = false;
   static bool resizeVerticalArrow = false;

   public static void StartDrawingResize(NestedPanel panel) {
      if (DragManager.IsDragging()) {
         return;
      }
      
      Cursor.visible = false;
      isDrawingResize = true;
      resizeVerticalArrow = !panel.IsSplitVertical();
   }

   public static void EndDrawingResize() {
      Cursor.visible = true;
      isDrawingResize = false;
   }

   public static void StartDrawingTabDrag() {
      // TODO: Screenshots!
   }

   public static void StopDrawingTabDrag() {

   }

   void OnGUI() {
      if (!DragManager.IsDragging() && !isDrawingResize) {
         return;
      }
      
      Rect cursorRect;
      Texture2D cursorImage;
      if (DragManager.IsDragging()) {
         DragTarget hit = DragManager.GetCurrentTarget();
         bool isOverCaption = (hit is CaptionBar);
         if (isOverCaption) {
            cursorRect = CreateRectOnMousePosition(Tab.GetTabSize());
            cursorImage = dragImage;
         } 
         else {
            cursorRect = CreateRectOnMousePosition(Tab.GetTabSize());
            cursorImage = dragImage;
         }
      }
      else {//(isDrawingResize) {
         float cursorSize = DPIScaler.ScaleFrom96(25);
         cursorRect = CreateRectOnMousePosition(Vector2.one * cursorSize);
         cursorImage = (resizeVerticalArrow) ? upDownArrow : leftRightArrow; 
      }

      GUI.DrawTexture(cursorRect, cursorImage);
   }

   private static Rect CreateRectOnMousePosition(Vector2 dimensions) {
      Vector2 mousePosition = Input.mousePosition;
      mousePosition.y = Screen.height - mousePosition.y;

      return new Rect(mousePosition.x - dimensions.x/2, 
               mousePosition.y - dimensions.y/2, dimensions.x, dimensions.y);
   }

}
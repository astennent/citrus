using UnityEngine;
using System.Collections;

public class CursorManager : MonoBehaviour {

   public Texture2D leftRightArrow;
   public Texture2D upDownArrow;

   public Texture2D dragImage;


   static bool isDrawingResize = false;
   static bool resizeVerticalArrow = false;

   static bool isDrawingDrag = false;

   public static void StartDrawingResize(NestedPanel panel) {
      Cursor.visible = false;
      isDrawingResize = true;
      resizeVerticalArrow = !panel.IsSplitVertical();
   }

   public static void EndDrawingResize() {
      Cursor.visible = true;
      isDrawingResize = false;
   }

   public static void StartDrawingTabDrag() {
      Cursor.visible = false;
      isDrawingDrag = true;
   }

   public static void EndDrawingTabDrag() {
      Cursor.visible = false;
      isDrawingDrag = false;
   }

   void OnGUI() {
      if (!isDrawingDrag && !isDrawingResize) {
         return;
      }

      
      Rect cursorRect;
      Texture2D cursorImage;
      if (isDrawingDrag) {
         DragTarget hit = PanelManager.GetDragTarget();
         //if (true) {
            cursorRect = CreateRectOnMousePosition(Tab.GetTabSize());
            cursorImage = dragImage;
         //} 

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
using UnityEngine;
using System.Collections;

public class CursorManager : MonoBehaviour {

   public Texture2D leftRightArrow;
   public Texture2D upDownArrow;
   public Texture2D diagonalArrow;

   public Texture2D dragImage;


   static bool m_isDrawingResize = false;
   static DragDirection m_direction = DragDirection.HORIZONTAL;

   public enum DragDirection {
      HORIZONTAL,
      VERTICAL,
      DIAGONAL // top left to botttom right. 
   }

   public static void StartDrawingResize(NestedPanel panel) {
      DragDirection direction = (panel == null) ? DragDirection.DIAGONAL :
                                (panel.IsSplitVertical()) ? DragDirection.VERTICAL :
                                DragDirection.HORIZONTAL;
      StartDrawingResize(direction);
   }

   public static void StartDrawingResize(DragDirection direction) {
      if (DragManager.IsDragging()) {
         return;
      }
      
      Cursor.visible = false;
      m_isDrawingResize = true;
      //resizeVerticalArrow = !panel.IsSplitVertical();
      m_direction = direction;
   }

   public static void EndDrawingResize() {
      Cursor.visible = true;
      m_isDrawingResize = false;
   }

   public static void StartDrawingTabDrag() {
      // TODO: Screenshots!
   }

   public static void StopDrawingTabDrag() {

   }

   void OnGUI() {
      if (!DragManager.IsDragging() && !m_isDrawingResize) {
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
      else {//(m_isDrawingResize) {
         float cursorSize = DPIScaler.ScaleFrom96(25);
         cursorRect = CreateRectOnMousePosition(Vector2.one * cursorSize);
         cursorImage = (m_direction == DragDirection.HORIZONTAL) ? upDownArrow :
                       (m_direction == DragDirection.VERTICAL) ? leftRightArrow :
                       diagonalArrow; 
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
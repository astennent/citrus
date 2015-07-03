using UnityEngine;
using System.Collections;

public class CursorManager : MonoBehaviour {

   public Texture2D leftRightArrow;
   public Texture2D upDownArrow;
   public Texture2D diagonalArrow;

   private static bool m_isDrawing = false;
   private static Texture2D m_cursorImage;
   private static Vector2 m_textureSize;

   private static CursorManager s_instance;

   public enum DragDirection {
      NONE,
      HORIZONTAL,
      VERTICAL,
      DIAGONAL // top left to botttom right. 
   }

   public static void StartDrawing(DragDirection direction) {
      Texture2D texture = (direction == DragDirection.HORIZONTAL) ? s_instance.leftRightArrow :
         (direction == DragDirection.VERTICAL) ? s_instance.upDownArrow :
         s_instance.diagonalArrow;
      Vector2 textureSize = Vector2.one * DPIScaler.ScaleFrom96(25f);
      StartDrawing(texture, textureSize);
   }

   public static void StartDrawing(Texture2D texture, Vector2 textureSize) {
      Cursor.visible = false;
      m_isDrawing = true;
      m_cursorImage = texture;
      m_textureSize = textureSize;
   }

   public static void EndDrawingResize() {
      Cursor.visible = true;
      m_isDrawing = false;
   }

   void Start() {
      s_instance = this;
   }

   void OnGUI() {
      if (!m_isDrawing) {
         return;
      }
      GUI.DrawTexture(CreateRectOnMousePosition(m_textureSize), m_cursorImage);
   }

   private static Rect CreateRectOnMousePosition(Vector2 dimensions) {
      Vector2 mousePosition = Input.mousePosition;
      mousePosition.y = Screen.height - mousePosition.y;

      return new Rect(mousePosition.x - dimensions.x/2, 
               mousePosition.y - dimensions.y/2, dimensions.x, dimensions.y);
   }

}
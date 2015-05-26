using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Resizer : MonoBehaviour {

   public NestedPanel m_panel;

   private bool m_isDragging = false;
   private bool m_isHovered = false;

   public void OnPointerDrag(BaseEventData data) {
      Vector2 delta = ((PointerEventData)data).delta;
      m_panel.OnDragResizer(delta);
   }

   public void OnPointerEnter(BaseEventData data) {
      m_isHovered = true;
      Cursor.visible = false;
   }

   public void OnPointerExit(BaseEventData data) {
      m_isHovered = false;
      Cursor.visible = !m_isDragging;
      Debug.Log(Cursor.visible);
   }

   public void OnPointerBeginDrag(BaseEventData data) {
      m_isDragging = true;
      Cursor.visible = false;
   }

   public void OnPointerEndDrag(BaseEventData data) {
      m_isDragging = false;
      Cursor.visible = !m_isHovered;
      Debug.Log(Cursor.visible);
   }

   void OnGUI() {
      if (!m_isHovered && !m_isDragging) {
         return;
      }

      Cursor.visible = false;
      float cursorSize = DPIScaler.ScaleFrom96(25);
      Texture2D cursorImage = (m_panel.IsSplitVertical()) 
         ? PanelManager.GetLeftRightArrow() 
         : PanelManager.GetUpDownArrow();
      GUI.DrawTexture(new Rect(Input.mousePosition.x - cursorSize/2, 
            Screen.height - Input.mousePosition.y - cursorSize/2, cursorSize, cursorSize), cursorImage);
   }
}

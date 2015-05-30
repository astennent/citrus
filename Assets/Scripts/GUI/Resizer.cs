using UnityEngine;
using System.Collections;
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
      CursorManager.StartDrawingResize(m_panel);
   }

   public void OnPointerExit(BaseEventData data) {
      m_isHovered = false;
      if (!m_isDragging) {
         CursorManager.EndDrawingResize();
      }
   }

   public void OnPointerBeginDrag(BaseEventData data) {
      m_isDragging = true;
      CursorManager.StartDrawingResize(m_panel);
   }

   public void OnPointerEndDrag(BaseEventData data) {
      m_isDragging = false;
      if (!m_isHovered) {
         CursorManager.EndDrawingResize();
      }
   }


}

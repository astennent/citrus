using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class Resizer : MonoBehaviour {

   public NestedPanel m_panel;

   private bool m_isDragging = false;
   private bool m_isHovered = false;

   private EventTrigger m_trigger;

   private event DragHandler DragEvent;
   public delegate void DragHandler(Vector2 delta);
   public void SubscribeToDrag(DragHandler handler) {
      DragEvent += handler;
   }

   void Start() {
      m_trigger = gameObject.AddComponent<EventTrigger>();
      m_trigger.delegates = new List<EventTrigger.Entry>();

      AddEvent(new UnityEngine.Events.UnityAction<BaseEventData>(OnPointerEnter), EventTriggerType.PointerEnter);
      AddEvent(new UnityEngine.Events.UnityAction<BaseEventData>(OnPointerExit), EventTriggerType.PointerExit);
      AddEvent(new UnityEngine.Events.UnityAction<BaseEventData>(OnPointerDrag), EventTriggerType.Drag);
      AddEvent(new UnityEngine.Events.UnityAction<BaseEventData>(OnPointerBeginDrag), EventTriggerType.BeginDrag);
      AddEvent(new UnityEngine.Events.UnityAction<BaseEventData>(OnPointerEndDrag), EventTriggerType.EndDrag);
   }

   private void AddEvent(UnityAction<BaseEventData> action, EventTriggerType triggerType) {
      EventTrigger.Entry entry = new EventTrigger.Entry();
      entry.eventID = triggerType;
      entry.callback = new EventTrigger.TriggerEvent();
      UnityEngine.Events.UnityAction<BaseEventData> callback = action;
      entry.callback.AddListener(callback);
      m_trigger.delegates.Add(entry);
   }

   public void OnPointerDrag(BaseEventData data) {
      Vector2 delta = ((PointerEventData)data).delta;
      if (DragEvent != null) {
         DragEvent(delta);
      }
      if (m_panel) {
         m_panel.OnDragResizer(delta);
      }
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

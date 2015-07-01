using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class Resizer : MonoBehaviour {

   public Texture2D customCursorImage; // Used if m_direction is not set
   public Vector2 customCursorSize = new Vector2(25, 25);
   public CursorManager.DragDirection direction = CursorManager.DragDirection.NONE;

   public bool ignoreHover = false;
   public bool ignoreDrag = false;

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

      if (!ignoreHover) {
         AddEvent(new UnityEngine.Events.UnityAction<BaseEventData>(OnPointerEnter), EventTriggerType.PointerEnter);
         AddEvent(new UnityEngine.Events.UnityAction<BaseEventData>(OnPointerExit), EventTriggerType.PointerExit);
      }

      if (!ignoreDrag) {
         AddEvent(new UnityEngine.Events.UnityAction<BaseEventData>(OnPointerDrag), EventTriggerType.Drag);
         AddEvent(new UnityEngine.Events.UnityAction<BaseEventData>(OnPointerBeginDrag), EventTriggerType.BeginDrag);
         AddEvent(new UnityEngine.Events.UnityAction<BaseEventData>(OnPointerEndDrag), EventTriggerType.EndDrag);
      }
   }

   private void AddEvent(UnityAction<BaseEventData> action, EventTriggerType triggerType) {
      EventTrigger.Entry entry = new EventTrigger.Entry();
      entry.eventID = triggerType;
      entry.callback = new EventTrigger.TriggerEvent();
      UnityEngine.Events.UnityAction<BaseEventData> callback = action;
      entry.callback.AddListener(callback);
      m_trigger.delegates.Add(entry);
   }

   public void SwitchToHorizontalCursor() {
      direction = CursorManager.DragDirection.HORIZONTAL;
   }
   public void SwitchToVerticalCursor(){
      direction = CursorManager.DragDirection.VERTICAL;
   }

   public void OnPointerDrag(BaseEventData data) {
      Vector2 delta = ((PointerEventData)data).delta;
      if (DragEvent != null) {
         DragEvent(delta);
      }
      //   m_panel.OnDragResizer(delta);
   }

   public void OnPointerEnter(BaseEventData data) {
      m_isHovered = true;
      StartDrawing();
   }

   public void OnPointerExit(BaseEventData data) {
      m_isHovered = false;
      if (!m_isDragging) {
         CursorManager.EndDrawingResize();
      }
   }

   public void OnPointerBeginDrag(BaseEventData data) {
      m_isDragging = true;
      StartDrawing();
   }

   public void OnPointerEndDrag(BaseEventData data) {
      m_isDragging = false;
      if (!m_isHovered) {
         CursorManager.EndDrawingResize();
      }
   }

   private void StartDrawing() {
      if (direction != CursorManager.DragDirection.NONE) {
         CursorManager.StartDrawing(direction);
      } else {
         CursorManager.StartDrawing(customCursorImage, DPIScaler.GetScalingFactor() * customCursorSize);
      }
   }


}

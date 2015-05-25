using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Resizer : MonoBehaviour {

   public NestedPanel m_panel;
   EventTrigger eventTrigger = null;

   void Start()
   {
      eventTrigger = gameObject.GetComponent<EventTrigger>();

      AddEventTrigger(OnPointerDrag, EventTriggerType.Drag);

   }

   private void AddEventTrigger(UnityAction action, EventTriggerType triggerType)
   {
      // Create a nee TriggerEvent and add a listener
      EventTrigger.TriggerEvent trigger = new EventTrigger.TriggerEvent();
      trigger.AddListener((eventData) => action()); // you can capture and pass the event data to the listener

      // Create and initialise EventTrigger.Entry using the created TriggerEvent
      EventTrigger.Entry entry = new EventTrigger.Entry() { callback = trigger, eventID = triggerType };

      // Add the EventTrigger.Entry to delegates list on the EventTrigger
      eventTrigger.delegates.Add(entry);
   }

   private void AddEventTrigger(UnityAction<BaseEventData> action, EventTriggerType triggerType)
   {
      // Create a nee TriggerEvent and add a listener
      EventTrigger.TriggerEvent trigger = new EventTrigger.TriggerEvent();
      trigger.AddListener((eventData) => action(eventData)); // you can capture and pass the event data to the listener

      // Create and initialise EventTrigger.Entry using the created TriggerEvent
      EventTrigger.Entry entry = new EventTrigger.Entry() { callback = trigger, eventID = triggerType };

      // Add the EventTrigger.Entry to delegates list on the EventTrigger
      eventTrigger.delegates.Add(entry);
   }

   void OnPointerDrag(BaseEventData data) {
      Vector2 delta = ((PointerEventData)data).delta;
      m_panel.OnDragResizer(delta);
   }
}

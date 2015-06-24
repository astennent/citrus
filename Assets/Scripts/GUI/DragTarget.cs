using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class DragTarget : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

   public abstract void HandleTabDrop(Tab tab);
   public abstract void OnPointerMoveWhileDragging(Vector2 mousePosition);

   // Start tracking the mouse
   public virtual void OnPointerEnter(PointerEventData eventData)
   {
      StartDragTracking();
   }

   // Stop tracking the mouse
   public virtual void OnPointerExit(PointerEventData eventData) {
      StopDragTracking();
   }

   public void StartDragTracking() {
      StartCoroutine( "TrackPointer" );  
      DragManager.SetDragTarget(this);
   }

   public void StopDragTracking() {
      StopCoroutine( "TrackPointer" );
   }

   IEnumerator TrackPointer() {
      while(Application.isPlaying) {
         if (DragManager.IsDragging()) {
            Vector2 localPos = GetLocalMousePosition();
            OnPointerMoveWhileDragging(localPos);
         }
         yield return 0;
      }        
   }

   protected Vector2 GetLocalMousePosition() {
      var ray = GetComponentInParent<GraphicRaycaster>();
      Vector2 localPos;
      RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, 
            Input.mousePosition, ray.eventCamera, out localPos);
      return localPos;
   }
}


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
     StartCoroutine( "TrackPointer" );  
     if (DragManager.IsDragging()) {
         DragManager.SetDragTarget(this);
      }          
   }

   // Stop tracking the mouse
   public virtual void OnPointerExit(PointerEventData eventData) {
      StopCoroutine( "TrackPointer" );
   }

   IEnumerator TrackPointer() {
      var ray = GetComponentInParent<GraphicRaycaster>();

      if(ray != null) {
         while(Application.isPlaying) {
            if (DragManager.IsDragging()) {
               Vector2 localPos;
               RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, 
                     Input.mousePosition, ray.eventCamera, out localPos );
               OnPointerMoveWhileDragging(localPos);
            }
            yield return 0;
         }        
      }
      else {
         UnityEngine.Debug.LogWarning( "Could not find GraphicRaycaster and/or StandaloneInputModule" );   
      }     
   }
}
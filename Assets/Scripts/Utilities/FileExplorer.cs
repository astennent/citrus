using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class FileExplorer : MonoBehaviour {

   public RectTransform rectTransform;
   public DraggableButton resizerButton;

   void Start () {
      resizerButton.SubscribeToDrag(OnDragResizer);
   }

   public void OnDragResizer(Vector2 delta) {
      Vector2 currentSize = rectTransform.sizeDelta;
      rectTransform.sizeDelta = new Vector2(currentSize.x + delta.x, currentSize.y - delta.y);
   }

}

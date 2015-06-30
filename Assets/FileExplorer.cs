using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class FileExplorer : MonoBehaviour {

   public RectTransform rectTransform;

   void Start () {

   }

   void Update () {

   }

   public void OnDragResizer(BaseEventData data) {
      Vector2 delta = ((PointerEventData)data).delta;
      Vector2 currentSize = rectTransform.sizeDelta;
      rectTransform.sizeDelta = new Vector2(currentSize.x + delta.x, currentSize.y - delta.y);
   }
}

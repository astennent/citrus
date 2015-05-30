using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public abstract class DragTarget : MonoBehaviour {
   public void OnPointerEnter(BaseEventData data) {
      PanelManager.NotifyDragTarget(this);
      GetComponent<UnityEngine.UI.Image>().color = new Color(Random.value, Random.value, Random.value);
   }

   public abstract void HandleTabDrop(Tab tab);
}
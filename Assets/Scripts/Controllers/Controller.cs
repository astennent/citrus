using UnityEngine;

public abstract class Controller : MonoBehaviour {

   protected ClientArea m_clientArea;

   public virtual void DisplayOn(ClientArea clientArea) {
      m_clientArea = clientArea;
      transform.SetParent(clientArea.transform);
      GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
      GetComponent<RectTransform>().sizeDelta = Vector2.zero;
   }


   public Rect GetBounds() {
      return m_clientArea.GetBounds();
   }

}
using UnityEngine;

public abstract class Controller : MonoBehaviour {

   protected ClientArea m_clientArea;

   public virtual void OnDisplayStart(ClientArea clientArea) {
      m_clientArea = clientArea;
      transform.SetParent(clientArea.transform);
      GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
      GetComponent<RectTransform>().sizeDelta = Vector2.zero;

      m_clientArea.GetComponent<UnityEngine.UI.Image>().color = GUISchemeManager.clientBackground;
   }

   public virtual void OnDisplayEnd() {
      // Clean up any damage you did to the client area.
   }

   public virtual void OnSize() {
      
   }

   public Rect GetBounds() {
      return m_clientArea.GetBounds();
   }

   public abstract string GetName();
}
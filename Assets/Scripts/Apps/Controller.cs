using UnityEngine;
using System.Runtime.Serialization;

[DataContract]
public abstract class Controller : MonoBehaviour {

   protected ClientArea m_clientArea;
   
   [DataMember]
   protected AppState m_state;

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

   public virtual void OnFocus() {

   }

   public virtual void OnBlur() {
      
   }

   public virtual void OnLButtonDown() {

   }
   
   public virtual void OnRButtonDown() {
      
   }

   public virtual void SetState(AppState state) {
      m_state = state;
   }

   public AppState GetState() {
      return m_state;
   }

   public Rect GetBounds() {
      return m_clientArea.GetBounds();
   }

   public abstract string GetName();
}
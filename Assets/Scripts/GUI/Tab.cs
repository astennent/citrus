using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Tab : MonoBehaviour {

   private Controller _controller;
   public Controller controller {
      get { return _controller; }
      set {
         _controller = value;
         text.text = controller.GetName();
      }
   }

   private CaptionBar _captionBar;
   public CaptionBar captionBar {
      get { return _captionBar; }
      set {
         _captionBar = value;
         transform.SetParent(value.transform); //Bring to the front.
         StartControllerDisplay();
      }
   }

   public static float GetWidth() {
      return DPIScaler.ScaleFrom96(70f);
   }
   public static float GetHeight() {
      return DPIScaler.ScaleFrom96(20f);
   }

   private int m_renderIndex; // only used for animations, don't rely on this for actual logic.

   public UnityEngine.UI.Text text;

	public static Tab Instantiate(Controller controller, CaptionBar captionBar) {
      Tab tab = (Tab)GameObject.Instantiate(PanelManager.GetTabPrefab(),
            Vector3.zero, new Quaternion(0,0,0,0));
      tab.controller = controller;
      tab.captionBar = captionBar;
      return tab;
   }

   public string GetName() {
      return controller.GetName();
   }

   public void OnPointerBeginDrag(BaseEventData data) {
      captionBar.RemoveTab(this);
      SetActive(false);
      transform.SetParent(PanelManager.GetBottomCanvas().transform); //Send to back
      DragManager.StartDrag();
   }

   public void OnPointerEndDrag(BaseEventData data) {
      DragTarget hit = DragManager.GetCurrentTarget();
      hit.HandleTabDrop(this);
      DragManager.EndDrag();
   }

   public void OnPointerClick(BaseEventData data) {
      captionBar.SelectTab(this);
   }

   public void SetActive(bool active) {
      var buttonComponent = GetComponent<UnityEngine.UI.Button>();
      UnityEngine.UI.ColorBlock colors = buttonComponent.colors;
      colors.normalColor = (active) 
         ? GUISchemeManager.activeTabNormal
         : GUISchemeManager.inactiveTabNormal;
      colors.highlightedColor = (active) 
         ? GUISchemeManager.activeTabHighlighted
         : GUISchemeManager.inactiveTabHighlighted;
      colors.pressedColor = GUISchemeManager.pressedTab;
      buttonComponent.colors = colors;

      controller.gameObject.SetActive(active);
      if (active) {
         StartControllerDisplay();
         controller.OnFocus();
      } else {
         controller.OnDisplayEnd();
         controller.OnBlur();
      }
   }

   public static Vector2 GetTabSize() {
      return new Vector2(GetWidth(), GetHeight());
   }

   public void SetRenderIndex(int i) {
      m_renderIndex = i;
      RectTransform rectTransform = GetComponent<RectTransform>();
      rectTransform.sizeDelta = new Vector2(GetWidth(), 0);
   }

   public void FinishAnimation() {
      GetComponent<RectTransform>().anchoredPosition = new Vector2(GetWidth() * m_renderIndex, 0);
   }

   private void StartControllerDisplay() {
      controller.OnDisplayStart(captionBar.panel.GetClientArea());
   }

   void Update() {
      RectTransform rectTransform = GetComponent<RectTransform>();
      float currentPosition = rectTransform.anchoredPosition.x;
      float desiredPosition = GetWidth() * m_renderIndex;
      float transitionPosition = Mathf.Lerp(currentPosition, desiredPosition, .5f);
      rectTransform.anchoredPosition = new Vector2(transitionPosition, 0);
   }
}

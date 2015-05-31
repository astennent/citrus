using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Tab : MonoBehaviour {

   public static float GetWidth() {
      return DPIScaler.ScaleFrom96(70f);
   }
   public static float GetHeight() {
      return DPIScaler.ScaleFrom96(20f);
   }

   private Controller m_controller;
   private CaptionBar m_captionBar;
   private int m_renderIndex; // only used for animations, don't rely on this for actual logic.

   public UnityEngine.UI.Text text;

	public static Tab Instantiate(Controller controller, CaptionBar captionBar) {
      Tab tab = (Tab)GameObject.Instantiate(PanelManager.GetTabPrefab(),
            Vector3.zero, new Quaternion(0,0,0,0));
      tab.SetController(controller);
      tab.SetCaptionBar(captionBar);
      return tab;
   }

   public void SetCaptionBar(CaptionBar captionBar) {
      m_captionBar = captionBar;
      transform.SetParent(captionBar.transform); //Bring to the front.
      StartControllerDisplay();
   }

   public void SetController(Controller controller) {
      m_controller = controller;
      text.text = controller.GetName();
   }

   public Controller GetController() {
      return m_controller;
   }

   public string GetName() {
      return m_controller.GetName();
   }

   public void OnPointerBeginDrag(BaseEventData data) {
      m_captionBar.RemoveTab(this);
      transform.SetParent(PanelManager.GetBottomCanvas().transform); //Send to back
      DragManager.StartDrag();
   }

   public void OnPointerEndDrag(BaseEventData data) {
      DragTarget hit = DragManager.GetCurrentTarget();
      hit.HandleTabDrop(this);
      DragManager.EndDrag();
   }

   public void OnPointerClick(BaseEventData data) {
      m_captionBar.SelectTab(this);
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

      m_controller.gameObject.SetActive(active);
      if (active) {
         StartControllerDisplay();
      } else {
         m_controller.OnDisplayEnd();
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
      m_controller.OnDisplayStart(m_captionBar.panel.GetClientArea());
   }

   void Update() {
      RectTransform rectTransform = GetComponent<RectTransform>();
      float currentPosition = rectTransform.anchoredPosition.x;
      float desiredPosition = GetWidth() * m_renderIndex;
      float transitionPosition = Mathf.Lerp(currentPosition, desiredPosition, .5f);
      rectTransform.anchoredPosition = new Vector2(transitionPosition, 0);
   }
}

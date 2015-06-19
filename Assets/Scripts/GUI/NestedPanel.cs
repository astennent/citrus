using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NestedPanel : MonoBehaviour {
   private bool m_splitVertical; // if true, split is drawn up down, children are on left and right.
   private float m_splitRatio; // Distance from the top or left.
   
   public NestedPanel m_parent;
   public NestedPanel m_firstChild; // top or left
   public NestedPanel m_secondChild; // bottom or right

   public CaptionBar m_captionBar;
   public ClientArea m_clientArea;

   public UnityEngine.UI.Button m_resizerButton;
   private static int resizerWidth = 6;

   public RectTransform m_tabHolderTransform;

   private static NestedPanel _focusedPanel;
   public static NestedPanel focusedPanel {
      get {
         return _focusedPanel;
      }
      set {
         if (_focusedPanel == value) {
            return;
         }
         if (_focusedPanel) {
            _focusedPanel.OnBlur();
         }
         _focusedPanel = value;
         _focusedPanel.OnFocus();
      }
   }

   static NestedPanel Instantiate(NestedPanel parent) {
      NestedPanel panel = (NestedPanel)GameObject.Instantiate(PanelManager.GetNestedPanelPrefab(),
            Vector3.zero, new Quaternion(0,0,0,0));
      panel.m_parent = parent;
      panel.transform.SetParent(parent.transform);
      
      // Position new panel directly on top of the parent.
      RectTransform transform = panel.GetComponent<RectTransform>();
      transform.sizeDelta = Vector2.zero;
      transform.anchoredPosition = Vector2.zero;

      // Set up resizer.
      panel.m_resizerButton.gameObject.SetActive(false);

      panel.m_captionBar.GetComponent<UnityEngine.UI.Image>().color = GUISchemeManager.inactiveCaption;

      return panel;
   }

   void Start() {
      RectTransform captionTransform = this.m_captionBar.GetComponent<RectTransform>();
      captionTransform.sizeDelta = new Vector2(captionTransform.sizeDelta.x, Tab.GetHeight());
      captionTransform.anchoredPosition = new Vector2(0, 0);

      RectTransform clientTransform = this.m_clientArea.GetComponent<RectTransform>();
      clientTransform.anchoredPosition = new Vector2(clientTransform.anchoredPosition.x, -Tab.GetHeight());
   }

   void Update() {
      if (IsLeaf() && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && 
            ContainsMouse()) {
         NestedPanel.focusedPanel = this;
      }
   }

   public void AddTab(Tab tab) {
      m_captionBar.AddTab(tab);
      NestedPanel.focusedPanel = this;
   }

   public void AddTab(Controller controller) {
      m_captionBar.AddTab(controller);
      NestedPanel.focusedPanel = this;
   }

   public void Split(Tab insertedTab, bool isSplitVertical, bool newTabIsFirst) {
      m_splitVertical = isSplitVertical;

      m_firstChild = NestedPanel.Instantiate(this);
      m_secondChild = NestedPanel.Instantiate(this);

      Tab selectedTab = m_captionBar.GetSelectedTab();
      NestedPanel protege = (newTabIsFirst) ? m_secondChild : m_firstChild;
      NestedPanel imposter = (newTabIsFirst) ? m_firstChild : m_secondChild;
      foreach (Tab tab in m_captionBar.GetTabs()) {
         protege.AddTab(tab);
      }

      //Activate resizer.
      m_resizerButton.gameObject.SetActive(true);

      // Deactivate self.
      m_captionBar.gameObject.SetActive(false);
      m_clientArea.gameObject.SetActive(false);
      GetComponent<UnityEngine.UI.Image>().enabled = (false);

      float splitRatio = ClientArea.fillTolerance;
      if (newTabIsFirst != isSplitVertical) {
         splitRatio = 1-splitRatio;
      }

      protege.m_captionBar.SelectTab(selectedTab);
      imposter.AddTab(insertedTab);

      SetSplitRatio(splitRatio);
   } 

   public void Merge(NestedPanel deadChild, Controller orphanedController) {
      NestedPanel livingChild = (m_firstChild == deadChild) ? m_secondChild : m_firstChild;

      // Update living child's parent pointer.
      livingChild.m_parent = m_parent;
      orphanedController.transform.SetParent(livingChild.GetClientArea().transform);

      if (m_parent) {
         // Update parent's child pointer.
         if (m_parent.m_firstChild == this) {
            m_parent.m_firstChild = livingChild;
         } 
         else {
            m_parent.m_secondChild = livingChild;
         }

         Transform parentTransform = m_parent.transform;
         livingChild.transform.SetParent(parentTransform);
      
         // Recalculate sizes
         PanelManager.root.Redraw();
      } 
      else {
         // If this is the root, the living child becomes the new root.
         PanelManager.root = livingChild;
         RectTransform rootTransform = livingChild.GetComponent<RectTransform>();
         rootTransform.anchoredPosition = Vector2.zero;
         rootTransform.sizeDelta = Vector2.zero;
         PanelManager.root.RedrawResizer();
      }

      // In case something goes wrong and no MousePointerEnter event is triggered before dropping
      // off the tab that triggered this merge, set the living child as the default drag target.
      DragManager.SetDragTarget(livingChild.GetClientArea());

      Destroy(gameObject); // Destroy the middle-man.
   }

   public void OnLastTabRemoved(Controller orphanedController) {
      m_parent.Merge(this, orphanedController);
   }

   public void OnDragResizer(Vector2 mouseDelta) {
      float dragDistance = (m_splitVertical) ? mouseDelta.x : mouseDelta.y;
      float fullDistance = (m_splitVertical) ? GetRect().width : GetRect().height;
      float normalizedDelta = dragDistance / fullDistance;
      SetSplitRatio(m_splitRatio + normalizedDelta);
   }

   private void SetSplitRatio(float ratio) {
      m_splitRatio = ratio;   
      Redraw();
   }

   /**
    * Called to handle events like resizing. Ensures that this panel and all children are drawn
    * with their correctly updated sizes. 
    */
   private void Redraw() {
      if (IsLeaf()) {
         RedrawLeaf();
      }
      else {
         RedrawBranch();
      }
   }

   private void RedrawLeaf() {
      m_captionBar.GetSelectedTab().controller.OnSize();
   }

   private void RedrawBranch() {
      RedrawResizer();
      
      float inverseSplitRatio = 1 - m_splitRatio; 
      float scaledResizerWidth = DPIScaler.ScaleFrom96(resizerWidth);
      RectTransform firstTransform = m_firstChild.GetComponent<RectTransform>();
      RectTransform secondTransform = m_secondChild.GetComponent<RectTransform>();

      Rect rect = GetRect();

      if (m_splitVertical) {
         firstTransform.anchoredPosition = new Vector2(rect.xMin, rect.yMin);
         firstTransform.sizeDelta = new Vector2(-rect.width * inverseSplitRatio - scaledResizerWidth/2, 0);
         secondTransform.sizeDelta = new Vector2(-rect.width * m_splitRatio - scaledResizerWidth/2, 0);
         secondTransform.anchoredPosition = new Vector2(rect.xMin + rect.width*m_splitRatio + scaledResizerWidth/2, rect.yMin);
      }
      else {
         secondTransform.anchoredPosition = new Vector2(rect.xMin, rect.yMin);
         secondTransform.sizeDelta = new Vector2(0, -rect.height * inverseSplitRatio - scaledResizerWidth);
         firstTransform.sizeDelta = new Vector2(0, -rect.height * m_splitRatio - scaledResizerWidth);
         firstTransform.anchoredPosition = new Vector2(rect.xMin, rect.yMin + rect.height*m_splitRatio + scaledResizerWidth);
      }

      m_firstChild.Redraw();
      m_secondChild.Redraw();
   }

   private void RedrawResizer() {
      Rect rect = GetRect();
      RectTransform resizerTransform = m_resizerButton.GetComponent<RectTransform>();
      float scaledResizerWidth = DPIScaler.ScaleFrom96(resizerWidth);

      if (m_splitVertical) {
         resizerTransform.anchoredPosition = new Vector2(rect.width * m_splitRatio - scaledResizerWidth/2, -scaledResizerWidth);
         resizerTransform.sizeDelta = new Vector2(scaledResizerWidth, rect.height + scaledResizerWidth);
      }
      else {
         resizerTransform.anchoredPosition = new Vector2(0, rect.height * m_splitRatio - scaledResizerWidth);
         resizerTransform.sizeDelta = new Vector2(rect.width, scaledResizerWidth);
      }
      
   }

   public Rect GetRect() {
      return GetComponent<RectTransform>().rect;
   }

   public ClientArea GetClientArea() {
      return m_clientArea;
   }

   public bool ContainsMouse() {
      return RectTransformUtility.RectangleContainsScreenPoint(GetComponent<RectTransform>(), 
            Input.mousePosition, null);
   }

   public bool IsLeaf() {
      return (m_firstChild == null);
   }

   public bool IsSplitVertical() {
      return m_splitVertical;
   }

   private void OnFocus() {
      m_captionBar.GetComponent<UnityEngine.UI.Image>().color = GUISchemeManager.activeCaption;
      m_captionBar.GetSelectedTab().controller.OnFocus();
   }

   private void OnBlur() {
      m_captionBar.GetComponent<UnityEngine.UI.Image>().color = GUISchemeManager.inactiveCaption;
      m_captionBar.GetSelectedTab().controller.OnBlur();
   }

   public PanelState GetState() {
      PanelState state = new PanelState();
      if (IsLeaf()) {
         state.appStates = new List<AppState>();
         foreach (Tab tab in m_captionBar.GetTabs()) {
            AppState appState = tab.controller.GetState();
            state.appStates.Add(appState);
         }
      } 
      else {
         state.splitRatio = m_splitRatio;
         state.splitVertical = m_splitVertical;
         state.firstChild = m_firstChild.GetState();
         state.secondChild = m_secondChild.GetState(); 
      }
      return state;
   }

}

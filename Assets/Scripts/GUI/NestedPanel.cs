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
   
   public UnityEngine.UI.Image m_clientArea;

   public UnityEngine.UI.Button m_resizerButton;
   private static int resizerWidth = 6;

   public RectTransform m_tabHolderTransform;

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

      return panel;
   }

   void Start() {
      // Scale the caption to DPI.
      float captionHeight = CaptionBar.height;
      RectTransform captionTransform = this.m_captionBar.GetComponent<RectTransform>();
      captionTransform.sizeDelta = new Vector2(captionTransform.sizeDelta.x, captionHeight);
      captionTransform.anchoredPosition = new Vector2(0, 0);

      RectTransform clientTransform = this.m_clientArea.GetComponent<RectTransform>();
      clientTransform.anchoredPosition = new Vector2(clientTransform.anchoredPosition.x, -captionHeight);
   }

   public void AddTab(Tab tab) {
      m_captionBar.AddTab(tab);
   }

   public void AddTab(string tabName) {
      m_captionBar.AddTab(tabName);
   }

   public void Split(Tab insertedTab, bool isSplitVertical, bool newTabIsFirst) {
      m_splitVertical = isSplitVertical;

      m_firstChild = NestedPanel.Instantiate(this);
      m_secondChild = NestedPanel.Instantiate(this);

      NestedPanel protege = (newTabIsFirst) ? m_secondChild : m_firstChild;
      NestedPanel imposter = (newTabIsFirst) ? m_firstChild : m_secondChild;
      foreach (Tab tab in m_captionBar.GetTabs()) {
         protege.AddTab(tab);
      }
      imposter.AddTab(insertedTab);

      //Activate resizer.
      m_resizerButton.gameObject.SetActive(true);

      // Deactivate self.
      m_captionBar.gameObject.SetActive(false);
      m_clientArea.gameObject.SetActive(false);
      GetComponent<UnityEngine.UI.Image>().enabled = (false);

      SetSplitRatio(0.5f);
   } 

   public void Merge(NestedPanel deadChild) {
      NestedPanel livingChild = (m_firstChild == deadChild) ? m_secondChild : m_firstChild;

      // Update living child's parent pointer.
      livingChild.m_parent = m_parent;

      if (m_parent) {
         // Update parent's child pointer.
         if (m_parent.m_firstChild == this) {
            m_parent.m_firstChild = livingChild;
         } else {
            m_parent.m_secondChild = livingChild;
         }

         Transform parentTransform = m_parent.transform;
         livingChild.transform.SetParent(parentTransform);
      
         // Recalculate sizes
         PanelManager.GetRoot().Redraw();
      } 
      else {
         // If this is the root, the living child becomes the new root.
         PanelManager.SetRoot(livingChild);
         RectTransform rootTransform = livingChild.GetComponent<RectTransform>();
         rootTransform.anchoredPosition = Vector2.zero;
         rootTransform.sizeDelta = Vector2.zero;
      }

      Destroy(gameObject); // Destroy the middle-man.
   }

   public void OnLastTabRemoved() {
      m_parent.Merge(this);
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
      if (!m_firstChild || !m_secondChild) {
         return;
      }

      float inverseSplitRatio = 1 - m_splitRatio; 
      float scaledResizerWidth = DPIScaler.ScaleFrom96(resizerWidth);
      RectTransform firstTransform = m_firstChild.GetComponent<RectTransform>();
      RectTransform secondTransform = m_secondChild.GetComponent<RectTransform>();

      Rect rect = GetRect();
      RectTransform resizerTransform = m_resizerButton.GetComponent<RectTransform>();
      if (m_splitVertical) {
         resizerTransform.anchoredPosition = new Vector2(rect.width * m_splitRatio - scaledResizerWidth/2, -scaledResizerWidth);
         resizerTransform.sizeDelta = new Vector2(scaledResizerWidth, rect.height + scaledResizerWidth);
      }
      else {
         resizerTransform.anchoredPosition = new Vector2(0, rect.height * m_splitRatio - scaledResizerWidth);
         resizerTransform.sizeDelta = new Vector2(rect.width, scaledResizerWidth);
      }
      
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

   public Rect GetRect() {
      return GetComponent<RectTransform>().rect;
   }

   public bool ContainsMouse() {
      return RectTransformUtility.RectangleContainsScreenPoint(GetComponent<RectTransform>(), 
            Input.mousePosition, Camera.main);
   }

   public bool IsLeaf() {
      return (m_firstChild == null);
   }

   public bool IsSplitVertical() {
      return m_splitVertical;
   }
}

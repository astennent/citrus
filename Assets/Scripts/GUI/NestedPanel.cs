using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NestedPanel : MonoBehaviour {

   private bool m_splitVertical; // if true, split is drawn up down, children are on left and right.
   private float m_splitRatio; // Distance from the top or left.
   
   public NestedPanel m_firstChild; // top or left
   public NestedPanel m_secondChild; // bottom or right

   public UnityEngine.UI.Image m_captionRect;
   private static int captionRectHeight = 20;
   
   public UnityEngine.UI.Image m_clientArea;

   public UnityEngine.UI.Button m_resizerButton;
   private static int resizerWidth = 6;

   public RectTransform m_tabHolderTransform;

   private List<Tab> m_tabs;

   static NestedPanel Instantiate(Transform parent) {
      NestedPanel panel = (NestedPanel)GameObject.Instantiate(PanelManager.GetNestedPanelPrefab(),
            Vector3.zero, new Quaternion(0,0,0,0));
      panel.transform.SetParent(parent);
      
      // Position new panel directly on top of the parent.
      RectTransform transform = panel.GetComponent<RectTransform>();
      transform.sizeDelta = Vector2.zero;
      transform.anchoredPosition = Vector2.zero;

      // Set up tabs
      panel.m_tabs = new List<Tab>();
      panel.AddTab("Inspector");
      panel.AddTab("Camera");

      // Set up resizer.
      panel.m_resizerButton.gameObject.SetActive(false);


      return panel;
   }

   // Update is called once per frame
   void Update () {
      if (Input.GetButtonDown("Jump") && m_firstChild == null) {
         Split(true);
      }
   }

   void Start() {
      // Scale the caption to DPI.
      int captionHeight = (int)DPIScaler.ScaleFrom96(captionRectHeight);
      RectTransform captionTransform = this.m_captionRect.GetComponent<RectTransform>();
      captionTransform.sizeDelta = new Vector2(captionTransform.sizeDelta.x, captionHeight);
      captionTransform.anchoredPosition = new Vector2(0, 0);

      RectTransform clientTransform = this.m_clientArea.GetComponent<RectTransform>();
      clientTransform.anchoredPosition = new Vector2(clientTransform.anchoredPosition.x, -captionHeight);
   }

   void AddTab(string name) {
      Tab tab = Tab.Instantiate(name, m_tabHolderTransform);
      RectTransform tabTransform = tab.GetComponent<RectTransform>();
      float scaledTabWidth = DPIScaler.ScaleFrom96(Tab.width);
      tabTransform.sizeDelta = new Vector2(scaledTabWidth, 0);
      tabTransform.anchoredPosition = new Vector2(scaledTabWidth * m_tabs.Count, 0);
      m_tabs.Add(tab);
   }

   void Split(bool vertical) {
      m_splitVertical = vertical;
      m_firstChild = NestedPanel.Instantiate(transform);
      m_secondChild = NestedPanel.Instantiate(transform);

      //Activate resizer.
      m_resizerButton.gameObject.SetActive(true);

      SetSplitRatio(0.5f);

      // Deactivate self.
      m_captionRect.gameObject.SetActive(false);
      m_clientArea.gameObject.SetActive(false);
      GetComponent<UnityEngine.UI.Image>().enabled = (false);


   }

   public void OnDragResizer(Vector2 mouseDelta) {
      float dragDistance = (m_splitVertical) ? mouseDelta.x : mouseDelta.y;
      float fullDistance = (m_splitVertical) ? GetRect().width : GetRect().height;
      float normalizedDelta = dragDistance / fullDistance;
      m_splitRatio += normalizedDelta;
      Debug.Log(m_splitRatio);
   }

   private void SetSplitRatio(float ratio) {
      m_splitRatio = ratio;      

      float scaledResizerWidth = DPIScaler.ScaleFrom96(resizerWidth);

      RectTransform firstTransform = m_firstChild.GetComponent<RectTransform>();
      RectTransform secondTransform = m_secondChild.GetComponent<RectTransform>();

      Rect rect = GetRect();
      RectTransform resizerTransform = m_resizerButton.GetComponent<RectTransform>();
      if (m_splitVertical) {
         resizerTransform.sizeDelta = new Vector2(scaledResizerWidth, rect.height);
      } else {
         resizerTransform.sizeDelta = new Vector2(rect.width, scaledResizerWidth);
      }
      
      firstTransform.anchoredPosition = new Vector2(rect.xMin, rect.yMin);
      if (m_splitVertical) {
         firstTransform.sizeDelta = new Vector2(-rect.width/2 - scaledResizerWidth/2, 0);
         secondTransform.sizeDelta = new Vector2(-rect.width/2 - scaledResizerWidth/2, 0);
         secondTransform.anchoredPosition = new Vector2(rect.xMin + rect.width/2 + scaledResizerWidth/2, rect.yMin);
      } else {
         firstTransform.sizeDelta = new Vector2(0, -rect.height/2 - scaledResizerWidth/2);
         secondTransform.sizeDelta = new Vector2(0, -rect.height/2 + scaledResizerWidth/2);
         secondTransform.anchoredPosition = new Vector2(rect.xMin, rect.yMin + rect.height/2 + scaledResizerWidth/2);
      }


   }

   Rect GetRect() {
      return GetComponent<RectTransform>().rect;
   }
}

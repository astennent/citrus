using UnityEngine;
using System.Collections;

public class NestedPanel : MonoBehaviour {

   private bool m_splitVertical; // if true, split is drawn up down, children are on left and right.
   private float m_splitRatio; // Distance from the top or left.
   
   public NestedPanel m_firstChild; // top or left
   public NestedPanel m_secondChild; // bottom or right

   public UnityEngine.UI.Image m_captionRect;
   private static int captionRectHeight = 20;
   public UnityEngine.UI.Image m_clientArea;

   static NestedPanel Instantiate(Transform parent) {
      NestedPanel panel = (NestedPanel)GameObject.Instantiate(PanelManager.GetNestedPanelPrefab(),
            Vector3.zero, new Quaternion(0,0,0,0));
      panel.transform.SetParent(parent);
      
      // Position new panel directly on top of the parent.
      RectTransform transform = panel.GetComponent<RectTransform>();
      transform.sizeDelta = Vector2.zero;
      transform.anchoredPosition = Vector2.zero;

      return panel;
   }

   // Update is called once per frame
   void Update () {
      if (Input.GetMouseButtonDown(0) && m_firstChild == null) {
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

   void Split(bool vertical) {
      m_splitVertical = vertical;
      m_splitRatio = 0.5f;
      m_firstChild = NestedPanel.Instantiate(transform);
      m_secondChild = NestedPanel.Instantiate(transform);

      // Chlidren are created with the same size of the parent (this)
      RectTransform firstTransform = m_firstChild.GetComponent<RectTransform>();
      RectTransform secondTransform = m_secondChild.GetComponent<RectTransform>();
      Rect rect = GetRect();
      
      if (m_splitVertical) {
         firstTransform.sizeDelta = new Vector2(-rect.width/2, 0);
         firstTransform.anchoredPosition = new Vector2(rect.xMin, rect.yMin);
         secondTransform.sizeDelta = new Vector2(-rect.width/2, 0);
         secondTransform.anchoredPosition = new Vector2(rect.xMin + rect.width/2, rect.yMin);
      } else {
         firstTransform.sizeDelta = new Vector2(0, -rect.height/2);
         firstTransform.anchoredPosition = new Vector2(rect.xMin, rect.yMin);
         secondTransform.sizeDelta = new Vector2(0, -rect.height/2);
         secondTransform.anchoredPosition = new Vector2(rect.xMin, rect.yMin + rect.height/2);
      }

      m_captionRect.gameObject.SetActive(false);
      m_clientArea.gameObject.SetActive(false);
   }

   Rect GetRect() {
      return GetComponent<RectTransform>().rect;
   }
}

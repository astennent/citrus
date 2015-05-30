using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CaptionBar : DragTarget {
   public static float height = DPIScaler.ScaleFrom96(20);

   public NestedPanel panel;
   private List<Tab> m_tabs = new List<Tab>();

   public override void HandleTabDrop(Tab tab) {
      AddTab(tab);
   }

   public override void OnPointerMoveWhileDragging(Vector2 mousePosition) {

   }

   public void AddTab(string tabName) {
      Tab tab = Tab.Instantiate(tabName, this);
      AddTab(tab);
      SelectTab(tab);
   }

   public void AddTab(Tab tab) {
      tab.SetCaptionBar(this);
      if (!m_tabs.Contains(tab)) {
         m_tabs.Add(tab);
      }
      Redraw();
   }

   public void RemoveTab(Tab tab) {
      m_tabs.Remove(tab);
      if (m_tabs.Count == 0) {
         panel.OnLastTabRemoved();
      } else {
         Redraw();
      }
   }

   public void SelectTab(Tab selectedTab) {
      if (!m_tabs.Contains(selectedTab)) {
         return;
      }
      foreach (Tab tab in m_tabs) {
         tab.SetActive(tab == selectedTab);
      }
   }

   public void Redraw() {
      for (int i = 0 ; i < m_tabs.Count ; i++) {
         Tab tab = m_tabs[i];
         RectTransform tabTransform = tab.GetComponent<RectTransform>();
         tabTransform.sizeDelta = new Vector2(Tab.width, 0);
         tabTransform.anchoredPosition = new Vector2(Tab.width * i, 0);
      }
   }

   public List<Tab> GetTabs() {
      return m_tabs;
   }

}

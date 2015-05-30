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

   public void AddTab(string tabName) {
      Tab tab = Tab.Instantiate(tabName, this);
      AddTab(tab);
   }

   public void AddTab(Tab tab) {
      tab.SetCaptionBar(this);
      m_tabs.Add(tab);
      RedrawTabs();
   }

   public void RemoveTab(Tab tab) {
      m_tabs.Remove(tab);
      RedrawTabs();
   }

   public void RedrawTabs() {
      for (int i = 0 ; i < m_tabs.Count ; i++) {
         Tab tab = m_tabs[i];
         RectTransform tabTransform = tab.GetComponent<RectTransform>();
         tabTransform.sizeDelta = new Vector2(Tab.width, 0);
         tabTransform.anchoredPosition = new Vector2(Tab.width * i, 0);
      }
   }

}

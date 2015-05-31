using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class CaptionBar : DragTarget {

   public NestedPanel panel;
   private List<Tab> m_tabs = new List<Tab>();
   private int m_selectedIndex = 0;

   public override void HandleTabDrop(Tab tab) {
      int insertionIndex = GetTabInsertionIndex(GetLocalMousePosition());
      AddTab(tab, insertionIndex);
   }

   public override void OnPointerMoveWhileDragging(Vector2 mousePosition) {
      int potentialInsertionIndex = GetTabInsertionIndex(mousePosition);
      for (int i = 0 ; i < m_tabs.Count ; i++) {
         int adjust = (i >= potentialInsertionIndex) ? 1 : 0;
         Tab tab = m_tabs[i];
         tab.SetRenderIndex(i + adjust);
      }
   }

   // Returns 0 if it should be at the far left, m_tabs.Length if far right.
   private int GetTabInsertionIndex(Vector2 mousePosition) {
      int index = (int)(mousePosition.x / Tab.GetWidth());
      return Mathf.Clamp(index, 0, m_tabs.Count);
   }

   public void AddTab(Controller controller) {
      Tab tab = Tab.Instantiate(controller, this);
      AddTab(tab);
      SelectTab(tab);
   }

   public void AddTab(Tab tab, int insertionIndex = -1) {
      tab.SetCaptionBar(this);
      if (m_tabs.Contains(tab)) {
         return;
      }

      // Deactivate the old selected tab.
      if (m_tabs.Count > 0) {
         m_tabs[m_selectedIndex].SetActive(false);
      }

      if (insertionIndex == -1) {
         insertionIndex = m_tabs.Count;
      }
      m_tabs.Insert(insertionIndex, tab);

      SelectTab(insertionIndex);
      Redraw();
      tab.FinishAnimation();
   }

   public void RemoveTab(Tab tab) {
      int tabIndex = m_tabs.IndexOf(tab);

      if (tabIndex == -1) {
         Debug.LogError("Attempted to remove a tab that wasn't present.");
         return;
      }

      m_tabs.Remove(tab);

      if (m_selectedIndex > tabIndex) {
         m_selectedIndex -= 1;
      }
      else if (m_selectedIndex == tabIndex && m_tabs.Count > 0) {
         if (tabIndex == m_tabs.Count) {
            m_selectedIndex -= 1;
         }
         SelectTab(m_selectedIndex);
      } 

      if (m_tabs.Count == 0) {
         Controller orphanedController = tab.GetController();
         panel.OnLastTabRemoved(orphanedController);
      } 
      else {
         Redraw();
      }

   }

   public void SelectTab(Tab selectedTab) {
      int tabIndex = m_tabs.IndexOf(selectedTab);
      if (tabIndex == -1) {
         Debug.LogError("Attempted to select a tab that wasn't present.");
         return;
      }
      SelectTab(tabIndex);
   }

   public void SelectTab(int index) {
      if (index < 0 || index >= m_tabs.Count) {
         Debug.LogError("Tab index out of range: " + index);
         return;
      }

      m_tabs[m_selectedIndex].SetActive(false);
      m_selectedIndex = index;
      m_tabs[m_selectedIndex].SetActive(true);
   }

   public void Redraw() {
      for (int i = 0 ; i < m_tabs.Count ; i++) {
          m_tabs[i].SetRenderIndex(i);
      }
   }

   public List<Tab> GetTabs() {
      return m_tabs;
   }

   public Tab GetSelectedTab() {
      return m_tabs[m_selectedIndex];
   }


   public override void OnPointerExit(PointerEventData eventData) {
      base.OnPointerExit(eventData);
      Redraw();
   }

}

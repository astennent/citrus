using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class CaptionBar : DragTarget {

   public NestedPanel panel;
   private List<Tab> m_tabs = new List<Tab>();

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
      int index = (int)(mousePosition.x / Tab.width);
      return Mathf.Clamp(index, 0, m_tabs.Count);
   } 

   public void AddTab(string tabName) {
      Tab tab = Tab.Instantiate(tabName, this);
      AddTab(tab);
      SelectTab(tab);
   }

   public void AddTab(Tab tab, int insertionIndex = -1) {
      tab.SetCaptionBar(this);
      if (m_tabs.Contains(tab)) {
         return;
      }

      if (insertionIndex == -1) {
         insertionIndex = m_tabs.Count;
      }

      m_tabs.Insert(insertionIndex, tab);
      Redraw();
      tab.FinishAnimation();
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
         tab.SetRenderIndex(i);
      }
   }

   public List<Tab> GetTabs() {
      return m_tabs;
   }


   public override void OnPointerExit(PointerEventData eventData) {
      base.OnPointerExit(eventData);
      if (DragManager.IsDragging()) {
         Redraw();
      }
   }

}

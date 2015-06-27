using UnityEngine;
using System.Collections.Generic;

public class InspectorController : Controller {
   private static float ROW_HEIGHT = 20;
   private static float ROW_PADDING = 2;

   public SimpleLabel labelPrefab;
   public RectTransform scrollingContent;
   private List<InspectorRow> m_rows = new List<InspectorRow>();

   private Node m_currentNode;
   private Node m_nodeToSet;

   class InspectorRow {
      RectTransform left;
      RectTransform right;
      int index;

      public InspectorRow(RectTransform _left, RectTransform _right, int _index) {
         left = _left;
         right = _right;
         index = _index;
      }

      public void Redraw(float width) {
         float ratio = 0.5f; // TODO: Make this draggable.
         float inverse = 1f - ratio;
         float padding = DPIScaler.ScaleFrom96(ROW_PADDING);
         float height = DPIScaler.ScaleFrom96(ROW_HEIGHT);

         left.sizeDelta = new Vector2(width*ratio - padding, height - padding);
         right.sizeDelta = new Vector2(width*inverse - 2*padding, height - padding);

         float top = (-index-1)*height - padding;
         left.anchoredPosition = new Vector2(padding, top);
         right.anchoredPosition = new Vector2(width*ratio + padding, top);
      }

      public void Destroy() {
         GameObject.Destroy(left.gameObject);
         GameObject.Destroy(right.gameObject);
      }
   }

   public static InspectorController Instantiate() {
      InspectorController controller = (InspectorController)GameObject.Instantiate(
               ControllerPrefabs.Inspector, Vector3.zero, new Quaternion(0,0,0,0));

      SelectionManager.SubscribeToNodeSelection(controller.SetInspectedNode);
      return controller;
   }

   public override string GetName() {
      return "Inspector";
   }

   void Update() {
      if (m_currentNode != m_nodeToSet) {
         UpdateNode(m_nodeToSet);
      }
   }

   public override void OnDisplayStart(ClientArea clientArea) {
      base.OnDisplayStart(clientArea);
      SetInspectedNode(SelectionManager.lastSelectedNode);
   }

   public override void OnDisplayEnd() {
      ClearRows();
      base.OnDisplayEnd();
   }

   public override void OnSize() {
      base.OnSize();
      foreach (InspectorRow row in m_rows) {
         row.Redraw(GetBounds().width);
      }
   }

   private void ClearRows() {
      foreach(InspectorRow row in m_rows) {
         row.Destroy();
      }
      m_rows.Clear();
   }

   public void SetInspectedNode(Node node) {
      m_nodeToSet = node;
   }

   /**
    * This is called by Update instead of directly for a couple reasons:
    * 1) Rebuilding all the rows is expensive. Calling it multiple times in a row, like when boxing,
    *    would cause the application to freeze for several seconds, and even crash.
    * 2) Calls from a separate thread cannot construct and size InspectorRow objects. 
    */
   private void UpdateNode(Node node) {
      m_currentNode = node;

      ClearRows();
      if (!node) {
         return; 
      }

      Row row = node.row;
      Attribute[] attributes = row.table.attributes;
      int numRows = attributes.Length;
      scrollingContent.sizeDelta = new Vector2(0, DPIScaler.ScaleFrom96(ROW_HEIGHT)*(numRows+1) + ROW_PADDING);

      for (int i = 0 ; i < attributes.Length ; i++) {
         AddInspectorRow(attributes[i], row[i], i);
      }
   }

   private void AddInspectorRow(Attribute attribute, string attributeValue, int index) {
      Rect clientRect = m_clientArea.GetComponent<RectTransform>().rect;

      SimpleLabel leftSide = SimpleLabel.Instantiate(labelPrefab, scrollingContent.transform);
      SimpleLabel rightSide = SimpleLabel.Instantiate(labelPrefab, scrollingContent.transform);

      InspectorRow row = new InspectorRow(leftSide.rectTransform, rightSide.rectTransform, index);
      row.Redraw(clientRect.width);
      m_rows.Add(row);

      leftSide.text = attribute.name;
      rightSide.text = attributeValue;
   }
}
using UnityEngine;
using System.Collections.Generic;

public class SelectionManager : MonoBehaviour {

   /**
    * Fired on Node selection
    */
   private static event NodeSelectedHandler NodeSelected;
   public delegate void NodeSelectedHandler(Node node);
   public static void SubscribeToNodeSelection(NodeSelectedHandler handler) {
      NodeSelected += handler;
   }


   HashSet<Node> selectedNodes = new HashSet<Node>();
   public static Node lastSelectedNode {get; private set;}

   public RectTransform selectionRect;
   private Vector2 m_selectionStartPosition;

   private static float m_lastClickTime; // Used for tracking doubleclick
   private static float DOUBLE_CLICK_TOLERANCE = 0.5f; //seconds

   Node m_draggingNode;
   float m_dragDistanceFromCamera = 0;

   private static SelectionManager s_instance;

   void Start () {
      s_instance = this;
   }

   void Update() {
      if (Input.GetMouseButtonUp(0)) {
         StopDragging();
         StopBoxing();
      }

      ProcessDragging();
   }   

   void LateUpdate() {
      ProcessBoxing();
   }

   public static void HandleClick() {
      Camera focusedCamera = GraphiteCamera.focusedCamera;
      if (!focusedCamera) {
         return;
      }


      Ray ray = focusedCamera.ScreenPointToRay(Input.mousePosition);
      RaycastHit hitInfo;
      if (Physics.Raycast(ray.origin, ray.direction, out hitInfo, 2000f)) {
         Node hitNode = hitInfo.collider.gameObject.GetComponent<Node>();
         bool doubleClicked = (Time.time - m_lastClickTime < DOUBLE_CLICK_TOLERANCE && 
                              lastSelectedNode == hitNode);
         s_instance.ClickNode(hitNode, doubleClicked);
         s_instance.StartDragging(hitNode);
      } else {
         s_instance.ClickNowhere();
         s_instance.StartBoxing();
      }

      m_lastClickTime = Time.time;
   }

   public void ClickNode(Node node, bool doubleClicked) {
      bool ctrl = Input.GetButton("Ctrl");

      if (doubleClicked) {
         HashSet<Node> connectedNodes = ClusterManager.GetConnectedNodes(node);
         foreach (Node connectedNode in connectedNodes) {
            SelectNode(connectedNode);
         }
         SelectNode(node); // This should be the last selected.
         return;
      }

      if (ctrl) {
         if (selectedNodes.Contains(node)) {
            UnselectNode(node);
         } else {
            SelectNode(node);
         }
      } else {
         if (!selectedNodes.Contains(node)) {
            ClearSelection();
            SelectNode(node);
         } 
      }
   }

   public void ClickNowhere() {
      bool ctrl = Input.GetButton("Ctrl");
      if (!ctrl) {
         ClearSelection();
      }
   }

   private void ClearSelection() {
      foreach (Node node in selectedNodes) {
         node.isSelected = false;
      }
      selectedNodes.Clear();
      NodeSelected(null);
   }

   private void SelectNode(Node node) {
      selectedNodes.Add(node);
      lastSelectedNode = node;
      node.isSelected = true;
      NodeSelected(node);
   }

   private void UnselectNode(Node node) {
      selectedNodes.Remove(node);
      node.isSelected = false;
   }

   private void ProcessDragging() {
      Camera camera = GraphiteCamera.focusedCamera;
      if (m_draggingNode == null || camera == null) {
         return;
      }

      Vector2 mouseCoords = Input.mousePosition;
      Vector3 desiredPosition = camera.ScreenToWorldPoint(new Vector3(mouseCoords.x, mouseCoords.y, m_dragDistanceFromCamera));
      Vector3 currentPosition = m_draggingNode.transform.position;
      Vector3 transitionPosition = Vector3.Lerp(currentPosition, desiredPosition, .5f);

      Vector3 positionDelta = transitionPosition - currentPosition; 

      foreach (Node node in selectedNodes) {
         Vector3 draggedPosition = node.transform.position + positionDelta;
         NodeMover.SetPosition(node, draggedPosition);
         node.transform.position = draggedPosition; // Operate immediately.
      }
   }

   private void StartDragging(Node node) {
      Camera camera = GraphiteCamera.focusedCamera;
      if (camera != null) {
         m_draggingNode = node;
         node.isDragging = true;
         Vector3 heading = node.transform.position - camera.transform.position;
         m_dragDistanceFromCamera = Vector3.Dot(heading, camera.transform.forward);
      }
   }

   private void StopDragging() {
      if (m_draggingNode != null) {
         m_draggingNode.isDragging = false;
         m_draggingNode = null;
      }
   }

   private void StartBoxing() {
      selectionRect.gameObject.SetActive(true);
      m_selectionStartPosition = Input.mousePosition;
   }

   private void ProcessBoxing() {
      if (selectionRect.gameObject.activeSelf) {
         Vector2 mousePosition = Input.mousePosition;
         Vector2 startPosition = m_selectionStartPosition;
         float left = Mathf.Min(mousePosition.x, startPosition.x);
         float right = Mathf.Max(mousePosition.x, startPosition.x);
         float top = Mathf.Min(mousePosition.y, startPosition.y);
         float bottom = Mathf.Max(mousePosition.y, startPosition.y);
         selectionRect.position = new Vector2(left, top);
         selectionRect.sizeDelta = new Vector2(right-left, bottom-top);
      }
   }

   private bool IsNodeInSelectionBox(Node node) {
      if (node.isLinking()) {
         return false;
      }

      Camera camera = GraphiteCamera.focusedCamera;
      if (camera == null) {
         return false;
      }

      Vector3 point = camera.WorldToScreenPoint(node.transform.position);
      if (point.z <= 0) {
         return false; // Node is behind the camera.
      }

      Rect rect = selectionRect.rect;
      Vector3 rectPoint = selectionRect.transform.position;
      return (point.x > rectPoint.x && point.x < rectPoint.x + rect.width &&
              point.y > rectPoint.y && point.y < rectPoint.y + rect.height);
   }

   private void StopBoxing() {
      selectionRect.gameObject.SetActive(false);
      List<Node> boxedNodes = NodeManager.Filter(IsNodeInSelectionBox);
      foreach (Node node in boxedNodes) {
         SelectNode(node);
      }
   }


}
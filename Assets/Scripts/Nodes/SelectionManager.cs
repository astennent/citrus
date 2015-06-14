using UnityEngine;
using System.Collections.Generic;

public class SelectionManager : MonoBehaviour {

   HashSet<Node> selectedNodes = new HashSet<Node>();

   Node m_draggingNode;
   float m_dragDistanceFromCamera = 0;
   // This is Late so that Panel selection can switch before processing the click.
   void LateUpdate() {
      if (Input.GetMouseButtonDown(0)) {

         Camera focusedCamera = CitrusCamera.focusedCamera;
         if (!focusedCamera) {
            Utils.Log("no camera");
            return;
         }

         Ray ray = focusedCamera.ScreenPointToRay(Input.mousePosition);
         RaycastHit hitInfo;
         if (Physics.Raycast(ray.origin, ray.direction, out hitInfo, 2000f)) {
            Node hitNode = hitInfo.collider.gameObject.GetComponent<Node>();
            ClickNode(hitNode);
            StartDragging(hitNode);
         } else {
            ClickNowhere();
         }
      }

      if (Input.GetMouseButtonUp(0)) {
         StopDragging();
      }

      ProcessDragging();
   }   

   public void ClickNode(Node node) {
      bool ctrl = Input.GetButton("Ctrl");

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
   }

   private void SelectNode(Node node) {
      selectedNodes.Add(node);
      node.isSelected = true;
   }

   private void UnselectNode(Node node) {
      selectedNodes.Remove(node);
      node.isSelected = false;
   }

   private void ProcessDragging() {
      Camera camera = CitrusCamera.focusedCamera;
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
         node.SetPosition(draggedPosition);
         node.transform.position = draggedPosition; // Operate immediately.
      }
   }

   private void StartDragging(Node node) {
      Camera camera = CitrusCamera.focusedCamera;
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


}
using UnityEngine;
using System.Collections.Generic;

public class SelectionManager : MonoBehaviour {

   HashSet<Node> selectedNodes = new HashSet<Node>();

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
         } else {
            ClickNowhere();
         }
      }
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
         ClearSelection(node);
         if (!selectedNodes.Contains(node)) {
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

   private void ClearSelection(Node exceptedNode = null) {
      foreach (Node node in selectedNodes) {
         if (node != exceptedNode) {
            node.isSelected = false;
         }
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


}
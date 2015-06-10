using UnityEngine;
using System.Collections.Generic;

public class SelectionManager : MonoBehaviour {

   HashSet<Node> selectedNodes = new HashSet<Node>();

   void Update() {
      if (Input.GetMouseButtonDown(0)) {
         Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
         RaycastHit hitInfo;
         if (Physics.Raycast(ray.origin, ray.direction, out hitInfo, 2000f)) {
            Node hitNode = hitInfo.collider.gameObject.GetComponent<Node>();
            ClickNode(hitNode);
         }
      }
   }   

   public void ClickNode(Node node) {
      if (selectedNodes.Contains(node)) {
         UnselectNode(node);
      } else {
         SelectNode(node);
      }
   }

   public void ClickNowhere() {
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


}
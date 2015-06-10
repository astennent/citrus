using UnityEngine;

class NodeClickHandler : MonoBehaviour {
   
   void Update() {
      if (Input.GetMouseButtonDown(0)) {
         Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
         RaycastHit hitInfo;
         if (Physics.Raycast(ray.origin, ray.direction, out hitInfo, 2000f)) {
            Node hitNode = hitInfo.collider.gameObject.GetComponent<Node>();
            hitNode.OnClick();
         }
      }
   }   

}
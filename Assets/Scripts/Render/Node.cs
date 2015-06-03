using UnityEngine;

public class Node : MonoBehaviour {

   public Row row {get; private set;}

   public static Node Instantiate(Node prefab, Row _row) {
      Node node = (Node)GameObject.Instantiate(prefab, Vector3.zero, new Quaternion(0,0,0,0));
      node.row = _row;
      node.JumpToRandomPosition();
      _row.SetNode(node);
      return node;
   }

   public void JumpToRandomPosition() {
      transform.position = new Vector3(Random.value, Random.value, Random.value) * 2000 - Vector3.one*1000;
   }
}
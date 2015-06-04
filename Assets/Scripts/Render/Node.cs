using UnityEngine;

public class Node : MonoBehaviour {

   public Row row {get; private set;}

   public static Node Instantiate(Node prefab, Row _row) {
      Node Node = (Node)GameObject.Instantiate(prefab, Vector3.zero, new Quaternion(0,0,0,0));
      Node.row = _row;
      Node.JumpToRandomPosition();
      return Node;
   }

   public void JumpToRandomPosition() {
      transform.position = new Vector3(Random.value, Random.value, Random.value) * 2000 - Vector3.one*1000;
   }
}
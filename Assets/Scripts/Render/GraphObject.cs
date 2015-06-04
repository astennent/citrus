using UnityEngine;

public class GraphObject : MonoBehaviour {

   public Row row {get; private set;}

   public static GraphObject Instantiate(GraphObject prefab, Row _row) {
      GraphObject GraphObject = (GraphObject)GameObject.Instantiate(prefab, Vector3.zero, new Quaternion(0,0,0,0));
      GraphObject.row = _row;
      GraphObject.JumpToRandomPosition();
      return GraphObject;
   }

   public void JumpToRandomPosition() {
      transform.position = new Vector3(Random.value, Random.value, Random.value) * 2000 - Vector3.one*1000;
   }
}
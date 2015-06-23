using UnityEngine;

// TODO: If this becomes useful elsewhere, don't make it so intwined with Inspector.
public class SimpleLabel : MonoBehaviour {
   public UnityEngine.UI.Text labelText;

   public string text {
      get {return labelText.text; }
      set {labelText.text = value; }
   }

   //public rectTransform
   public RectTransform rectTransform {
      get {return GetComponent<RectTransform>(); }
      set {}
   }
   public static SimpleLabel Instantiate(SimpleLabel prefab, Transform parent) {
      SimpleLabel label = (SimpleLabel)GameObject.Instantiate(
            prefab, Vector3.zero, new Quaternion(0,0,0,0));
      label.transform.SetParent(parent);
      return label;
   }
}
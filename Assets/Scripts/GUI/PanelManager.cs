using UnityEngine;
using System.Collections;

public class PanelManager : MonoBehaviour {

   public NestedPanel nestedPanelPrefab;
   
   static PanelManager s_instance;

   // Use this for initialization
   void Start () {
      s_instance = this;
   }

   public static NestedPanel GetNestedPanelPrefab() {
      return s_instance.nestedPanelPrefab;
   }

   
}

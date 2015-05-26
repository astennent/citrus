using UnityEngine;
using System.Collections;

public class PanelManager : MonoBehaviour {

   public NestedPanel nestedPanelPrefab;
   public Tab tabPrefab;

   public Texture2D leftRightArrow;
   public Texture2D upDownArrow;

   static PanelManager s_instance;

   // Use this for initialization
   void Start () {
      s_instance = this;
   }

   public static NestedPanel GetNestedPanelPrefab() {
      return s_instance.nestedPanelPrefab;
   }

   public static Tab GetTabPrefab() {
      return s_instance.tabPrefab;
   }

   public static Texture2D GetLeftRightArrow() {
      return s_instance.leftRightArrow;
   }

   public static Texture2D GetUpDownArrow() {
      return s_instance.upDownArrow;
   }
   
}

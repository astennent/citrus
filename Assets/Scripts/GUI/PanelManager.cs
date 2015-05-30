using UnityEngine;
using System.Collections;

public class PanelManager : MonoBehaviour {

   public NestedPanel nestedPanelPrefab;
   public Tab tabPrefab;

   public NestedPanel root;
   public UnityEngine.Canvas topCanvas;
   public UnityEngine.Canvas bottomCanvas;

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

   public static NestedPanel GetRoot() {
      return s_instance.root;
   }

   /**
    * Returns a canvas that can be used to render elements so that they are always on top.
    * This must be used when doing UI rendering that crosses between panels to ensure a consistent
    * z-order. It should not be used to render things inside of a NestedPanel.
    */
   public static Canvas GetTopCanvas() {
      return s_instance.topCanvas;
   }
   public static Canvas GetBottomCanvas() {
      return s_instance.bottomCanvas;
   }

   private static DragTarget s_dragTarget;
   public static DragTarget GetDragTarget() {
      return s_dragTarget;
   }
   public static void NotifyDragTarget(DragTarget target) {
      s_dragTarget = target;
   }



}

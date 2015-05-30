using UnityEngine;
using System.Collections;

public class PanelManager : MonoBehaviour {

   public NestedPanel nestedPanelPrefab;
   public Tab tabPrefab;

   public NestedPanel root;
   public UnityEngine.Canvas mainCanvas;
   public UnityEngine.Canvas topCanvas;
   public UnityEngine.Canvas bottomCanvas;

   private static PanelManager s_instance;

   // Use this for initialization
   void Start () {
      s_instance = this;

      // Set up tabs
      GetRoot().AddTab("Inspector");
      GetRoot().AddTab("Camera");
      GetRoot().AddTab("Workspace");
      GetRoot().AddTab("Console");
      GetRoot().AddTab("Data");
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

   public static void SetRoot(NestedPanel newRoot) {
      s_instance.root = newRoot;
      newRoot.transform.SetParent(s_instance.mainCanvas.transform);
   }

}

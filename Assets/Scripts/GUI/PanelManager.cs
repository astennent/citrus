using UnityEngine;
using System.Collections;

public class PanelManager : MonoBehaviour {

   public NestedPanel nestedPanelPrefab;
   public Tab tabPrefab;

   NestedPanel _root = null;
   public static NestedPanel root {
      get { 
         return s_instance._root;
      } 
      set { 
         s_instance._root = value;
         value.transform.SetParent(s_instance.mainCanvas.transform);
      }
   }

   public UnityEngine.Canvas mainCanvas;
   public UnityEngine.Canvas topCanvas;
   public UnityEngine.Canvas bottomCanvas;

   private static PanelManager s_instance;

   // Use this for initialization
   void Start () {
      s_instance = this;
      root = GameObject.FindGameObjectWithTag("RootPanel").GetComponent<NestedPanel>();

      // Set up tabs
      root.AddTab(DummyController.Instantiate());
      root.AddTab(DummyController.Instantiate());
      root.AddTab(DummyController.Instantiate());
      root.AddTab(CameraController.Instantiate());
      root.AddTab(InspectorController.Instantiate());
      root.AddTab(CameraController.Instantiate());
   }

   public static NestedPanel GetNestedPanelPrefab() {
      return s_instance.nestedPanelPrefab;
   }

   public static Tab GetTabPrefab() {
      return s_instance.tabPrefab;
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

}

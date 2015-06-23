using UnityEngine;

public class ControllerPrefabs : MonoBehaviour {

   public DummyController _dummy;
   public static DummyController Dummy {
      get { return s_instance._dummy; }
      private set {}
   }

   public CameraController _camera;
   public static CameraController Camera {
      get { return s_instance._camera; }
      private set {}
   }

   public InspectorController _inspector;
   public static InspectorController Inspector {
      get { return s_instance._inspector; }
      private set {}
   }

   private static ControllerPrefabs s_instance;
   void Start() {
      s_instance = this;
   }
}
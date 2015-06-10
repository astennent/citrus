using UnityEngine;

public class CitrusCamera : MonoBehaviour 
{
   float x;
   float y;
   float shiftVelocity;

   private float FREE_ROTATE_SPEED = 8f;
   private float FREE_MOVEMENT_SPEED = 5f;

   public static Camera focusedCamera;

   void Update()
   {

      if (GetComponent<Camera>() != focusedCamera) {
         return;
      }

      // Rotation
      if (Input.GetMouseButton(1)) {
         x = Input.GetAxis("Mouse X") * FREE_ROTATE_SPEED;
         y = Input.GetAxis("Mouse Y") * FREE_ROTATE_SPEED;
         
         transform.RotateAround(transform.position, transform.right, -y);
         transform.RotateAround(transform.position, Vector3.up, x);
      }

      if (Input.GetButton("Shift")) {
         shiftVelocity = Mathf.Max(shiftVelocity + .025f, 1.5f);
      }
      else {
         shiftVelocity = Mathf.Max(shiftVelocity/2, 1);
      }

      //Movement
      float forward = Input.GetAxis("Vertical") * FREE_MOVEMENT_SPEED * shiftVelocity;
      float strafe = Input.GetAxis("Horizontal") * FREE_MOVEMENT_SPEED * shiftVelocity;
      float strafeVertical = Input.GetAxis("StrafeVertical") * FREE_MOVEMENT_SPEED * shiftVelocity;

      transform.position += transform.forward * forward;
      transform.position += transform.right * strafe;
      transform.position += transform.up * strafeVertical;
   }



}
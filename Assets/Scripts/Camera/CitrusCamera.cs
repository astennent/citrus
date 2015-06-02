using UnityEngine;

public class CitrusCamera : MonoBehaviour 
{

   float x;
   float y;

   private float FREE_ROTATE_SPEED = 8f;
   private float FREE_MOVEMENT_SPEED = 5f;

   void Update()
   {

      // Rotation
      if (Input.GetMouseButton(1)) {
         x = Input.GetAxis("Mouse X") * FREE_ROTATE_SPEED;
         y = Input.GetAxis("Mouse Y") * FREE_ROTATE_SPEED;
         
         transform.RotateAround(transform.position, transform.right, -y);
         transform.RotateAround(transform.position, Vector3.up, x);
      }

      //Movement
      float forward = Input.GetAxis("Vertical") * FREE_MOVEMENT_SPEED;
      float strafe = Input.GetAxis("Horizontal") * FREE_MOVEMENT_SPEED;
      float strafeVertical = Input.GetAxis("StrafeVertical") * FREE_MOVEMENT_SPEED;

      transform.position += transform.forward * forward;
      transform.position += transform.right * strafe;
      transform.position += transform.up * strafeVertical;

   }



}
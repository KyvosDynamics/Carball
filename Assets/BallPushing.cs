using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPushing : MonoBehaviour {

    float pushpower = 20f;



    private void OnCollisionEnter(Collision collision)
    {
        


    //}
//
  //  private void OnfControllerColliderHit(ControllerColliderHit hit)
   // {
        if (collision.gameObject.tag != "Ball")
          return;



        var body = collision.collider.attachedRigidbody;

        Vector3 collisionForce = collision.impulse / Time.fixedDeltaTime; //because in physics  impulse = force * time
        var forceWithoutY = new Vector3(collisionForce.x, 0, collisionForce.z);// hit.moveDirection.x, 0, hit.moveDirection.z);


//        body.velocity = vector * pushpower;

        body.AddForce(forceWithoutY*pushpower, ForceMode.Force);
    }



}

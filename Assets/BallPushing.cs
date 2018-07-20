using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPushing : MonoBehaviour {

    float pushpower = 2;


    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag != "Ball")
            return;



        var body = hit.collider.attachedRigidbody;


        var vector = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);


        body.velocity = vector * pushpower;
    }
}

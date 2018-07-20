using UnityEngine;

public class GolemMovement : MonoBehaviour
{
    public float CarSpeed = 1f;
    public float RotationSpeed = 1f;

    private Transform car;
    public Transform wheeltarget;
    public Transform backwardMovementRotationTarget;

    //private bool move = false;
    enum MoveState { None, Forward, Backward, ForwardNitro }
    MoveState moveState = MoveState.None;

    enum WheelState { Straight, Right, Left }
    WheelState wheelState = WheelState.Straight;





    private void Start()
    {
        car = transform.parent;

        backwardMovementRotationTarget.position = wheeltarget.position = transform.position + transform.forward * 4;






        // get the distance to ground

        distToGround = car.GetComponent<Collider>().bounds.extents.y;
    }


    float distToGround;



    bool IsGrounded()
    {
        return Physics.Raycast(car.position, -Vector3.up, distToGround + 0.1f);
    }




    private bool jump = false;


    private void FixedUpdate()
    {



        jump = Input.GetKey(KeyCode.Space);



        if (Input.GetKey(KeyCode.UpArrow))
            moveState = MoveState.Forward;
        else if (Input.GetKey(KeyCode.DownArrow))
            moveState = MoveState.Backward;
        else
            moveState = MoveState.None;// move = false;




        if (Input.GetKey(KeyCode.RightArrow))
            wheelState = WheelState.Right;// rotatewheelsright = true;
        else if (Input.GetKey(KeyCode.LeftArrow))
            wheelState = WheelState.Left;
        else
            wheelState = WheelState.Straight;//            rotatewheelsright = false;




        //        if (Input.GetKey(KeyCode.LeftArrow))
        //          rotatewheelsleft = true;
        //    else
        //      rotatewheelsleft = false;
    }





    private float rightmultforangle(float angle)
    {

        //we want to move the wheeltarget so that the wheel faces it
        //we know the angle between the car and the wheel, so
        /*
		 * 

tanangle=(transform.right*x)/(transform.forward*4)

(transform.right*x)=tanalge*(tranform.forward*4)

x=tanalge*(tranform.forward*4)/transform.right */




        return Mathf.Tan(Mathf.PI * angle / 180) * (transform.forward * 4).magnitude / transform.right.magnitude;
    }



    void UpdateCameraPosition()
    {
        Camera.main.transform.position = car.position - car.forward * 7 + new Vector3(0, 3, 0);
        //        Camera.main.transform.LookAt(car.forward);



        Vector3 newRotation = Camera.main.transform.eulerAngles;

        newRotation.y = car.eulerAngles.y;
        newRotation.x = 0;
        newRotation.z = 0;

        Camera.main.transform.eulerAngles = newRotation;

    }



    void Update()
    {
        Vector3 wheelToWheelTargetVector = wheeltarget.position - transform.position;
        float angleBetweenCarAndWheel = Vector3.SignedAngle(car.forward, wheelToWheelTargetVector, Vector3.up);







        if (jump == true && IsGrounded())
        {

            car.GetComponent<Rigidbody>().AddForce(new Vector3(0, 3, 0), ForceMode.Impulse);
        }



        if (moveState == MoveState.Forward || moveState == MoveState.Backward)
        {

            //we want the "forward" speed of the car to be smaller when it is turning
            float speed = wheelState == WheelState.Straight ? CarSpeed : CarSpeed / 2;

            if (moveState == MoveState.Backward)
                speed *= -1;


            car.position += transform.forward * speed * Time.deltaTime;




            //we want to rotate the car to face the wheeltarget, but not in one step
            //and when moving backwards we want the car to face the backwardrotationtarget
            wheeltarget.position = new Vector3(wheeltarget.position.x, car.position.y, wheeltarget.position.z);
            backwardMovementRotationTarget.position = new Vector3(backwardMovementRotationTarget.position.x, car.position.y, backwardMovementRotationTarget.position.z);

            Quaternion desiredRotation = new Quaternion();

            if (moveState == MoveState.Forward)
                desiredRotation = Quaternion.LookRotation(wheeltarget.position - car.position);
            else if (moveState == MoveState.Backward)
                desiredRotation = Quaternion.LookRotation(backwardMovementRotationTarget.position - car.position);

            car.rotation = Quaternion.Slerp(car.rotation, desiredRotation, Time.deltaTime * Mathf.Abs(angleBetweenCarAndWheel) * RotationSpeed);





            //rotating the car also rotates the wheel. So it no longer faces the wheel target.
            //we want to move the wheeltarget to be "in front of the wheel"


            wheeltarget.position = transform.position + transform.forward * 4;




        }









        //to rotate the wheels we move the wheeltarget and make the wheels look at that target

        if (wheelState == WheelState.Right)
        {

            if (angleBetweenCarAndWheel < 30)
            {
                float rightmult = rightmultforangle(30 - angleBetweenCarAndWheel); //so that if the angle is 0 we shift by 30, if 20 by 10

                wheeltarget.position = transform.position + transform.forward * 4 + transform.right * rightmult;



                transform.LookAt(wheeltarget);
            }
            else
            {
                Vector3 vector = Quaternion.Euler(0, -60, 0) * transform.forward * 4;

                backwardMovementRotationTarget.position = transform.position + vector;// car.forward * 4 + car.right * rightmultforangle2(30);
                int ksdjf = 34;
            }

        }
        else if (wheelState == WheelState.Left)
        {
            if (angleBetweenCarAndWheel > -30)
            {
                wheeltarget.position = transform.position + transform.forward * 4 - transform.right * rightmultforangle(30 + angleBetweenCarAndWheel); //we use positive angles here too but negative sign
                transform.LookAt(wheeltarget);
            }
            else
            {
                Vector3 vector = Quaternion.Euler(0, 60, 0) * transform.forward * 4;

                backwardMovementRotationTarget.position = transform.position + vector;


            }
        }
        else
        {//restore wheels


            if (angleBetweenCarAndWheel > 0)
            {//rotate counterclockwise

                float rightmult = rightmultforangle(angleBetweenCarAndWheel);
                backwardMovementRotationTarget.position = wheeltarget.position = transform.position + transform.forward * 4 - transform.right * rightmult;


                transform.LookAt(wheeltarget);
            }
            else if (angleBetweenCarAndWheel < 0)
            {//rotate clockwise
                backwardMovementRotationTarget.position = wheeltarget.position = transform.position + transform.forward * 4 + transform.right * rightmultforangle(-angleBetweenCarAndWheel);
                transform.LookAt(wheeltarget);
            }

        }








        UpdateCameraPosition();
    }





}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Controls behavor of platforms that move
//Attached to Moving Platform parent object of platform
public class MovingPlatform : MonoBehaviour
{
    //The platform's destination
    [Header("Destination Coordinates")]
    public float x;
    public float y;
    public float z;

    //The speed of the platform
    [Range(0, 1)]
    public float speed = 1;

    //Indicates if the platform is moving upwards
    public bool goingUp;

    //The original position and destination of the platform
    private Vector3 origin;
    private Vector3 destination;

    //Timer values for the platform waiting
    private float timer;
    private float goTime;

    //The previous Y coordinate of the platform
    private float lastY;

    void Start()
    {
        //Initialize original position and destination
        origin = transform.position;
        destination = new Vector3(x, y, z);

        //Initialize timer values
        timer = 0;
        goTime = -1f;

        //Initialize Y coordinate and up indication
        lastY = origin.y;
        goingUp = false;
    }

    void FixedUpdate()
    {
        //If it is not time to move, stop
        if (Time.time < goTime) return;

        //Increment the timer
        timer += Time.deltaTime;

        //Calculate position of platform
        float pingPong = Mathf.PingPong(timer * speed, 1);
        transform.position = Vector3.Lerp(origin, destination, pingPong);

        //If the current Y is greater than the previous,
        if (transform.position.y > lastY)
        {
            //Indicate the platform is moving up
            goingUp = true;
        }
        //If the current Y is less than or equal to the previous,
        else
        {
            //Indicate the platform is not moving up
            goingUp = false;
        }

        //If the value is within a certain distance to the destination,
        if (pingPong > 0.9985 || pingPong < 0.0015)
        {
            //Make the platform stop and wait
            goTime = Time.time + 3f;
            goingUp = false;
        }

        //Update the previous Y coordinate
        lastY = transform.position.y;
    }
}

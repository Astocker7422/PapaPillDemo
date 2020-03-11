using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Controls camera that follows the player
//Attached to the main camera
public class CameraFollow : MonoBehaviour
{
    //The player the camera is following
    public GameObject player;

    //Distance from the player
    private float distance;

    //Camera movement speed
    private float speed;

    //Camera angles
    private float y;
    private float x;
    
    //The previous transform values of the camera
    private Vector3 lastPosition;
    private Quaternion lastRotation;

    void Start ()
    {
        //Initialize distance from player and movement speed
        distance = 7.21f;
        speed = 50.0f;

        //Initialize angles
        Vector3 angles = transform.eulerAngles;
        y = angles.y;
        x = angles.x;

        //Initialize transform values
        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }
	
	void LateUpdate ()
    {
        //If the player is not paused
        if (!player.GetComponent<Player>().isPaused)
        {
            //Obtain the mouse input and scale
            y += Input.GetAxis("Mouse X") * distance;
            x = Mathf.Clamp(x += -(Input.GetAxis("Mouse Y") * distance), 0, 65);
            
            //Obtain the rotation of the camera and restrict the movement
            var rotation = Quaternion.Euler(x, y, 0);

            //Calculate the position of the camera based on the rotation and distance
            var position = rotation * new Vector3(0.0f, 0.0f, -distance) + player.transform.position;

            //Apply transformation
            transform.rotation = rotation;
            transform.position = position;

            //Cast ray to check for objects
            RaycastHit hit = new RaycastHit();
            Ray wallRay = new Ray(player.transform.position, transform.position - player.transform.position);

            //If the ray hit an object,
            if (Physics.Raycast(wallRay, out hit, distance))
            {
                //If the object was not a coin or the player's weapon, 
                if (!hit.collider.gameObject.CompareTag("Coin") && !hit.collider.gameObject.CompareTag("Weapon"))
                {
                    //If the object was not an enemy or an enemy's weapon object,
                    if (!hit.collider.gameObject.CompareTag("Enemy") && !hit.collider.gameObject.CompareTag("Enemy Weapon"))
                    {
                        //Move the camera to the point the ray hit
                        transform.position = hit.point;
                    }
                }
            }

            //Update previous transform values
            lastPosition = transform.position;
            lastRotation = transform.rotation;
        }
        //If the player is paused,
        else
        {
            //Do not move the camera
            transform.position = lastPosition;
            transform.rotation = lastRotation;
        }
    }
}

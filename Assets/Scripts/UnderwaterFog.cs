using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Controls the underwater effects
//Attached to each water tile
public class UnderwaterFog : MonoBehaviour
{
    //The level music
    public GameObject music;

    //The normal level color and the underwater fog color
    private Color normalColor;
    private Color underwaterColor;

    //The player
    private Player player;

    //The player's normal jump speed and underwater jump speed
    private float jumpSpeed;
    private float swimSpeed;

	void Start ()
    {
        //Initialize fog colors
        normalColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        underwaterColor = new Color(0.2f, 0.65f, 0.7f, 0.5f);

        //Initialize player
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        //Initialize jump values
        jumpSpeed = player.jumpSpeed;
        swimSpeed = jumpSpeed / 1.5f;
    }

    void OnTriggerEnter(Collider other)
    {
        //If the main camera enter's the water trigger,
        if (other.gameObject.CompareTag("MainCamera"))
        {
            //Decrease the gravity and change the player's jump speed
            Physics.gravity = new Vector3(0, -9f, 0);
            player.jumpSpeed = swimSpeed;

            //Activate the underwater fog
            RenderSettings.fogColor = underwaterColor;
            RenderSettings.fogDensity = 0.03f;
            RenderSettings.fog = true;

            //Muffle music
            music.GetComponent<AudioLowPassFilter>().cutoffFrequency = 500;
        }
    }

    void OnTriggerExit(Collider other)
    {

        //If the main camera exits the water trigger,
        if (other.gameObject.CompareTag("MainCamera"))
        {
            //If the camera is now above the water,
            if (transform.position.y < other.transform.position.y)
            {
                //Set gravity and player jump speed back to normal
                Physics.gravity = new Vector3(0, -20f, 0);
                player.jumpSpeed = jumpSpeed;

                //Deactivate the underwater fog
                RenderSettings.fogColor = normalColor;
                RenderSettings.fogDensity = 0.002f;
                RenderSettings.fog = false;

                //Un-muffle the music
                music.GetComponent<AudioLowPassFilter>().cutoffFrequency = 5000;
            }
        }
    }
}

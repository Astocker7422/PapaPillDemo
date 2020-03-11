using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Controls behavior relating to the boss area
//Attached to pillar housing boss fight
public class BossPillar : MonoBehaviour
{
    //The player object
    public Player player;

    //The platforms up the pillar and the camera that shows them
    public GameObject platforms;
    public GameObject panningCam;

    //The walls activated during boss fight
    public GameObject bossWalls;

    //The normal level music and boss music objects
    public GameObject levelMusic;
    public GameObject bossMusic;

    //The boss object and its health bar
    public GameObject boss;
    public Slider bossHealth;

    //Indication of the platforms activated
    private bool activated;

    //Timer controlling the camera's active state
    private float animTimer;
    private float animStopTime;

    //The level music
    public GameObject music;

    //The normal level color and the underwater fog color
    private Color normalColor;
    private Color underwaterColor;

    void Start()
    {
        //Initialize the platforms as inactive
        activated = false;

        //initialize the timer values
        animTimer = 0;
        animStopTime = 10f;

        //Initialize fog colors
        normalColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        underwaterColor = new Color(0.2f, 0.65f, 0.7f, 0.5f);
    }

	void Update ()
    {
        //If the player has reached the coin goal,
		if(player.count == player.collectGoal)
        {
            //If the platforms have not been activated,
            if (!activated)
            {
                if(player.isSwimming)
                {
                    //Deactivate the underwater fog
                    RenderSettings.fogColor = normalColor;
                    RenderSettings.fogDensity = 0.002f;
                    RenderSettings.fog = false;

                    //Un-muffle the music
                    music.GetComponent<AudioLowPassFilter>().cutoffFrequency = 5000;
                }

                //Indicate a cutscene is playing
                player.isCutscene = true;

                //Activate platforms, trigger camera animation, and indicate platforms activated
                ActivatePlatforms();
                activated = true;

                //Pause the player
                player.isPaused = true;
            }

            //Increment the camera's animation timer
            animTimer += Time.deltaTime;
        }

        //If the camera's timer has reached the threshold,
        if(animTimer >= animStopTime)
        {
            if (player.isSwimming)
            {
                //Activate the underwater fog
                RenderSettings.fogColor = underwaterColor;
                RenderSettings.fogDensity = 0.03f;
                RenderSettings.fog = true;

                //Muffle music
                music.GetComponent<AudioLowPassFilter>().cutoffFrequency = 500;
            }

            //Indicate a cutscene is not playing
            player.isCutscene = false;

            //Deactivate the cutscene camera and unpause the player
            panningCam.SetActive(false);
            player.isPaused = false;
        }
        
        //If the player is dead,
        if(player.healthBar.value == 0)
        {
            //Deactivate the boss fight walls
            bossWalls.SetActive(false);

            //Change the music back to the level music
            levelMusic.GetComponent<AudioSource>().enabled = true;
            bossMusic.GetComponent<AudioSource>().enabled = false;

            //Stop the boss behavior
            boss.GetComponent<Boss>().enabled = false;

            //Deactivate the boss health bar
            bossHealth.gameObject.SetActive(false);

            //Reactivate the trigger at the top
            transform.GetComponent<BoxCollider>().enabled = true;

            //Reset boss values
            boss.GetComponent<Boss>().Reset();
        }
	}

    //Activates the platfoms to boss and calls camera animation
    void ActivatePlatforms()
    {
        //Activate platforms to boss
        platforms.SetActive(true);

        //Start camera animation
        ShowPlatforms();
    }

    //Activates camera to show activated platforms
    void ShowPlatforms()
    {
        //Activate the camera
        panningCam.SetActive(true);

        //Play the panning animation
        panningCam.GetComponent<Animator>().SetBool("Pan", true);
    }

    void OnTriggerEnter(Collider other)
    {
        //If the player enters the boss trigger,
        if(other.gameObject.CompareTag("Player"))
        {
            //Activate the boss fight walls
            bossWalls.SetActive(true);

            //Change the music to the boss music
            levelMusic.GetComponent<AudioSource>().enabled = false;
            bossMusic.GetComponent<AudioSource>().enabled = true;

            //Enable boss behavior
            boss.GetComponent<Boss>().enabled = true;

            //Activate the boss health bar
            bossHealth.gameObject.SetActive(true);

            //Deactivate the boss trigger
            transform.GetComponent<BoxCollider>().enabled = false;
        }
    }
}

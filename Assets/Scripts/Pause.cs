using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//Controls behavior of world when paused
//Attached to empty object
public class Pause : MonoBehaviour
{
    //The pause menu and its default button
    public GameObject PauseCanvas;
    private Button defaultButton;

    //The player
    public Player player;

    //The scene's EventSystem
    private EventSystem eventSystem;

    void Start()
    {
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();

        defaultButton = PauseCanvas.transform.FindDeepChild("Resume Button").GetComponent<Button>();
    }

    void Update()
    {
        //If pause is pressed,
        if (Input.GetButtonDown("Pause"))
        {
            //If the tutorial canvas and win canvas are not active,
            if (!GameObject.Find("Tutorial Canvas") && !GameObject.Find("Win Canvas"))
            {
                //Handle pausing behavior
                togglePause();
            }
        }
    }

    //Handles behavior when pause activated
    public void togglePause()
    {
        if (!GameObject.Find("Win Canvas") && !GameObject.Find("Start Canvas") && !GameObject.Find("Tutorial Canvas"))
        {
            //If the world is already paused,
            if (Time.timeScale == 0f)
            {
                //Deactivate the pause menu, unpause the player, and unpause the world
                player.Paused(false);
                PauseCanvas.SetActive(false);
                Time.timeScale = 1f;
            }
            //If the world is not paused,
            else
            {
                //Activate the pause menu
                PauseCanvas.SetActive(true);

                //Select the resume button
                defaultButton.Select();

                //Pause the player and the world
                player.Paused(true);
                Time.timeScale = 0f;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Controls which scene is loaded
//Attached to empty object
public class SceneLoader : MonoBehaviour
{
    //Load the demo scene
    public void LoadOpenLevel()
    {
        SceneManager.LoadScene("Open Level");
    }

    //Load the main menu scene
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    //Exit the game
    public void Quit()
    {
        Application.Quit();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handles player death when falling off the map
//Attached to death level trigger
public class Death : MonoBehaviour
{
    //The player and the proper respawn point set in inspector
    public GameObject player;
    public Vector3 respawn;

    //AudioSource to play death sound
    private AudioSource audioSource;

    void Start()
    {
        //Set audioSource
        audioSource = GetComponent<AudioSource>();
    }

	IEnumerator OnTriggerEnter(Collider other)
    {
        //If the player enters the trigger
        if (other.gameObject.CompareTag("Player"))
        {
            //Play the sound, wait, and respawn the player
            audioSource.Play();
            yield return new WaitForSeconds(2);
            player.transform.position = respawn;
        }
    }
}

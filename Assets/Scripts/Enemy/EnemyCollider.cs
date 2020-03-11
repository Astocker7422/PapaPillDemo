using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Controls behavior of an enemy's line of sight
//Attached to child of enemy with sphere collider trigger
public class EnemyCollider : MonoBehaviour
{
    //Enemy the object is a child of
    public Enemy enemy;

    //The trigger
    public SphereCollider lineOfSight;

    //The player
    public Player player;

    //Indicates the player is within range
    public bool playerInSight;

    void Start ()
    {
        //Indicate the player is out of range
        playerInSight = false;
	}
	
	void Update ()
    {
        //If the player is in range, move toward them
        if (playerInSight)
        {
            enemy.moveToPlayer();
        }
        else
        {
            enemy.Idle();
        }
	}

    void OnTriggerEnter(Collider other)
    {
        //If the player enters the trigger,
        if (other.gameObject.CompareTag("Player"))
        {
            //If the enemy is not a water enemy, play the sighting audio
            if (!enemy.isShark) enemy.gameObject.GetComponent<AudioSource>().PlayOneShot(Resources.Load("Audio/Enemy/EnemySight-Hey") as AudioClip);
            else
            {
                enemy.gameObject.GetComponent<AudioSource>().PlayOneShot(Resources.Load("Audio/Enemy/SharkAttack") as AudioClip);
                enemy.gameObject.GetComponent<Animator>().SetBool("Attack", true);
            }

            //Indicate the player is in range
            playerInSight = true;

            //Play walk animation
            enemy.Walk();
        }
    }

    void OnTriggerExit(Collider other)
    {
        //If the player exits the trigger
        if (other.gameObject.CompareTag("Player"))
        {
            if(enemy.isShark)
            {
                enemy.gameObject.GetComponent<Animator>().SetBool("Attack", false);
            }

            //Indicate the player is out of range
            playerInSight = false;

            //Stop walk animation
            enemy.Idle();
        }
    }
}

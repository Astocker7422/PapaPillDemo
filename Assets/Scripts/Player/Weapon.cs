using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Controls behavior of the player's weapon object
//Attached to the player's weapon object
public class Weapon : MonoBehaviour
{
    //The player holding the weapon
    public Player player;

    //Indication that the weapon is being used
    public bool isSwinging;

    //The animator and audio components
    private Animator animator;
    private AudioSource audioSource;

    //The timer to allow animation to play before indicating not swinging
    private float swingTimer;
    private float swingTime;

    void Start ()
    {
        //Initialize the animator and audio components
        animator = transform.GetComponent<Animator>();
        audioSource = transform.GetComponent<AudioSource>();

        //Initialize timer variables
        swingTimer = 0;
        swingTime = 0.8f;
	}

    void Update()
    {
        //If the timer has reached the appropriate time,
        if(swingTimer >= swingTime)
        {
            //Indicate the weapon is not swinging
            animator.SetBool("Attack", false);
            isSwinging = false;

            //Reset the timer
            swingTimer = 0;
        }
    }

    void LateUpdate()
    {
        //If the weapon is swinging,
        if(isSwinging)
        {
            //Update the timer
            swingTimer += Time.deltaTime;
        }
    }

    //Handles the attack animation and audio
    public void PerformAttack()
    {
        //Play the attack animation
        animator.SetBool("Attack", true);
        isSwinging = true;

        if (swingTimer == 0)
        {
            //Play the attacking audio
            audioSource.Play();
            audioSource.PlayOneShot(Resources.Load("Audio/Player/PlayerAttack-Peterson") as AudioClip);
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        //If the weapon is being used,
        if (isSwinging)
        {
            //If an enemy enters the weapon's trigger,
            if(coll.gameObject.CompareTag("Enemy"))
            {
                //Apply damage to the enemy
                coll.gameObject.GetComponent<Enemy>().TakeDamage(player.power);

                //Play the hitting audio
                audioSource.PlayOneShot(Resources.Load("Audio/Player/SwordHit") as AudioClip);
            }
            //If the boss enters the weapon's trigger,
            if(coll.gameObject.CompareTag("Boss"))
            {
                //Apply damage to the boss
                Boss bossScript = coll.gameObject.GetComponent<Boss>();
                bossScript.TakeDamage(player.power);

                //Play the hitting audio
                audioSource.PlayOneShot(Resources.Load("Audio/Player/SwordHit") as AudioClip);
            }
        }
    }
}

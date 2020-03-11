using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Controls the behavior of the player's shield object
//Attached to the player's shield object
public class Shield : MonoBehaviour
{
    //The player holding the shield
    public Player player;

    //Indication of whether the shield is being used
    public bool isBlocking;

    //Animator component
    private Animator animator;

    void Start()
    {
        //Initialize the animator component
        animator = transform.GetComponent<Animator>();
    }

    //Plays the blocking animation
    public void PerformBlock(bool block)
    {
        //Play the blocking animation or reverse
        animator.SetBool("Blocking", block);

        //Indicate the shield is blocking or not
        isBlocking = block;
    }
}

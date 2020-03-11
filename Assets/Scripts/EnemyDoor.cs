using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Controls behavior of door that opens after killing enemies
//Attached to secret door that opens
public class EnemyDoor : MonoBehaviour
{
    //List of enemies to defeat
    public GameObject[] Enemies;

    //The number of enemies that must be defeated
    private int numEnemies;

    //Timer values to let the animation play
    private float timer;
    private float destroyTime;

    void Start()
    {
        //Initialize number of enemies and timer values
        numEnemies = Enemies.Length;
        timer = 0;
        destroyTime = 10f;
    }

    void Update()
    {
        //The number of enemies defeated
        int inactive = 0;

        //For every enemy,
        for (int index = 0; index < Enemies.Length; index++)
        {
            //If the enemy is inactive,
            if (Enemies[index] == null)
            {
                //Increment the number of enemies defeated
                inactive++;
            }
        }

        //If all of the enemies were defeated,
        if(inactive == numEnemies)
        {
            //Play the door animation and increment the timer
            transform.gameObject.GetComponent<Animator>().SetBool("isComplete", true);
            timer += Time.deltaTime;
        }

        //If the timer is complete,
        if(timer >= destroyTime)
        {
            //Deactivate the door
            Destroy(transform.gameObject);
        }
    }
}

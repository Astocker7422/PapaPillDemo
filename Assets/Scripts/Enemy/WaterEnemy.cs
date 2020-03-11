using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Controls the behavior of enemies living in water
//Attached to enemies that stay in water
public class WaterEnemy : MonoBehaviour
{
    //Line of sight trigger
    public EnemyCollider range;

    //Speed of going up and down
    public float ySpeed;

    //The enemy script component
    private Enemy enemy;

    void Start()
    {
        //Initialize the enemy script component
        enemy = transform.GetComponent<Enemy>();
    }

    void Update()
    {
        //If the player is within range,
        if (range.playerInSight)
        {
            //If the enemy is below the player,
            if (enemy.player.transform.position.y - enemy.transform.position.y > 0)
            {
                //Move up
                enemy.rigid.velocity += new Vector3(0, ySpeed, 0);
            }
            //If the enemy is above the player
            else if(enemy.player.transform.position.y - enemy.transform.position.y < 0)
            {
                //Move down
                enemy.rigid.velocity += new Vector3(0, -ySpeed, 0);
            }
        }
        //If the player is out of range,
        else
        {
            //Stop moving
            enemy.rigid.velocity = new Vector3(0, 0, 0);
        }

        //If the enemy is at the surface of the water,
        if(enemy.transform.position.y > 199)
        {
            //The enemy cannot move up
            enemy.transform.position = new Vector3(enemy.transform.position.x, 199, enemy.transform.position.z);
        }
        //If the enemy is at the bottom of the water,
        if (enemy.transform.position.y < 151.5f)
        {
            //The enemy cannot move down
            enemy.transform.position = new Vector3(enemy.transform.position.x, 151.5f, enemy.transform.position.z);
        }
    }
}

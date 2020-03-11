using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Controls the behavior of the level's boss
//Attached to the boss object
public class Boss : MonoBehaviour
{
    //Boss attributes
    public float maxHealth;
    public float power;
    private float currHealth;

    //The player
    public Player player;

    //UI elements
    public GameObject healthBar;
    public GameObject WinCanvas;

    //Boss attack range trigger and particle effect
    public GameObject attackRadius;
    public GameObject magicAttack;

    //The world's pause object
    public Pause pause;

    //The boundaries of the boss teleportation ability
    [Header("Teleport Boundaries")]
    public float MinX;
    public float MaxX;
    public float MinY;
    public float MaxY;
    public float MinZ;
    public float MaxZ;

    //Timer values to control invincibilty after damage
    private float hitTime;
    private float hitTimer;
    private bool canHit;

    //Timer values to indicate time to attack
    private float attackTime;
    private float attackTimer;
    private bool isAttacking;

    //Variables controlling when and how many times to teleport
    private float teleportTime;
    private float teleportTimer;
    private int teleportCount;

    //The audio component
    private AudioSource audioSource;

    //Indicates boss is dead
    private bool isDead;

    void Start()
    {
        //Initialize health
        currHealth = maxHealth;

        //Initialize damage timer values
        hitTime = 0.5f;
        hitTimer = 0;
        canHit = true;

        //Initialize attack timer values
        attackTimer = 0;
        attackTime = 5f;
        isAttacking = false;

        //Initialize teleport timer values
        teleportTimer = 0;
        teleportTime = 4f;
        teleportCount = 0;

        //Initialize boss health bar
        healthBar.GetComponent<Slider>().maxValue = maxHealth;
        healthBar.GetComponent<Slider>().value = maxHealth;

        //Initialize audio component
        audioSource = transform.GetComponent<AudioSource>();

        //Indicate boss is not dead
        isDead = false;
    }

    void Update()
    {
        //If the boss is not dead
        if (!isDead)
        {
            //If the attack is complete,
            if (attackTimer >= attackTime)
            {
                //Deactivate trigger and particle effect
                attackRadius.SetActive(false);
                magicAttack.SetActive(false);

                //Indicate boss is not attacking and reset timer
                isAttacking = false;
                attackTimer = 0;

                //Stop the attack audio
                audioSource.Stop();
            }

            //If it is time to teleport and the boss is not attacking,
            if (teleportTimer >= teleportTime && !isAttacking)
            {
                //Teleport to a random position, reset the timer, and increment the count
                transform.position = RandomPosition();
                teleportTimer = 0;
                teleportCount++;

                //Play the teleport audio
                audioSource.PlayOneShot(Resources.Load("Audio/Boss/Teleport") as AudioClip);

                //Occasionally play the laughing audio
                float chance = Random.Range(0, 5);
                if (chance == 0)
                {
                    audioSource.PlayOneShot(Resources.Load("Audio/Boss/BossLaugh") as AudioClip);
                }
            }

            //If the boss has teleported enough times,
            if (teleportCount == 4)
            {
                //Reset the count, indicate the boss is attacking, and activate attack
                teleportCount = 0;
                isAttacking = true;
                StartCoroutine(Attack());
            }
        }
    }

    void LateUpdate()
    {
        //If the boss is attacking, increment the timer
        if (isAttacking) attackTimer += Time.deltaTime;

        //Increment the invincibility timer
        //If the time is up, the player can be hit
        hitTimer += Time.deltaTime;
        if (hitTimer > hitTime) canHit = true;

        //Increment the teleport timer
        teleportTimer += Time.deltaTime;
    }

    //Handles the attack behavior
    IEnumerator Attack()
    {
        //Play the attack audio and teleport to the player
        audioSource.Play();
        transform.position = AttackPosition();

        yield return new WaitForSeconds(3);

        //After waiting, activate attack trigger and particle effect
        attackRadius.SetActive(true);
        magicAttack.SetActive(true);
        magicAttack.GetComponent<ParticleSystem>().Play();
    }
    
    //Teleport to player
    Vector3 AttackPosition()
    {
        //Calculate an x coordinate 1 away from the player in any direction
        //Check the teleport agains the boundaries
        float teleportX = player.transform.position.x + RandomSign();
        if (teleportX > MaxX) teleportX = MaxX;
        if (teleportX < MinX) teleportX = MinX;

        //Calculate a z coordinate 1 away from the player in any direction
        //Check the teleport agains the boundaries
        float teleportZ = player.transform.position.z + RandomSign();
        if (teleportZ > MaxZ) teleportZ = MaxZ;
        if (teleportZ < MinZ) teleportZ = MinZ;

        //Return the new position
        return new Vector3(teleportX, MinY, teleportZ);
    }

    //Calculates a random position within the teleport boundaries
    Vector3 RandomPosition()
    {
        //Calculate coordinates of new position within boundaries
        float teleportX = UnityEngine.Random.Range(MinX, MaxX);
        float teleportY = UnityEngine.Random.Range(MinY, MaxY);
        float teleportZ = UnityEngine.Random.Range(MinZ, MaxZ);

        //Return new position
        return new Vector3(teleportX, teleportY, teleportZ);
    }

    //Apply damage to boss
    public void TakeDamage(float damage)
    {
        //If boss was hit to recently, stop
        if (!canHit) return;

        //If the health bar is not active, activate it
        if (!healthBar.activeInHierarchy) healthBar.SetActive(true);

        //Decrement health and update health bar
        currHealth -= damage;
        healthBar.GetComponent<Slider>().value = currHealth;

        //Play hurt audio
        audioSource.PlayOneShot(Resources.Load("Audio/Boss/BossHurt") as AudioClip);

        //Start the invincibility timer and indicate the boss cannot be damaged
        hitTimer = 0;
        canHit = false;

        //If the boss has no health,
        if (currHealth <= 0)
        {
            //Trigger death
            StartCoroutine(Die());
        }
    }

    IEnumerator Die()
    {
        //Deactivate trigger and particle effect
        attackRadius.SetActive(false);
        magicAttack.SetActive(false);

        //Indicate boss is not attacking and reset timer
        isAttacking = false;
        attackTimer = 0;

        //Stop the attack audio
        audioSource.Stop();
    
        //Play the dying audio and make the boss object invisible
        audioSource.PlayOneShot(Resources.Load("Audio/Boss/BossDie") as AudioClip);
        GetComponent<MeshRenderer>().enabled = false;

        //Stop boss behavior
        isDead = true;

        //Deactivate the health bar
        healthBar.SetActive(false);

        //Wait
        yield return new WaitForSeconds(5);

        //Destroy the boss object
        Destroy(this.gameObject);

        //Activate the winning canvas and select the first button
        WinCanvas.SetActive(true);
        WinCanvas.transform.FindDeepChild("Main Menu Button").GetComponent<Button>().Select();

        //Change the music to the level music
        GameObject.Find("Boss Music").GetComponent<AudioSource>().enabled = false;
        GameObject.Find("Music").GetComponent<AudioSource>().enabled = true;

        //Pause the player and the world
        player.Paused(true);
        Time.timeScale = 0f;
    }

    //Resets boss health values
    public void Reset()
    {
        //Reset health value and health bar
        currHealth = maxHealth;
        healthBar.GetComponent<Slider>().value = currHealth;
    }

    //Returns 1 or -1
    int RandomSign()
    {
        if (Random.Range(0, 2) == 0)
        {
            return -1;
        }
        return 1;
    }
}

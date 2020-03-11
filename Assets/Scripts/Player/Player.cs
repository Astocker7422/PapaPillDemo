using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Controls input and behaviors of the player object
//Attached to the player object
public class Player : MonoBehaviour
{
    //Player attributes
    public float maxHealth;
    public float speed;
    public float turnSpeed;
    public float jumpSpeed;
    public float power;

    //Proper respawn transformation values
    public Vector3 respawn;
    public Quaternion respawnRotation;

    //Variables for coin collection
    public float collectGoal;
    public int count;
    public TMPro.TMP_Text countText;
    //public Text countText;

    //The player's weapon and shield
    public Weapon weapon;
    public Shield shield;
    public Slider healthBar;

    //Booleans to check for pausing and swimming 
    public bool isPaused;
    public bool isSwimming;
    
    //Player's current health
    private float currHealth;

    //Variables for jumping and running
    private bool isGrounded;
    private float defaultJumpSpeed;
    private bool isRunning;

    //Components of the player object
    private AudioSource audioSource;
    private Rigidbody rigid;
    private Animator animator;

    //Variables to keep track of invincibility after damage
    private float hitTime;
    private float hitTimer;
    private bool canHit;

    //Variables for death behavior
    private bool isDead;
    private float deathTimer;
    private float deathTime;

    //Timer to control jumping while swimming
    private float swimTimer;
    private float swimTime;

    //Timer to stop jumping after unpause
    private float jumpTimer;
    private float jumpTime;

    //Boolean to represent if cutscene is playing
    public bool isCutscene;

    void Start()
    {
        //Initialize the coin count and change the text
        count = 0;
        SetCountText();

        //Initialize components
        audioSource = GetComponent<AudioSource>();
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        //Maximize health
        currHealth = maxHealth;

        //Initialize player to not be invincible
        hitTime = 0.4f;
        hitTimer = hitTime;
        canHit = true;

        //Initialize running state and original jump speed
        isRunning = false;
        defaultJumpSpeed = jumpSpeed;

        //Initialize death variables
        isDead = false;
        deathTime = 5;
        deathTimer = 0;

        //Initialize swimming timer
        swimTime = 0.48f;
        swimTimer = swimTime;

        //Initialize jump timer
        jumpTime = 0.5f;
        jumpTimer = jumpTime;

        //Initialize cutscene indicator
        isCutscene = false;
    }

    void Update()
    {
        //If the player is not paused and is not dead.
        if(!isPaused && !isDead)
        {
            float block = Input.GetAxis("Block");

            //If the block button is pressed,
            if (Input.GetButton("Block") || block < 0)
            {
                shield.PerformBlock(true);
            }
            //If the block button is released,
            else
            {
                shield.PerformBlock(false);
            }

            //If the attack button is presssed,
            if (Input.GetButtonDown("Attack") && !shield.isBlocking)
            {
                shield.PerformBlock(false);
                weapon.PerformAttack();
            }

            //If the sprint button is pressed,
            if (Input.GetButtonDown("Sprint"))
            {
                //If the player is not running,
                if (!isRunning)
                {
                    //Increase movement speed and indicate the player is running
                    speed *= 2;
                    isRunning = true;

                    //Increase the walk animation speed
                    animator.speed = 1.5f;
                }
                //If the player is running,
                else
                {
                    //Decrease movement speed and indicate the player is not running
                    speed /= 2;
                    isRunning = false;

                    //Decrease the walk animation speed
                    animator.speed = 1;
                }
            }

            //Take the movement input and apply movement speed
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            //Apply movement variables
            Movement(horizontal, vertical);

            //Apply jump
            Jump();
        }

        //If the game is paused,
        if (isPaused && !isCutscene)
        {
            //If there is mouse movement
            if (Input.GetAxis("MouseHorizontal") != 0 || Input.GetAxis("MouseVertical") != 0)
            {
                //Make the cursor visible
                Cursor.visible = true;
            }
        }

        //If the player is dead and the animation is done playing,
        if (isDead && deathTimer >= deathTime)
        {
            //Reset player values
            Respawn();
        }
    }

    void LateUpdate()
    {
        //Increment the invincibility timer
        //If the time is up, the player can be hit
        hitTimer += Time.deltaTime;
        if (hitTimer > hitTime) canHit = true;

        //If the player is swimming,
        if(isSwimming)
        {
            //Increment the swim jump timer
            swimTimer += Time.deltaTime;
        }

        //If the player is dead,
        if(isDead)
        {
            //Increment the death timer
            deathTimer += Time.deltaTime;
        }

        if(jumpTimer > 0 && !isPaused)
        {
            jumpTimer -= Time.deltaTime;
        }
    }

    //Applies movement to the player using the RigidBody
    void Movement(float horizontal, float vertical)
    {
        //If there is enough movement input,
        if(Mathf.Abs(horizontal) > 0.1 || Mathf.Abs(vertical) > 0.1)
        {
            //Let the animator know there is movement input
            animator.SetBool("Input", true);
        }
        //If there is not enough movement input,
        else
        {
            //Let the animator know there is not movement input
            animator.SetBool("Input", false);
        }

        //If there is input,
        if (horizontal != 0f || vertical != 0f)
        {
            //Trigger walk animation if not already playing
            if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Walk") && isGrounded)
            {
                animator.SetTrigger("Walk");
            }

            //Calculate direction of input and angle
            Vector3 inputDir = new Vector3(horizontal, 0, vertical);
            float applyDir = Vector3.Angle(Vector3.forward, inputDir);

            //If input is pointed left, apply negative angle
            if(horizontal < 0)
            {
                applyDir = -applyDir;
            }

            //Gradually rotate the player in the direction of the input, taking the camera's rotation into account
            Quaternion direction = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y + applyDir, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, direction, Time.deltaTime / turnSpeed);

            //Move the RigidBody based on the direction the player is facing
            rigid.velocity = new Vector3(transform.forward.x * speed, rigid.velocity.y, transform.forward.z * speed);

            animator.SetBool("IsIdle", false);
        }
        //If there is no input,
        else
        {
            //Stop animation
            animator.SetTrigger("Idle");
            animator.SetBool("IsIdle", true);

            //Stop moving
            rigid.velocity = new Vector3(0, rigid.velocity.y, 0);
        }
    }

    //Applies jumping behavior to the player object using the RigidBody
    void Jump()
    {
        //If the jump button is pressed,
        if (Input.GetButtonDown("Jump") && jumpTimer <= 0)
        {
            //If the player is on the ground,
            if (isGrounded)
            {
                //If the player object has a parent (possibly a moving platform),
                if (transform.parent)
                {
                    //If the parent classifies as a moving platform,
                    if (transform.parent.CompareTag("Moving"))
                    {
                        //If the moving platform is moving upward,
                        if (transform.parent.GetComponent<MovingPlatform>().goingUp)
                        {
                            //Apply a jump with extra force to offset the platform also moving and play the jump audio
                            rigid.AddForce(Vector3.up * (jumpSpeed * 1.3f), ForceMode.Impulse);
                            audioSource.PlayOneShot(Resources.Load("Audio/Player/JumpLow") as AudioClip);
                            audioSource.PlayOneShot(Resources.Load("Audio/Player/PlayerJump-Short") as AudioClip);
                            animator.SetTrigger("Jump");
                            animator.ResetTrigger("Walk");
                            return;
                        }
                    }
                }

                //If the player is swimming,
                if (isSwimming)
                {
                    //If the player has not jumped too recently,
                    if (swimTimer >= swimTime)
                    {
                        //Apply a jump and play the jump audio
                        rigid.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
                        audioSource.PlayOneShot(Resources.Load("Audio/Player/JumpLow") as AudioClip);
                        audioSource.PlayOneShot(Resources.Load("Audio/Player/PlayerJump-Short") as AudioClip);
                        animator.SetTrigger("Jump");
                        animator.ResetTrigger("Walk");
                        swimTimer = 0;
                    }
                }
                //If the player is not swimming,
                else
                {
                    //Apply a jump and play the jump audio
                    rigid.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
                    audioSource.PlayOneShot(Resources.Load("Audio/Player/JumpLow") as AudioClip);
                    audioSource.PlayOneShot(Resources.Load("Audio/Player/PlayerJump-Short") as AudioClip);
                    animator.SetTrigger("Jump");
                    animator.ResetTrigger("Walk");
                }
            }
        }
    }

    //Resets player values after death animation plays
    void Respawn()
    {
        //Respawn the player object and reset health
        transform.position = respawn;
        transform.rotation = respawnRotation;
        currHealth = maxHealth;
        healthBar.value = maxHealth;

        //If the player died under water
        if (RenderSettings.fog == true)
        {
            //Reset the gravity and player's jump speed
            Physics.gravity = new Vector3(0, -20f, 0);
            jumpSpeed = defaultJumpSpeed;

            //Indicate the player is now on the ground and not swimming
            isGrounded = true;
            isSwimming = false;

            //Turn off the underwater fog and muffled sound effects
            RenderSettings.fogColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            RenderSettings.fogDensity = 0.002f;
            RenderSettings.fog = false;
            GameObject.Find("Music").GetComponent<AudioLowPassFilter>().cutoffFrequency = 5000;
            transform.GetComponent<AudioLowPassFilter>().cutoffFrequency = 5000;
            weapon.GetComponent<AudioLowPassFilter>().cutoffFrequency = 5000;
        }

        //Indicate the player is no longer dead and reset the timer
        isDead = false;
        deathTimer = 0;
    }
    
    //Controls indication of paused game and cursor visibility
    public void Paused(bool paused)
    {
        //Set the paused indication
        isPaused = paused;

        //Make the cursor invisible
        Cursor.visible = false;

        if (isPaused)
        {
            jumpTimer = jumpTime;
        }
    }

    void OnCollisionEnter(Collision coll)
    {
        //Stop the player object
        rigid.angularVelocity = new Vector3(0, 0, 0);

        //If the collision was with an enemy,
        if (coll.gameObject.CompareTag("Enemy"))
        {
            //Apply damage to player and play the damage audio
            TakeDamage(coll.collider.gameObject.GetComponent<Enemy>().power);
            audioSource.PlayOneShot(Resources.Load("Audio/Player/PlayerHurt") as AudioClip);
        }

        //If the collision was with the ground,
        if(coll.gameObject.CompareTag("Ground"))
        {
            //If the player is jumping,
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
            {
                //Play the walk animation if moving
                if(Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
                {
                    animator.SetTrigger("Walk");
                }
                //Otherwise play the idle animation
                else
                {
                    animator.SetTrigger("Idle");
                }
            }
        }
    }

    //Applies damage to the player
    void TakeDamage(float damage)
    {
        //If the player has been hurt too recently, stop
        if (!canHit) return;

        //If the player is blocking,
        if(shield.isBlocking)
        {
            //Play the block sound and stop
            audioSource.PlayOneShot(Resources.Load("Audio/Player/Block") as AudioClip);
            return;
        }

        //If the player is dead or the game is paused, stop
        if (isDead || isPaused) return;


        //Decrement the player's current health and change the health bar
        currHealth -= damage;
        healthBar.value = currHealth;

        //Start the invincibility timer and indicate the player cannot be damaged
        hitTimer = 0;
        canHit = false;

        //Play the damage audio
        audioSource.PlayOneShot(Resources.Load("Audio/Player/Hit") as AudioClip);
        audioSource.PlayOneShot(Resources.Load("Audio/Player/PlayerHurt") as AudioClip);

        //If the player is out of health,
        if (currHealth <= 0)
        {
            //Stop animation
            animator.SetTrigger("Idle");

            //Play the death animation, indicate the player's death, and play the death audio
            transform.GetComponent<Animation>().Play();
            isDead = true;
            audioSource.PlayOneShot(Resources.Load("Audio/Player/PlayerDie") as AudioClip);
        }
    }

    void OnCollisionStay(Collision coll)
    {
        //Stop the player object
        rigid.angularVelocity = new Vector3(0, 0, 0);

        //If the player collided with the side of a platform or a wall,
        if (coll.gameObject.CompareTag("Wall") || coll.gameObject.CompareTag("Platform"))
        {
            //The player cannot jump
            isGrounded = false;
        }
        //If the player did not collide with the side of a platform or a wall,
        else
        {
            //The player can jump
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision coll)
    {
        //If the player was on the ground and is not swimming,
        if (isGrounded && !isSwimming)
        {
            //The player cannot jump
            isGrounded = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        //If the player enters a coin's trigger
        if(other.gameObject.CompareTag("Coin"))
        {
            //Play the coin audio
            audioSource.Play();

            //Deactivate the coin
            other.gameObject.SetActive(false);

            //Increment the coin count and change the canvas text
            count++;
            SetCountText();

            //If the player is not at maximum health,
            if(currHealth < maxHealth)
            {
                //Increment the player's health and change the health bar
                currHealth += 1;
                healthBar.value = currHealth;
            }
        }
        //If the player enter's an enemy's weapon trigger
        if (other.gameObject.CompareTag("Enemy Weapon"))
        {
            //Get the enemy object and apply damage based on whether it is a regular enemy or a boss
            GameObject enemy = other.gameObject.transform.parent.gameObject;
            if (enemy.CompareTag("Enemy")) TakeDamage(other.gameObject.transform.parent.GetComponent<Enemy>().power);
            if (enemy.CompareTag("Boss")) TakeDamage(other.gameObject.transform.parent.GetComponent<Boss>().power);

            //Calculate a slight force backward and up
            float force = 2;
            float height = 2;
            Vector3 enemyDir = other.gameObject.transform.forward;

            //If the player is blocking,
            if (shield.isBlocking)
            {
                //Negate the force
                force /= 2;
                height = 0;
            }

            //Apply the calculated force
            Vector3 dir = new Vector3(enemyDir.x * force, height, enemyDir.z * force);
            rigid.AddForce(dir, ForceMode.Impulse);
        }
        //If the player entered a water trigger,
        if(other.gameObject.CompareTag("Water"))
        {
            //Slow the walking animation
            animator.speed = 0.5f;

            //Indicate the player is swimming and can jump
            isSwimming = true;
            isGrounded = true;

            //Play the water splash audio and muffle the player audio
            audioSource.PlayOneShot(Resources.Load("Audio/Player/Splash") as AudioClip);
            transform.GetComponent<AudioLowPassFilter>().cutoffFrequency = 500;
            weapon.GetComponent<AudioLowPassFilter>().cutoffFrequency = 500;
        }
    }

    void OnTriggerStay(Collider other)
    {
        //If the player stays on the ground,
        if (other.gameObject.tag == "Ground")
        {
            //If the other object is a grandchild (indicates a moving platform) and the scale of the grandparent is standard,
            if (other.transform.parent.parent != null && other.transform.parent.parent.localScale.Equals(new Vector3(1, 1, 1)))
            {
                //Make the grandparent the parent of the player object
                transform.parent = other.transform.parent.parent;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        //If the player leaves the ground or top of a platform,
        if (other.gameObject.tag == "Ground")
        {
            //Unparent the player
            transform.parent = null;
        }
        //If the player is swimming and exits a water trigger,
        if (other.gameObject.CompareTag("Water") && isSwimming)
        {
            //If the player is now above water,
            if(transform.position.y > other.transform.position.y)
            {
                //Indicate the player is not swimming and cannot jump
                isSwimming = false;
                isGrounded = false;

                Debug.Log(rigid.velocity.y);

                //Un-muffle the player audio
                transform.GetComponent<AudioLowPassFilter>().cutoffFrequency = 5000;
                weapon.GetComponent<AudioLowPassFilter>().cutoffFrequency = 5000;
            }
        }
    }

    //Updates the text on the canvas keeping track of the player's coins
    void SetCountText()
    {
        countText.text = "Coins: " + count.ToString();
    }
}

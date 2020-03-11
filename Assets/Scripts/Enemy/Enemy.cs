using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Controls behavior of enemy object
//Attacked to enemy object
public class Enemy : MonoBehaviour
{
    //Enemy attributes
    public float maxHealth;
    public float speed;
    public float turnSpeed;
    public float power;
    public float attackDistance;
    private float currHealth;

    //The player
    public Player player;

    //The enemy health bar
    public GameObject healthBar;
    
    //The RigidBody and audio components
    public Rigidbody rigid;
    private AudioSource audioSource;

    //The enemy's animator
    private Animator animator;

    //Body collider
    private BoxCollider bodyCollider;

    //Weapon's animator component
    private Animator weaponAnimator;

    //The particle effect played on death
    public GameObject deathEffect;

    //Indicates if is a water enemy
    public bool isShark;

    //Variables to keep track of invincibility after damage
    private float hitTime;
    private float hitTimer;
    private bool canHit;

    //Control when the enemy can attack
    private float attackTime;
    private float attackTimer;
    private bool isAttacking;

    //Indicates if the enemy is dead
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
        attackTime = 2f;
        attackTimer = attackTime;
        isAttacking = false;

        //Initialize RigidBody and audio components
        rigid = transform.GetComponent<Rigidbody>();
        audioSource = transform.GetComponent<AudioSource>();

        //Initialize the animator
        animator = GetComponent<Animator>();

        //Initialize body collider
        bodyCollider = transform.GetComponent<BoxCollider>();

        //Indicate the enemy is not dead
        isDead = false;

        //If the object does not have a WaterEnemy component,
        if (GetComponent<WaterEnemy>() == null)
        {
            //Indicate is not a water enemy
            isShark = false;
        }
        //If the object has a WaterEnemy component,
        else
        {
            //Indicate is a water enemy
            isShark = true;
        }

        //Initialize weapon's animator component
        if (!isShark) weaponAnimator = transform.Find("Fist").GetComponentInChildren<Animator>();

        player = GameObject.Find("Player").GetComponent<Player>();
    }

    void Update()
    {
        //If the enemy is not dead
        if (!isDead)
        {
            //If the enemy is within attacking range,
            if (Vector3.Distance(transform.position, player.transform.position) <= attackDistance)
            {
                //Stop walking animation
                animator.SetTrigger("Idle");

                //If the enemy is not attacking and is allowed to attack
                if (isAttacking == false && attackTimer >= attackTime)
                {
                    //Play attack animation
                    if (!isShark) weaponAnimator.SetBool("Attack", true);

                    //If is not a water enemy,
                    if (!isShark)
                    {
                        //Play normal enemy attack audio
                        audioSource.PlayOneShot(Resources.Load("Audio/Enemy/EnemyAttack") as AudioClip);
                    }

                    //Indicate is attacking and initialize timer
                    isAttacking = true;
                    attackTimer = 0;
                }
                //If the enemy is attacking or is not allowed to attack
                else
                {
                    //Stop the animation and indicate is not attacking
                    if (!isShark) weaponAnimator.SetBool("Attack", false);
                    isAttacking = false;
                }
            }
            //If the enemy is out of attacking range,
            else
            {
                //Stop the animation and indicate is not attacking
                if (!isShark)
                {
                    weaponAnimator.SetBool("Attack", false);
                }

                isAttacking = false;
            }

            if (transform.rotation.eulerAngles.x > 25)
            {
                rigid.AddTorque(Vector3.up);
            }
        }
    }

    void LateUpdate()
    {
        //Increment the invincibility timer
        //If the time is up, the player can be hit
        hitTimer += Time.deltaTime;
        if (hitTimer > hitTime) canHit = true;

        //Increment the attack timer
        attackTimer += Time.deltaTime;
    }

    //Applies damage to enemy object
    public void TakeDamage(float damage)
    {
        //If enemy was hit to recently, stop
        if (!canHit) return;

        //If the enemy health bar inactive, activate the health bar
        if (!healthBar.activeInHierarchy) healthBar.SetActive(true);

        //Decrement health and update health bar
        currHealth -= damage;
        healthBar.GetComponent<Slider>().value = currHealth;

        //If is not a water enemy, play the normal enemy hurt audio
        if(!isShark) audioSource.PlayOneShot(Resources.Load("Audio/Enemy/EnemyHurt") as AudioClip);

        //Start the invincibility timer and indicate the boss cannot be damaged
        hitTimer = 0;
        canHit = false;

        //If the enemy is out of health,
        if(currHealth <= 0)
        {
            //Start dying behavior
            StartCoroutine(Die());
        }
    }

    //Moves the enemy toward the player
	public void moveToPlayer()
    {
        //If the enemy is attacking or is dead, stop
        if(isAttacking || isDead) return;

        //Look at target
        Vector3 v3 = player.transform.position - transform.position;
        v3.y = 0.0f;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(v3), turnSpeed * Time.deltaTime);

        //If the enemy is out of attack range,
        if (Vector3.Distance(transform.position, player.transform.position) > attackDistance)
        {
            Walk();

            //Move towards target
            rigid.velocity = new Vector3(transform.forward.x * speed, rigid.velocity.y, transform.forward.z * speed);
        }
        else
        {
            animator.SetTrigger("Idle");
        }
    }

    //Makes enemy play idle animation
    public void Idle()
    {
        animator.SetTrigger("Idle");
    }

    //Makes enemy play walk animation
    public void Walk()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Walk") && !isShark)
        {
            animator.SetTrigger("Walk");
        }
    }

    //Handles behavior of enemy when health is depleted
    IEnumerator Die()
    {
        //Indicate the enemy is dead and deactivate the health bar
        isDead = true;
        healthBar.SetActive(false);

        //If is not a water enemy,
        if (!isShark)
        {
            //Deactivate collider and disable gravity
            //to prevent player colliding with enemy after death
            rigid.useGravity = false;
            bodyCollider.enabled = false;

            //Deactivate the body
            transform.FindDeepChild("Body").GetComponent<MeshRenderer>().enabled = false;

            //Play death audio and activate death effect
            audioSource.PlayOneShot(Resources.Load("Audio/Enemy/EnemyDie") as AudioClip);
            deathEffect.SetActive(true);
            deathEffect.GetComponent<ParticleSystem>().Play();

            //Deactivate weapon
            transform.Find("Fist").gameObject.SetActive(false);

            yield return new WaitForSeconds(2);

            //Destroy the enemy object after waiting
            Destroy(this.gameObject);
        }
        //If is a water enemy,
        else
        {
            //Destroy the enemy object
            Destroy(this.gameObject);
        }
    }

    void OnCollisionEnter(Collision coll)
    {
        //If the player hits the enemy and is a water enemy,
        if (coll.collider.tag.Equals("Player") && isShark)
        {
            //Play the shark enemy audio
            audioSource.PlayOneShot(Resources.Load("Audio/Enemy/SharkAttack") as AudioClip);
        }
    }

    void OnCollisionStay(Collision coll)
    {
        //If the player hits the enemy,
        if(coll.collider.tag.Equals("Player"))
        {
            //Freeze the enemy
            rigid.constraints = RigidbodyConstraints.FreezePosition;
        }
    }

    void OnCollisionExit(Collision coll)
    {
        //If the enemy stops touching the player,
        if (coll.collider.tag.Equals("Player"))
        {
            //Unfreeze the enemy
            rigid.constraints = RigidbodyConstraints.None;
        }
    }
}

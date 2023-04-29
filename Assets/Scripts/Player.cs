using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Header("Player stats")]
    public float speed = 10.0f;
    public float jumpForce = 5.0f;
    public int curHp = 25;
    public int maxHp = 25;
    public int score = 0;
    private int damage = 2;
    private float attackRange = 2.5f;
    private float attackRate = 1.0f;
    private bool isOnGround = true;
    private bool isAttacking = false;
    private bool isMoving = false;
    public bool hasKey = false;
    public float gravityModifier;
    private AudioSource playerAudioSource;
    public AudioClip swordSound;
    public AudioClip takeDamageSound;
    public Camera miniMapCamera;


    public int coinAmount;

    public Rigidbody playerRb;
    public Animator playerAnim;

    //boundary
    [Header("Boundary of the world")]
    public float zBound = 20;
    public float xBound = 20;

    private string sceneName;

    // make a singleton
    public static Player instance;
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // get the name of the current scene
        sceneName = SceneManager.GetActiveScene().name;
        playerRb = GetComponent<Rigidbody>();
        Physics.gravity *= gravityModifier;
        playerAudioSource = GetComponent<AudioSource>();
    }
    private void LateUpdate()
    {
        // make the camera follow the player without rotating
        miniMapCamera.transform.position = new Vector3(transform.position.x, miniMapCamera.transform.position.y, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        
        if(!isAttacking)
        {
            UpdateAnimator();
            Move();
            //Jump();
        }
        if (Input.GetMouseButton(0) && !isAttacking && isOnGround)
        {
            Attack();
            isMoving = false;
            //stop the step sound
        } else
        {
            isMoving = true;
        }
        StayInBound();
        UI.instance.UpdateHealth(curHp, maxHp);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
        }
    }

    void UpdateAnimator()
    {
        playerAnim.SetBool("MovingForwards", false);
        playerAnim.SetBool("MovingBackwards", false);
        playerAnim.SetBool("MovingLeft", false);
        playerAnim.SetBool("MovingRight", false);

        // calculate the local velocity of the player
        Vector3 localVelocity = transform.InverseTransformDirection(playerRb.velocity);

        // if our local z-velocity is positive, we are moving forwards
        if (localVelocity.z > 0.1f)
        {
            playerAnim.SetBool("MovingForwards", true);
        }
        // if our local z-velocity is negative, we are moving backwards
        else if (localVelocity.z < -0.1f)
        {
            playerAnim.SetBool("MovingBackwards", true);
        }
        // if our local x-velocity is positive, we are moving right
        else if (localVelocity.x > 0.1f)
        {
            playerAnim.SetBool("MovingBackwards", true);
        }
        // if our local x-velocity is negative, we are moving left
        else if (localVelocity.x < -0.1f)
        {
            playerAnim.SetBool("MovingBackwards", true);
        }
    }

    void Move()
    {
        if (isMoving)
        {
            // Detecting the inputs from horizontal movement
            float horizontalInput = Input.GetAxis("Horizontal");
            // Detecting the inputs from vertical movement
            float verticalInput = Input.GetAxis("Vertical");

            // get  a relative direction based on where our player is facing
            Vector3 dir = transform.right * horizontalInput + transform.forward * verticalInput;
            // apply movespeed and gravity
            dir *= speed;
            dir.y = playerRb.velocity.y;

            // apply the velocity to the player
            playerRb.velocity = dir;
            // play sound when moving
        } else
        {
            return;
        }
        
        
        
    }

    //void Jump()
    //{
    //    // if the player presses the space bar and the player is on the ground
    //    if (Input.GetKeyDown(KeyCode.Space) && isOnGround)
    //    {
    //        // add jumpForce to the player's rigidbody
    //        playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    //        // set isOnGround to false
    //        isOnGround = false;
    //    }
    //}

    void Attack()
    {
        isAttacking = true;
        playerAudioSource.PlayOneShot(swordSound);
        Invoke(nameof(TryDamage), attackRate/2);
        playerAnim.SetTrigger("Attack");
        Invoke(nameof(DisableIsAttaking), attackRate);
    }

    void TryDamage()
    {
        // create a ray one meter in front of the player
        Ray ray = new Ray(transform.position + transform.forward, transform.forward);

        // cast a sphere and look for all colliders inside it, which are on the "enemy" layer (8)
        RaycastHit[] hits = Physics.SphereCastAll(ray, 1.0f, attackRange, LayerMask.GetMask("Enemy"));

        foreach (RaycastHit hit in hits)
        {
            hit.collider.GetComponent<Enemy>()?.TakeDamage(damage);
        }
    }

    void DisableIsAttaking()
    {
        isAttacking = false;
        
    }

    public void TakeDamage(int damageToTake)
    {
        curHp -= damageToTake;
        playerAudioSource.PlayOneShot(takeDamageSound);
        //playerAnim.SetTrigger("GetHit");
        if (curHp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // restart the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        // deconfine the cursor to the center of the game mode and enable the cursor
        Cursor.lockState = CursorLockMode.None;
    }

    public void Addcoin(int value)
    {
        
        UI.instance.UpdateScore(value);
    }

    public bool AddHealth(int  value)
    {
        if(curHp < maxHp)
        {
            curHp += value;
            if(curHp > maxHp)
            {
                curHp = maxHp;
            }
            UI.instance.UpdateHealth(curHp, maxHp);
            return true;
        }
        return false;

    }

    // prevent the player to leave the world
    void StayInBound()
    {
        // if the name contains "Laby" then return
        if (sceneName.Contains("Laby"))
        {
            return;
        } else if (sceneName.Contains("Rogue"))
        {
            return;
        } else
        {
            //player stays in bound
            if (transform.position.z < -zBound)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, -zBound);
            }
            if (transform.position.z > zBound)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, zBound);
            }
            if (transform.position.x < -xBound)
            {
                transform.position = new Vector3(-xBound, transform.position.y, transform.position.z);
            }
            if (transform.position.x > xBound)
            {
                transform.position = new Vector3(xBound, transform.position.y, transform.position.z);
            }
        }
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public Player player;
    [Header("Enemy Stats")]
    public float chaseRange;
    public float attackRange;
    public int damage;
    private  float attackRate =2.5f;
    public float health;
    public int score = 10;
    private AudioSource enemyAudioSource;
    public AudioClip enemyAttackSound;
    public AudioClip enemyDieSound;
    public AudioClip takeDamageSound;

    private bool isAttacking;
    public bool isDead =false;
    public bool isMoving = false;

    public NavMeshAgent agent;
    public Animator enemyAnim;
    public GameObject miniMapIconEnemy;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
    }

    private void Start()
    {
        isDead = false;
        enemyAudioSource = GetComponent<AudioSource>();
        agent = GetComponent<NavMeshAgent>();
        
    }

    private void Update()
    {
        // if the enemy is currently dead, Stop the function
        if (isDead)
        {
            return;
        }
        Move();
        
    }

    void Move()
    {
        // get the direction toward the player and rotate the enemy to the player
        Vector3 direction = player.transform.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        // get the distance between the enemy and the player
        float distance = Vector3.Distance(transform.position, player.transform.position);

        // if the enemy is outside the chase range
        if (distance > chaseRange)
        {
            // stop the enemy
            agent.isStopped = true;
            enemyAnim.SetBool("MovingForward", false);
            return;
        }
        // if the enemy is inside the chase range but outside the attack range
        else if (distance > attackRange && distance < chaseRange)
        {
            isMoving = true;
            // chase the player
            agent.isStopped = false;
            agent.SetDestination(player.transform.position);
            enemyAnim.SetBool("MovingForward", true);
        }
        // if the enemy is inside the attack range
        else if (distance < attackRange)
        {
            Debug.Log("distance < attackRange");
            // stop the enemy
            agent.isStopped = true;
            enemyAnim.SetBool("MovingForward", false);
            //attack();
            if (!isAttacking)
            {
                Attack();
            }
        }
    }
    void Attack()
    {
        isAttacking = true;
        // play the attack sound
        enemyAudioSource.PlayOneShot(enemyAttackSound);
        // there will be a attachTimer delay between each attack
        Invoke(nameof(TryDamage), attackRate/2);
        // anim the attack
        enemyAnim.SetTrigger("Attack");

        // there will be an attackCoolDown between each call;
        Invoke(nameof(DisableIsAttacking), attackRate);
    }

    void TryDamage()
    {
        // Is the player in front of us?
        if (Vector3.Distance(transform.position, player.transform.position) <= attackRange)
        {
            // if so, damage the player
            player.TakeDamage(damage);
        }
    }

    void DisableIsAttacking()
    {
        // set isAttacking to false
        isAttacking = false;
    }

    public void TakeDamage(int damageToTake)
    {
        // decrease the health of the enemy
        health -= damageToTake;
        // play the take damage sound
        enemyAudioSource.PlayOneShot(takeDamageSound);
        //enemyAnim.SetTrigger("GetHit");

        if (health <= 0)
        {
            

            Die();

        }
    }

    // coroutine to wait for a few seconds before destroying the enemy
    void Die()
    {
        isDead = true;

        agent.isStopped = true;
        enemyAnim.SetBool("MovingForward", false);
        // play the death sound
        enemyAnim.SetTrigger("Die");
        // play thee death anim
        enemyAudioSource.PlayOneShot(enemyDieSound);
        GetComponent<Collider>().enabled = false;
        GetComponent<NavMeshAgent>().enabled = false;
        miniMapIconEnemy.SetActive(false);
        EnemyManager.instance.enemyCount--;
        UI.instance.UpdateScore(score);
        UI.instance.UpdateNumberOfEnemies(EnemyManager.instance.enemyCount);

    }


}

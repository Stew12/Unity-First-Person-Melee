using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    int currentHealth;
    public int maxHealth;
    public float attackDamage;

    [Header("Movement")]
    public float roamSpeed;
    public float chaseSpeed;
    public float distanceFromPlayer;
    [SerializeField] private bool waiting;

    [Header("Attacking")]
    public float attackMoveTowardsSpeed;
    public float attackDistance;
    public float aggroAttackDistance;
    public bool enemyAttackProcess = false;
    public bool enemyAttacking = false;
    

    [Header("Components")]
    //public SphereCollider detectionRadius;
    private SpriteRenderer spriteRenderer;

    [Header("Knock Back")]
    private bool knockedBack;
    private Transform playerPos;
    public float knockBackSpeed = 3f;
    public float maxKnockBackTime = 0.45f;
    [SerializeField] float knockBackTime = 0;

    [Header("Timing")]
    private IEnumerator coroutine;
    public float attackWindUpTime = 0.85f;
    public float maxAttackCoolDownTime = 2f;
    private float attackCoolDownTime = 0;
    public float maxAttackDuration = 0.6f;
    [SerializeField]private float attackDuration = 0;
    public float maxWaitTime = 0.7f;
    [SerializeField]private float waitTime = 0;



    public enum EnemyState
    {
        ROAMING,
        CHASING,
        ATTACKING
    }

    [Header("State Machine")]
    public EnemyState enemyState;

    public enum EnemyAttackType
    {
        BASIC,
        RANGED
    }

    [Header("Attack list")]
    public EnemyAttackType enemyAttackType;

    void Awake()
    {
        currentHealth = maxHealth;
        enemyState = EnemyState.ROAMING;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!knockedBack)
        {
            if (!waiting)
            {
                //Get distance from the player (camera is used since the camera will always be in the centre of the player)
                distanceFromPlayer = Vector3.Distance(Camera.main.transform.position, transform.position);
                
                switch (enemyState)
                {
                    case EnemyState.ROAMING:

                    break;

                    case EnemyState.CHASING:

                        //When chasing, move towards player on X and Z axis
                        transform.position += new Vector3(transform.forward.x * chaseSpeed * Time.deltaTime, 0, transform.forward.z * chaseSpeed * Time.deltaTime); 

                        if (distanceFromPlayer <= aggroAttackDistance && attackCoolDownTime <= 0)
                        {
                            enemyState = EnemyState.ATTACKING;
                        }

                    break;

                    case EnemyState.ATTACKING:

                        if(!enemyAttackProcess)
                        {
                            if (attackCoolDownTime <= 0)
                            {
                                //Debug.Log("SKELLY ATK SETUP");
                                attackSetup();
                            }
                        }

                        attackDuration -= Time.deltaTime;

                        if (enemyAttacking)
                        {
                            if (attackDuration > 0)
                            {
                                //Debug.Log("SKELLY TIMER ON");
                                switch (enemyAttackType)
                                {
                                    case EnemyAttackType.BASIC:
                                        BasicAttack();
                                    break;

                                    case EnemyAttackType.RANGED:
                                        
                                    break;

                                    default:
                                        
                                    break;
                                }
                            }
                            else if (attackDuration <= 0)
                            {
                                //Debug.Log("SKELLY TIMER OFF");
                                //Reset to before attack
                                enemyAttacking = false;
                                enemyAttackProcess = false;
                                enemyState = EnemyState.CHASING;

                                attackCoolDownTime = maxAttackCoolDownTime;

                            }
                        }

                    break;

                    default:

                    break;
                }

                if (attackCoolDownTime > 0)
                {
                    attackCoolDownTime -= Time.deltaTime;
                }
            }
            else
            {
                if (waitTime > 0)
                {
                    waitTime -= Time.deltaTime;
                }
                else
                {
                    waiting = false;
                }
            }
        }
        else
        {
            if (knockBackTime > 0)
        {
            knockBackTime -= Time.deltaTime;

            transform.position += new Vector3(playerPos.forward.x * knockBackSpeed * Time.deltaTime, 0, playerPos.forward.z * knockBackSpeed * Time.deltaTime);
        }
        else
        {
            knockedBack = false;
        }
        }

    }

    public void Wait()
    {
        waitTime = maxWaitTime;
        waiting = true;
        attackDuration = 0;
    }

    public void Roam()
    {
        if (enemyState != EnemyState.ATTACKING)
        {
            enemyState = EnemyState.ROAMING;
        }
    }

    public void ChasePlayer()
    {
        if (enemyState != EnemyState.ATTACKING)
        {
            enemyState = EnemyState.CHASING;
        }
    }

    
    private void attackSetup()
    {
        spriteRenderer.color = Color.red;

        enemyAttackProcess = true;

        coroutine = ExecuteEnemyAttack(attackWindUpTime);
        StartCoroutine(coroutine);

    }

    private IEnumerator ExecuteEnemyAttack(float waitTime) 
    {
            yield return new WaitForSeconds(waitTime);

            enemyAttacking = true;
            spriteRenderer.color = Color.white;
            attackDuration = maxAttackDuration;
    }

    private void BasicAttack()
    {
        //Debug.Log("YU<");
        //the enemy moves quickly towards the player
        transform.position += new Vector3(transform.forward.x * attackMoveTowardsSpeed * Time.deltaTime, 0, transform.forward.z * attackMoveTowardsSpeed * Time.deltaTime); 
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if(currentHealth <= 0)
        { Death(); }
    }

    public void EnemyKnockBack(Transform playerPos)
    {
        this.playerPos = playerPos;
        knockedBack = true;
        knockBackTime = maxKnockBackTime;
    }

    void Death()
    {
        // Death function
        // TEMPORARY: Destroy Object
        Destroy(gameObject);
    }
}

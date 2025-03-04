using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public enum EnemyType
    {
        SKELETON,
        SORCERESS,
        WRAITH,
        IMP,
        MIMIC,
        SHADOW,
        TENTACLES,
        ZOMBIE
    }

    public enum EnemyState
    {
        ROAMING,
        CHASING,
        ATTACKING
    }

public class Enemy : MonoBehaviour
{
    [Header("Enemy Type")]
    public EnemyType enemyType;

    [Header("Stats")]
    int currentHealth;
    public int maxHealth;
    public float attackDamage;

    [Header("Movement")]
    public float roamSpeed;
    public float chaseSpeed;
    public float distanceFromPlayer;
    [SerializeField] private bool waiting;
    private bool isGrounded;
    Vector3 _EnemyVelocity;
    public float gravity = -9.8f;


    [Header("Attacking")]
    public float attackMoveTowardsSpeed;
    public float attackDistance;
    public float aggroAttackDistance;
    public bool enemyAttackProcess = false;
    public bool enemyAttacking = false;
    public bool canFireProjectile = true;
    public bool attackTrajectorySet = false;
    

    [Header("Components")]
    //public SphereCollider detectionRadius;
    [HideInInspector] public CharacterController enemyController;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private EnemyBehaviourAndAttackList enemyBehaviourAndAttackList;
    public GameObject enemyProjectile;
    public GameObject enemyAOE;

    [Header("Knock Back")]
    private bool knockedBack;
    private Transform playerPos;
    public float knockBackSpeed = 3f;
    public float maxParryKnockBackTime = 0.45f;
    [SerializeField] float knockBackTime = 0;

    [Header("Timing")]
    private IEnumerator coroutine;
    public float attackWindUpTime = 0.85f;
    public float maxAttackCoolDownTime = 2f;
    public float attackCoolDownTime = 0;
    public float maxAttackDuration = 0.6f;
    public float attackDuration = 0;
    public float maxWaitTime = 0.7f;
    [SerializeField]private float waitTime = 0;

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
        
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        
        enemyController = GetComponent<CharacterController>();

        enemyBehaviourAndAttackList = new EnemyBehaviourAndAttackList();
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

                       enemyBehaviourAndAttackList.ChaseBehaviourList(enemyType, this, gameObject);
                        
                    break;

                    case EnemyState.ATTACKING:

                        if(!enemyAttackProcess)
                        {
                            if (attackCoolDownTime <= 0)
                            {
                                attackSetup();
                            }
                        }

                        attackDuration -= Time.deltaTime;

                        if (enemyAttacking)
                        {
                            if (attackDuration > 0)
                            {
                                //ATTACK OCCURS HERE!
                                enemyBehaviourAndAttackList.AttackBehaviourList(enemyType, this, gameObject, enemyProjectile, enemyAOE);
                            }
                            else if (attackDuration <= 0)
                            {
                                //Reset to before attack
                                enemyAttacking = false;
                                enemyAttackProcess = false;
                                enemyState = EnemyState.CHASING;

                                attackTrajectorySet = false;

                                attackCoolDownTime = maxAttackCoolDownTime;

                                enemyBehaviourAndAttackList.attackChoice = -1;

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

                enemyController.Move(new Vector3(playerPos.forward.x * knockBackSpeed * Time.deltaTime, 0, playerPos.forward.z * knockBackSpeed * Time.deltaTime));
            }
            else
            {
                knockedBack = false;
            }
        }

        // Gravity
        _EnemyVelocity.y += gravity * Time.deltaTime;
            
        if (isGrounded && _EnemyVelocity.y < 0)
        {
           _EnemyVelocity.y = -2f;
        }
        enemyController.Move(_EnemyVelocity * Time.deltaTime);

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

    public void TakeDamage(int amount)
    {
        Debug.Log("DAMAGE: " + amount);

        currentHealth -= amount;

        if(currentHealth <= 0)
        { Death(); }
    }

    public void EnemyKnockBack(GameObject player, bool parried)
    {
        float wWeightDivFactor = 10;

        playerPos = player.transform;
        knockedBack = true;

        // Being parried has larger knockback than being damaged
        if (parried)
        {
            knockBackTime = maxParryKnockBackTime;
        }
        else
        {
            // Knockback depends on weight of the weapon used to hit enemy
            knockBackTime = player.GetComponent<PlayerController>().equippedWeapon.GetComponent<PlayerWeaponValues>().weaponWeight / wWeightDivFactor;
        }
        
    }

    void Death()
    {
        // Death function
        // TEMPORARY: Destroy Object
        Destroy(gameObject);
    }
}

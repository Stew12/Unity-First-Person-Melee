using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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

     [Header("Components")]
    //public SphereCollider detectionRadius;
    [HideInInspector] public CharacterController enemyController;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private EnemyBehaviourAndAttackList enemyBehaviourAndAttackList;
    public GameObject enemyProjectile;
    public GameObject projectileSpawn;
    public GameObject enemyWeaponAttack;
    public GameObject enemyAOE;
    [SerializeField] private GameObject enemyAttackWarningSource;
    [SerializeField] private GameObject enemyAttackWarningRadius;
    private GameObject attkWarning;
    private GameObject attkWarningRad;
    [SerializeField] private GameObject enemyDeathEffect;
    [SerializeField] private GameObject enemyHPBarBG;
    public CapsuleCollider mainHitbox;
    public CapsuleCollider[] weakPointHitboxes;
    private SpriteRenderer enemyHPBar;
    private TextMeshPro damageNumber;
    [HideInInspector] public AudioSource audioSource;

    [Header("Stats")]
    int currentHealth;
    public int maxHealth;
    public float attackDamage;
    
    [Header("Drops")]
    public int bronze = 3;

    [Header("Movement")]
    public float roamSpeed;
    public float chaseSpeed;
    public float distanceFromPlayer;
    [SerializeField] private bool waiting;
    private bool isGrounded;
    Vector3 _EnemyVelocity;
    public float gravity = -9.8f;

    [Header("Visual")]
    [SerializeField] private bool flipSpriteOnAttack = false;
    [SerializeField] private float slashOffset = 0.2f;
    [SerializeField] private float attackWarningSpriteScale = 1.1f;
    [SerializeField] private float attackWarningHeightUp = 0.05f;
    [SerializeField] private float attackWarningRadYOffset = 0.6f;

    [Header("Attacking")]
    public float attackMoveTowardsSpeed;
    public float attackDistance;
    public float aggroAttackDistance;
    public bool enemyAttackProcess = false;
    public bool enemyAttacking = false;
    public bool canFireProjectile = true;
    public bool attackTrajectorySet = false;

    [Header("Audio Clips")]
    public AudioClip enemyAlert;
    public AudioClip enemyAttack;
    [SerializeField] private AudioClip enemyHurt;
    [SerializeField] private AudioClip enemyDie;

    [Header("Knock Back")]
    private bool knockedBack;
    private Transform playerPos;
    public float knockBackSpeed = 3f;
    public float maxParryKnockBackTime = 0.45f;
    [SerializeField] private float knockBackTime = 0;

    [Header("Timing")]
    private IEnumerator coroutine;
    public float attackWindUpTime = 0.85f;
    public float maxAttackCoolDownTime = 2f;
    public float attackCoolDownTime = 0;
    public float maxAttackDuration = 0.6f;
    public float attackDuration = 0;
    public float maxWaitTime = 0.7f;
    [SerializeField] private float hpBarShowTime = 0.8f;
    [SerializeField]private float waitTime = 0;

    [Header("State Machine")]
    public EnemyState enemyState;

    void Awake()
    {
        currentHealth = maxHealth;
        enemyState = EnemyState.ROAMING;
        
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        // Set enemy HP bar, then hide it
        enemyHPBar = enemyHPBarBG.transform.GetChild(0).GetComponent<SpriteRenderer>();
        damageNumber = enemyHPBarBG.transform.GetChild(1).GetComponent<TextMeshPro>();
        enemyHPBarBG.SetActive(false);
        
        audioSource = GetComponent<AudioSource>();

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

                        EnemyAttack eAttack = EnemyAttack.BASICPHYSICAL;

                        eAttack = enemyBehaviourAndAttackList.selectEnemyAttack(enemyType);

                        if (!enemyAttackProcess)
                        {
                            if (attackCoolDownTime <= 0)
                            {
                                attackSetup(eAttack);
                            }
                        }

                        attackDuration -= Time.deltaTime;

                        if (enemyAttacking)
                        {
                            if (attackDuration > 0)
                            {
                                //ATTACK OCCURS HERE!
                                enemyBehaviourAndAttackList.AttackBehaviourList(eAttack, this, gameObject, enemyProjectile, enemyAOE, enemyWeaponAttack, slashOffset);

                                if (flipSpriteOnAttack)
                                {
                                    //Flip sprite to give better illusion of attacking
                                    GetComponent<SpriteRenderer>().flipX = true;
                                }
                            }
                            else if (attackDuration <= 0)
                            {
                                //Reset to before attack
                                enemyAttacking = false;
                                enemyAttackProcess = false;

                                if (flipSpriteOnAttack)
                                {
                                    GetComponent<SpriteRenderer>().flipX = false;
                                }
                                
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

    
    private void attackSetup(EnemyAttack EA)
    {
        createAttackWarning();

        if (EA == EnemyAttack.BASICAOE)
        {
            createAttackWarningRadius();
        }

        canFireProjectile = true;

        enemyAttackProcess = true;

        StartCoroutine(ExecuteEnemyAttack(attackWindUpTime));

    }

    private void createAttackWarning()
    {
        attkWarning = Instantiate(enemyAttackWarningSource, new Vector3(transform.position.x, transform.position.y + attackWarningHeightUp, transform.position.z), transform.rotation);

        if (GetComponent<SpriteRenderer>() != null)
        {
            attkWarning.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
        }
        else
        {
            attkWarning.GetComponent<SpriteRenderer>().sprite = spriteRenderer.sprite;
        }
        
        attkWarning.transform.localScale = transform.localScale * attackWarningSpriteScale;
    }

    private void createAttackWarningRadius()
    {
        attkWarningRad = Instantiate(enemyAttackWarningRadius, new Vector3(transform.position.x, transform.position.y - attackWarningRadYOffset, transform.position.z), Quaternion.Euler(90, 0, 0));
    }

    private IEnumerator ExecuteEnemyAttack(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        Destroy(attkWarning);

        if (attkWarningRad != null) Destroy(attkWarningRad);

        enemyAttacking = true;
        spriteRenderer.color = Color.white;
        attackDuration = maxAttackDuration;
    }

    public void TakeDamage(int amount, bool weakPointHit, float weakPointDamageFactor)
    {
        if (weakPointHit) { amount = (int)(amount * weakPointDamageFactor); }

        Debug.Log("DAMAGE: " + amount);

        currentHealth -= amount;

        // Enemy HP Bar
        enemyHPBarBG.SetActive(true);
        EnemyHPBarChange(amount, weakPointHit);
        coroutine = HideEnemyHPBar();
        StartCoroutine(coroutine);

        //Audio
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(enemyHurt);

        if(currentHealth <= 0)
        { 
            EnemyDeath();     
        }
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

    private void EnemyHPBarChange(float damageAmt, bool weakPointHit)
    {
        //Health bar elements
        enemyHPBar.transform.localScale = new Vector3((float)currentHealth / (float)maxHealth, enemyHPBar.transform.localScale.y, enemyHPBar.transform.localScale.z);
        damageNumber.text = damageAmt.ToString();

        if (weakPointHit)
        {
            /* YELLOW- weak point damage */
            damageNumber.color = Color.yellow;
        }
        else
        {
            /* WHITE- normal damage */
            damageNumber.color = Color.white;
        }
    }

    private IEnumerator HideEnemyHPBar()
    {
        yield return new WaitForSeconds(hpBarShowTime);

        enemyHPBarBG.SetActive(false);
    }

    void EnemyDeath()
    {
        //audioSource.pitch = 1;
        //audioSource.PlayOneShot(enemyDie);

        // Spawn enemy death object and destroy enemy object
        GameObject EDE = Instantiate(enemyDeathEffect, transform.position, Quaternion.identity);

        if (attkWarning != null) Destroy(attkWarning);
        if (attkWarningRad != null) Destroy(attkWarningRad);

        EDE.GetComponent<EnemyDeathEffect>().deathSound = enemyDie;
        EDE.GetComponent<EnemyDeathEffect>().deathEffectTime = enemyDie.length;
        EDE.GetComponent<EnemyDeathEffect>().coinAmount = bronze;
        
        Destroy(gameObject);
    }
}

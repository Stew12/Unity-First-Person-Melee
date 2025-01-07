using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using UnityEngine.Rendering;
using Unity.VisualScripting;
using TMPro;
using JetBrains.Annotations;

public class PlayerController : MonoBehaviour
{
    [Header("Input")]
    PlayerInput playerInput;
    PlayerInput.MainActions input;

    public GameObject equippedWeapon;

    [HideInInspector] public CharacterController controller;
    private AudioSource audioSource;
    public AudioSource footstepsAudioSource;

    [Header("Inventory")]
    public PlayerInventory playerInventory;

    [Header("UI")]
    public Image healthBarUI;
    public Image momentumBarUI;
    public Image dragonPointBarUI;
    public Image powerBarUI;
    public Image durabilityBarUI;
    public TextMeshProUGUI durabilityLabel;

    [Header("Controller")]
    public float moveSpeed = 2.5f;
    public float moveSpeedDefault;
    public float gravity = -9.8f;
    public float jumpHeight = 1.2f;

    Vector3 _PlayerVelocity;

    private bool isGrounded;

    [Header("Camera")]
    public Camera cam;
    bool cameraLocked = false;
    public float sensitivity ;
    float xRotation = 0f;

    /* Animation variables */
    [Header("Animation")]
    [HideInInspector] public Animator animator;
    [HideInInspector] public string IDLE = "Null";
    [HideInInspector] public string WALK = "Null";
    [HideInInspector] public string SWINGACROSS = "Sword Swing Across";
    //public const string SWINGDOWN = "Sword Swing Down";
    [HideInInspector] public string SWINGBACK = "Sword Swing Across Back";
    [HideInInspector] public string BLOCK = "Sword Block";

    string currentAnimationState;

    /* Attacking variables */
    [Header("Attacking")]
    public bool attacking = false;
    private bool readyToAttack = true;
    private int attackCount;    

    [Header("Effects")]
    public LayerMask attackLayer;
    public GameObject hitEffect;
    public AudioClip swordSwing;
    public AudioClip hitSound;

    [Header("Blocking/Parrying")]
    public GameObject blockAndParryHitbox;
    public bool blocking = false;

    [Header("Power Bar")]
    //public float power = 0;
    [SerializeField] private float powerTimeFactor = 5; //Times the attack delay
    [SerializeField] private float powerDamageFactor = 4;
    [SerializeField] private float powerBarSpeedupFactor = 1.2f;
    private bool attackPowerBuilding = false;

    [Header("Knock Back")]
    private bool knockedBack;
    private Transform attackingEntityPos;
    public float knockBackSpeed = 3f;

    /* Momentum bar variables */
    [Header("Momentum Bar")]
    [SerializeField] private bool boosting = false;
    public float currMomentumValue;
    public float maxMomentum = 1.2f;
    public float momentumIncrease = 0.1f;
    public float parryMomentumIncrease = 0.4f;
    private bool momentumDecreasing = false;
    public float maxTimeBeforeMomentumDecrease = 2f;
    private float timeBeforeMomentumDecrease;
    public float momentumDecreaseSpeed = 0.1f;

    [Header("Durability")]
    [SerializeField] private int weaponDurabilityLossHit = 1;
    public int weaponDurabilityLossBlock = 3;

    [Header("Dragon Spell")]
    [SerializeField] private DragonSpells dragonSpellSelected;
    public GameObject fireBall;

    [Header("Timing")]
    public float maxDontTakeDamageTime = 0.8f;
    private float dontTakeDamageTime = 0;
    public float maxKnockBackTime = 0.45f;
    private float knockBackTime = 0;
    public float maxParryWindowTime = 0.3f;
    [SerializeField] private float maxPowerTime = 0;
    [SerializeField] private float powerTime = 0;
    [HideInInspector] public float parryWindowTime = 0;

    [Header("Debug")]

    public bool stopWhenAttacking = false;

    void Awake()
    { 
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();

        playerInput = new PlayerInput();
        input = playerInput.Main;
        AssignInputs();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        healthBarUI.fillAmount = 1;
        momentumBarUI.fillAmount = 0;
        dragonPointBarUI.fillAmount = 1;
        powerBarUI.fillAmount = 0;

        moveSpeedDefault = moveSpeed;
       // animator.speed += 2;

       blockAndParryHitbox.SetActive(false);
       GetComponent<PlayerCollisions>().hurtFlash.enabled = false;

       // Set animations for equipped weapon
       GetComponent<PlayerAnimation>().WeaponAnimationChange(equippedWeapon.GetComponent<PlayerWeaponValues>().weaponClass, this);

       NewLevelLoad();
    }

    void Update()
    {
        isGrounded = controller.isGrounded;
        SetAnimations();

        if (dontTakeDamageTime > 0)
        {
            dontTakeDamageTime -= Time.deltaTime;
        }
        else
        {
            GetComponent<PlayerCollisions>().canTakeDamage = true;
        }

        // Knock player backwards
        if (knockBackTime > 0)
        {
            knockBackTime -= Time.deltaTime;

            if (attackingEntityPos != null)
            {
                //transform.position += new Vector3(attackingEntityPos.forward.x * knockBackSpeed * Time.deltaTime, 0, attackingEntityPos.forward.z * knockBackSpeed * Time.deltaTime);
                controller.Move(new Vector3(attackingEntityPos.forward.x * knockBackSpeed * Time.deltaTime, 0, attackingEntityPos.forward.z * knockBackSpeed * Time.deltaTime));
            }
        }
        else
        {
            knockedBack = false;
        }

        if (parryWindowTime > 0)
        {
            parryWindowTime -= Time.deltaTime;
        }

    }

    void FixedUpdate() 
    { 
        MoveInput(input.Movement.ReadValue<Vector2>());
    }

    void LateUpdate() 
    { 
        LookInput(input.Look.ReadValue<Vector2>()); 

        healthBarUI.fillAmount = (float)GetComponent<PlayerValues>().currentHealth / (float)GetComponent<PlayerValues>().maxHealth;

        momentumBarUI.fillAmount = currMomentumValue / maxMomentum;

        dragonPointBarUI.fillAmount = (float)GetComponent<PlayerValues>().currentDragonPoints / (float)GetComponent<PlayerValues>().maxDragonPoints;

        if (attackPowerBuilding && powerTime < maxPowerTime && !attacking)
        {
            powerTime += Time.deltaTime;
            
            powerBarUI.fillAmount = powerTime / maxPowerTime;
        }
        
        // If durability on weapon has run out, break the weapon to a weaker version
        if (equippedWeapon.GetComponent<PlayerWeaponValues>().currentWeaponDurability <= 0)
        {
            equippedWeapon.GetComponent<PlayerWeaponValues>().currentWeaponDurability = 0;

            equippedWeapon.GetComponent<PlayerWeaponValues>().NoWeaponDurability();
        }

        // Show weapon durability in UI
        durabilityBarUI.fillAmount = equippedWeapon.GetComponent<PlayerWeaponValues>().currentWeaponDurability / equippedWeapon.GetComponent<PlayerWeaponValues>().maxWeaponDurability;
        durabilityLabel.text = equippedWeapon.GetComponent<PlayerWeaponValues>().currentWeaponDurability.ToString();


        if (boosting)
        {
            timeBeforeMomentumDecrease -= Time.deltaTime;

            if (timeBeforeMomentumDecrease <= 0)
            {
                if (currMomentumValue >= 0)
                {
                    currMomentumValue -= Time.deltaTime * momentumDecreaseSpeed;
                    
                    if (equippedWeapon.GetComponent<PlayerWeaponValues>().weaponAttackDelay < equippedWeapon.GetComponent<PlayerWeaponValues>().weaponAttackDelayDefault)
                        equippedWeapon.GetComponent<PlayerWeaponValues>().weaponAttackDelay += Time.deltaTime * momentumDecreaseSpeed;
                    
                    if (moveSpeed > moveSpeedDefault)
                    {
                        moveSpeed -= Time.deltaTime * (2 * momentumDecreaseSpeed);
                    }
                }
                else
                {
                    currMomentumValue = 0;

                    //Boosting deactivates when momentum reaches 0
                    boosting = false;
                }
            }
        } 

        if (blocking)
        {
            blockAndParryHitbox.SetActive(true);
        }
        else
        {
            blockAndParryHitbox.SetActive(false);
        }

    }

    void MoveInput(Vector2 input)
    {
        if ((!attacking || !stopWhenAttacking) && !knockedBack)
        {
            Vector3 moveDirection = Vector3.zero;
            moveDirection.x = input.x;
            moveDirection.z = input.y;

            if (!attacking)
            {
                controller.Move(transform.TransformDirection(moveDirection) * moveSpeed * Time.deltaTime);
            }
            else
            {
                //When attacking, slow the player down based on the weight of their weapon. This will also be affected by momentum guage.
                controller.Move(transform.TransformDirection(moveDirection) * (moveSpeed / equippedWeapon.GetComponent<PlayerWeaponValues>().weaponWeight) * Time.deltaTime);
            }

            _PlayerVelocity.y += gravity * Time.deltaTime;

            if (isGrounded && _PlayerVelocity.y < 0)
            {
                _PlayerVelocity.y = -2f;
            }
            controller.Move(_PlayerVelocity * Time.deltaTime);

            // Footsteps noise- only play when moving!
            if (moveDirection != Vector3.zero)
            {
                if (!footstepsAudioSource.isPlaying)
                {
                    // Change pitch based on movespeed
                    footstepsAudioSource.pitch = moveSpeed / 2.5f;
                    
                    footstepsAudioSource.Play();
                }
            }
           
        }
    }

    void LookInput(Vector3 input)
    {
        float mouseX = input.x;
        float mouseY = input.y;

        xRotation -= (mouseY * Time.deltaTime * sensitivity);
        xRotation = Mathf.Clamp(xRotation, -80, 80);

        if (!cameraLocked)
        {
            cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        }

            transform.Rotate(Vector3.up * (mouseX * Time.deltaTime * sensitivity));
        
    }

    void OnEnable() 
    { input.Enable(); }

    void OnDisable()
    { input.Disable(); }

    void AssignInputs()
    {
        input.Jump.performed += ctx => Jump();
        //TODO: decide if 'performed' is better (holding down attack instead of clicking)
        input.Attack.started += ctx => Attack();
        input.Block.started += ctx => Block();
        input.Cast.performed += ctx => Cast();
        input.Boost.performed += ctx => Boost();

        input._1.performed += ctx => ItemSwitch(1);
        input._2.performed += ctx => ItemSwitch(2);
        input._3.performed += ctx => ItemSwitch(3);
        input._4.performed += ctx => ItemSwitch(4);
        input._5.performed += ctx => ItemSwitch(5);
        input._6.performed += ctx => ItemSwitch(6);
        input._7.performed += ctx => ItemSwitch(7);
        input._8.performed += ctx => ItemSwitch(8);
        input._9.performed += ctx => ItemSwitch(9);
    }

    private void Jump()
    {
        // Adds force to the player rigidbody to jump
        if (isGrounded)
            _PlayerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
    }

    // ---------- //
    // ANIMATIONS //
    // ---------- //

    public void ChangeAnimationState(string newState) 
    {
        // STOP THE SAME ANIMATION FROM INTERRUPTING WITH ITSELF //
        if (currentAnimationState == newState) return;

        // PLAY THE ANIMATION //
        currentAnimationState = newState;
        animator.CrossFadeInFixedTime(currentAnimationState, 0.2f);
    }

    void SetAnimations()
    {
        // If player is not attacking
        if(!attacking && !blocking)
        {
            if (_PlayerVelocity.x == 0 &&_PlayerVelocity.z == 0)
            { 
                ChangeAnimationState(IDLE); 
            }
            else
            { 
                ChangeAnimationState(WALK); 
            }
        }
    }

    // ------------------- //
    // ATTACKING BEHAVIOUR //
    // ------------------- //

    public void Attack()
    {
        if(!readyToAttack || attacking) return;
        
        readyToAttack = false;
        attacking = true;

        blocking = false;

        Invoke(nameof(ResetAttack), equippedWeapon.GetComponent<PlayerWeaponValues>().weaponAttackDelay);
        Invoke(nameof(AttackRaycast), equippedWeapon.GetComponent<PlayerWeaponValues>().weaponAttackSpeed);

        audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(swordSwing);

        if(attackCount == 0)
        {
            ChangeAnimationState(SWINGACROSS);
            attackCount++;
        }
        else
        {
            ChangeAnimationState(SWINGBACK);
            attackCount = 0;
        }
    }

    void ResetAttack()
    {
        attacking = false;
        readyToAttack = true;

        // Make power bar start to go up
        attackPowerBuilding = true;
        maxPowerTime = equippedWeapon.GetComponent<PlayerWeaponValues>().weaponAttackDelay * powerTimeFactor;

        //If boosting, the max power time will decrease (bar moves faster)
        if (boosting)
        {
            maxPowerTime /= moveSpeed / powerBarSpeedupFactor;
        }

        powerTime = 0;
    }

    void AttackRaycast()
    {
        //GameObject weapon = equippedWeapon.transform.parent.gameObject;
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, equippedWeapon.GetComponent<PlayerWeaponValues>().weaponAttackDistance, attackLayer))
        { 
            HitTarget(hit.point);

            /* Enemy hit by melee */
            if(hit.transform.TryGetComponent<Enemy>(out Enemy T))
            { 
                //Power damage multiplier
                float powerMultiplier;

                //Damage depends on how full the power bar is
                powerMultiplier = (1 + powerBarUI.fillAmount) * powerDamageFactor;

                T.TakeDamage((int)(equippedWeapon.GetComponent<PlayerWeaponValues>().weaponAttackDamage * powerMultiplier)); 

                // Knock back enemy slightly if enemy is not currently attacking
                if (!T.GetComponent<Enemy>().enemyAttackProcess)
                {
                    T.GetComponent<Enemy>().EnemyKnockBack(gameObject, false);
                }

                //Reduce durability on weapon
                equippedWeapon.GetComponent<PlayerWeaponValues>().currentWeaponDurability -= weaponDurabilityLossHit;

                /* Momentum increases upon hitting an enemy */
                MomentumIncrease(false);
            }
        } 
    }

    void HitTarget(Vector3 pos)
    {
        audioSource.pitch = 1;
        audioSource.PlayOneShot(hitSound);

        GameObject GO = Instantiate(hitEffect, pos, Quaternion.identity);
        Destroy(GO, 20);
    }

    void Block()
    {
        if (!blocking)
        {
            blocking = true;

            parryWindowTime = maxParryWindowTime;

            ChangeAnimationState(BLOCK);
        }
        else
        {
            blocking = false;
        }
    }

    public void StopBlocking()
    {
        ChangeAnimationState(IDLE);
        blocking = false;
    }

    private void Cast()
    {
        //TODO spell casted depends on which spell is currently selected

        if (GetComponent<PlayerValues>().currentDragonPoints > 0)
        {
            GetComponent<PlayerDragonSpellList>().PrepareDragonSpell(dragonSpellSelected, this, GetComponent<PlayerValues>());
        }
        else
        {
            //TODO: Notify player they cannot cast the spell e.g. a sound effect
        }
        
    }

    private void Boost()
    {
        if (currMomentumValue > 0)
        {
            boosting = true;

            equippedWeapon.GetComponent<PlayerWeaponValues>().weaponAttackDelay -= currMomentumValue;
            moveSpeed += 2 * currMomentumValue;
        }
    }

    public void KnockBack(Transform attackingEntityPos)
    {
        //TODO: make it so that knocking back player doesn't send them into a wall

        this.attackingEntityPos = attackingEntityPos;
        knockedBack = true;
        knockBackTime = maxKnockBackTime;
    }

    public void DontTakeDamage()
    {
        dontTakeDamageTime = maxDontTakeDamageTime;
    }

    public void MomentumIncrease(bool parryIncrease)
    {
        //timeBeforeMomentumDecrease = maxTimeBeforeMomentumDecrease;

        //momentumDecreasing = false;
        
        if (currMomentumValue < maxMomentum)
        {
            //NEW CHANGE: momentum only increases when not boosting, and the effects of momentum only kick in when boosting.
            if (!boosting)
            {
                if (!parryIncrease)
                {
                    // If the power bar is higher, gain slightly more momentum
                    currMomentumValue += momentumIncrease + (powerBarUI.fillAmount / 10);
                }
                else
                {
                    currMomentumValue += parryMomentumIncrease;
                }
            }
        }
    }

    void ItemSwitch(int hotKeyNumber)
    {
        playerInventory.HotKeyedItem(hotKeyNumber);
    }

    // TODO: use this method upon entering each new scene (Temporarily called in Awake())
    void NewLevelLoad()
    {
        foreach (GameObject eWall in GameObject.FindGameObjectsWithTag("Enemy Wall"))
        {
            Physics.IgnoreCollision(GetComponent<PlayerController>().controller, eWall.GetComponent<Collider>());
        }
    }

    
}
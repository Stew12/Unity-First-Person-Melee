using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using UnityEngine.Rendering;
using Unity.VisualScripting;
using TMPro;
using UnityEngine.SceneManagement;
using JetBrains.Annotations;
using UnityEditor.Rendering;

public class PlayerController : MonoBehaviour
{
    [Header("Debug")]
    private bool restarting1 = false;
    private bool restarting2 = false;
    public bool stopWhenAttacking = false;
    [SerializeField] private bool noLookInputModeControls = false;
    [SerializeField] private bool crossHairAttackChange = false;

    [Header("Input")]
    PlayerInput playerInput;
    PlayerInput.MainActions input;

    public GameObject equippedWeapon;
    public GameObject startingWeaponPickup;

    [HideInInspector] public CharacterController controller;
    [HideInInspector] public AudioSource audioSource;
    public AudioSource footstepsAudioSource;

    [Header("Inventory")]
    public PlayerInventory playerInventory;

    [Header("UI")]
    public Image healthBarUI;
    public Image momentumBarUI;
    public Image dragonPointBarUI;
    public Image powerBarUI;
    public Image durabilityBarUI;
    [SerializeField] private Image crosshair;
    public Sprite crosshairDefault;
    public Sprite crosshairCanAttack;
    public TextMeshProUGUI durabilityLabel;
    public TextMeshProUGUI bronzeAmountLabel;
    public TextMeshProUGUI statusMessage;
    [SerializeField] private TextMeshProUGUI pausedText;
    public string lockedDoorStatusMessage = "Locked...";
    //public string lockedOpenDoorStatusMessage = "Can't be moved.";
    public string openDoorStatusMessage = "You heard something moving.";

    [Header("Controller")]
    public float moveSpeed = 2.5f;
    public float moveSpeedDefault;
    [SerializeField] private float moveSpeedSheathedFactor = 2;
    [SerializeField] private float turnSpeed = 20f;
    public float gravity = -9.8f;
    [SerializeField] private float jumpHeight = 1.2f;
    [SerializeField] private float jumpHeightSheathedFactor = 2f;
    [SerializeField] private float interactRaycastDistance = 2.5f;
    [SerializeField] private bool noLookInputMode = false;
    private bool strafeLeft = false;
    private bool strafing = false;
    [SerializeField] private float horizDeadzone = 0.35f;

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
    [SerializeField] private GameObject noWeaponHand;

    string currentAnimationState;

    /* Attacking variables */
    [Header("Attacking")]
    [HideInInspector] public bool attacking = false;
    private bool readyToAttack = true;
    private int attackCount;   
    [HideInInspector] public bool weaponSheathed = false;
    public float weakPointDamageFactor = 1.35f;
    private bool inAttackRange = false;

    [Header("Lighting")]
    [SerializeField] private GameObject lantern;

    [Header("Effects")]
    public LayerMask attackLayer;
    public GameObject hitEffect;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip swordSwing;
    [SerializeField] private AudioClip wallHitSound;
    [SerializeField] private AudioClip enemyHitSound;
    [SerializeField] private AudioClip unsheatheSound;
    [SerializeField] private AudioClip sheatheSound;
    public AudioClip coinPickupSound;
    public AudioClip parrySound;
    public AudioClip hurtSound;
    public AudioClip blockSound;

    [Header("Blocking/Parrying/Backstepping")]
    public GameObject blockAndParryHitbox;
    public bool blocking = false;
    private bool backstepping = false;
    [SerializeField] private float backstepSpeed = 5f;

    [Header("Power Bar")]
    //public float power = 0;
    [SerializeField] private float powerTimeFactor = 5; //Times the attack delay
    [SerializeField] private float powerDamageFactor = 4;
    [SerializeField] private float powerBarSpeedupFactor = 1.2f;
    public bool attackPowerBuilding = true;

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

    [Header("Dialog")]
    public TextCrawl dialogueTextBox;
    private NPC speakingNPC;

    [Header("Dragon Spells")]
    public DragonSpells dragonSpellSelected;
    public GameObject fireBall;
    public GameObject thunderTarget;
    public GameObject thunder;

    [Header("Timing")]

    // Waiting - player can't input anything. Paused - player cannot do anything 
    [HideInInspector] public bool waiting = false;
    private bool paused = false;
    
    public float maxDontTakeDamageTime = 0.8f;
    private float dontTakeDamageTime = 0;
    public float maxKnockBackTime = 0.45f;
    private float knockBackTime = 0;
    public float maxParryWindowTime = 0.3f;
    [SerializeField] private float backstepTime = 0.3f;
    [SerializeField] private float maxPowerTime = 0;
    [SerializeField] private float powerTime = 0;
    [HideInInspector] public float parryWindowTime = 0;
    [SerializeField] private float statusMessageVisibleTime = 1;   

    [Header("Currency")]
    [SerializeField] private float coinToBronzeFactor = 10;

    void Awake()
    {
       //DontDestroyOnLoad(gameObject);

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

        statusMessage.gameObject.SetActive(false);

        moveSpeedDefault = moveSpeed;
        // animator.speed += 2;

        //Set required elements to inactive
        blockAndParryHitbox.SetActive(false);
        GetComponent<PlayerCollisions>().hurtFlash.enabled = false;
        dialogueTextBox.transform.parent.gameObject.SetActive(false);
        pausedText.gameObject.SetActive(false);
        noWeaponHand.SetActive(false);

        // Set animations for equipped weapon
        GetComponent<PlayerAnimation>().WeaponAnimationChange(equippedWeapon.GetComponent<PlayerWeaponValues>().weaponClass, this);

        maxPowerTime = equippedWeapon.GetComponent<PlayerWeaponValues>().weaponAttackDelay * powerTimeFactor;

        NewLevelLoad();
    }

    void AssignInputs()
    {
        input.Jump.performed += ctx => Jump();
        input.Attack.started += ctx => Attack();
        input.Block.started += ctx => Block();
        input.Cast.started += ctx => StartCast();
        input.Cast.canceled += ctx => StopCast();
        input.Boost.performed += ctx => Boost();
        input.Backstep.started += ctx => Backstep();
        input.Interact.performed += ctx => Interact();
        input.LanternToggle.performed += ctx => LanternToggle();
        input.Sheathe.performed += ctx => SheatheWeaponToggle();
        input.ItemQuickSelect.started += ctx => ItemQuickSelect();
        input.ItemQuickSelect.canceled += ctx => ItemStopSelecting();
        input.OpenItemInfo.performed += ctx => ShowItemInfo();
        input.SelectOptionNextDialog.performed += ctx => SelectOptionOrNextDialog();

        input.Pause.performed += ctx => PauseToggle(false);
        input.Inventory.performed += ctx => InventoryToggle();
        input.EquipItem.performed += ctx => EquipItem();
        input.InventorySelectUp.performed += ctx => InventorySelect(InventoryDir.UP);
        input.InventorySelectDown.performed += ctx => InventorySelect(InventoryDir.DOWN);
        input.InventorySelectLeft.performed += ctx => InventorySelect(InventoryDir.LEFT);
        input.InventorySelectRight.performed += ctx => InventorySelect(InventoryDir.RIGHT);

        input.InventoryChangeTabLeft.performed += ctx => InventoryTabChange(InventoryDir.LEFT);
        input.InventoryChangeTabRight.performed += ctx => InventoryTabChange(InventoryDir.RIGHT);

        input._1.performed += ctx => ItemSwitch(1);
        input._2.performed += ctx => ItemSwitch(2);
        input._3.performed += ctx => ItemSwitch(3);
        input._4.performed += ctx => ItemSwitch(4);
        input._5.performed += ctx => ItemSwitch(5);
        input._6.performed += ctx => ItemSwitch(6);
        input._7.performed += ctx => ItemSwitch(7);
        input._8.performed += ctx => ItemSwitch(8);
        input._9.performed += ctx => ItemSwitch(9);

        input.RestartButton1.started += ctx => Restart1(true);
        input.RestartButton1.canceled += ctx => Restart1(false);
        input.RestartButton1.started += ctx => Restart2(true);
        input.RestartButton1.canceled += ctx => Restart2(false);
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

        // Constantly check to see if player is in range of something, e.g an enemy or an interactable.
        Invoke(nameof(InRangeRaycast), 0);

        // Handles movement for knock back when knocking player back
        if (knockBackTime > 0)
        {
            knockBackTime -= Time.deltaTime;

            if (attackingEntityPos != null)
            {
                //transform.position += new Vector3(attackingEntityPos.forward.x * knockBackSpeed * Time.deltaTime, 0, attackingEntityPos.forward.z * knockBackSpeed * Time.deltaTime);
                controller.Move(new Vector3(-attackingEntityPos.forward.x * knockBackSpeed * Time.deltaTime, 0, -attackingEntityPos.forward.z * knockBackSpeed * Time.deltaTime));
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

        if (backstepping)
        {
            controller.Move(new Vector3(-transform.forward.x * backstepSpeed * Time.deltaTime, 0, -transform.forward.z * backstepSpeed * Time.deltaTime));
        }
    }

    void FixedUpdate() 
    { 
        MoveInput(input.Movement.ReadValue<Vector2>());
    }

    void LateUpdate() 
    {
        if (!noLookInputMode)
        {
            LookInput(input.Look.ReadValue<Vector2>()); 
        }

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
        durabilityBarUI.fillAmount = (float)equippedWeapon.GetComponent<PlayerWeaponValues>().currentWeaponDurability / (float)equippedWeapon.GetComponent<PlayerWeaponValues>().maxWeaponDurability;
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

        //Make values not go up while 'waiting' (in a dialog box, inventory, etc)
        // if (waiting)
        // {
        //     attackPowerBuilding = false;
        // }
        // else
        // {
        //     attackPowerBuilding = true;
        // }

    }

    void MoveInput(Vector2 input)
    {
        //NoLookInputMode
        if(noLookInputModeControls)
        {
            if (input == Vector2.zero || strafing)
                noLookInputMode = false;
            else
                noLookInputMode = true;
        }

        // if (strafing)
        // {
        //     if (strafeLeft)
        //         input.x = -1;
        //     else
        //         input.x = 1;
        // }

        // Only move if these conditions are met
        if ((!attacking || !stopWhenAttacking) && !knockedBack && !waiting)
        {

            Vector3 moveDirection = Vector3.zero;
            moveDirection.x = input.x;
            moveDirection.z = input.y;

            if (!attacking)
            {
                PlayerMove(input, moveDirection, moveSpeed * Time.deltaTime);
            }
            else
            {
                 //When attacking, slow the player down based on the weight of their weapon. This will also be affected by momentum guage.
                PlayerMove(input, moveDirection, (moveSpeed / equippedWeapon.GetComponent<PlayerWeaponValues>().weaponWeight) * Time.deltaTime);
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

    // private void StrafeMovement(bool strafeLeft, bool strafing)
    // {
    //     this.strafeLeft = strafeLeft;
    //     this.strafing = strafing;
    // }

    private void PlayerMove(Vector2 input, Vector3 moveDir, float totalMovementSpeed)
    {
        //Debug.Log("VEC 2: " + input);

        // Turn left
        // if (input.x < -horizDeadzone && input.x >= -1 && !strafing)
        // {
        //     turnInput(input);
        // }
        // //Turn right
        // else if (input.x > horizDeadzone && input.x <= 1 && !strafing)
        // {
        //     turnInput(input);
        // }
        
        // else
        // {
            controller.Move(transform.TransformDirection(moveDir) * totalMovementSpeed);
        //}
        

    }

    private void LookInput(Vector3 input)
    {
        if (!waiting)
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
        
    }

    // private void turnInput(Vector2 input)
    // {
    //     float mouseX = input.x;
    //     float mouseY = input.y;

    //     xRotation -= (mouseY * Time.deltaTime * sensitivity);
    //     xRotation = Mathf.Clamp(xRotation, -80, 80);

    //     if (!cameraLocked)
    //     {
    //         cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
    //     }

    //     transform.Rotate(Vector3.up * (mouseX * Time.deltaTime * turnSpeed));
    // }

    void OnEnable() 
    { input.Enable(); }

    void OnDisable()
    { input.Disable(); }

    private void Jump()
    {
        // Adds force to the player rigidbody to jump
        if (isGrounded && !waiting)
        {
            _PlayerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
        }

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
        if(!readyToAttack || attacking || waiting || weaponSheathed) return;
        
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

    public void ResetAttack()
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

    private void AttackRaycast()
    {
        //GameObject weapon = equippedWeapon.transform.parent.gameObject;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, equippedWeapon.GetComponent<PlayerWeaponValues>().weaponAttackDistance, attackLayer))
        {
            HitTarget(hit.point);

            /* Weakness hit*/
            if (hit.transform.TryGetComponent<EnemyWeakPointGameObject>(out EnemyWeakPointGameObject enemyWeakPointGameObject))
            {
                PlayerHitEnemy(hit, enemyWeakPointGameObject.parentEnemy, true);
            }
            /* Enemy hit by melee */
            else if (hit.transform.TryGetComponent<Enemy>(out Enemy enemy))
            {
                PlayerHitEnemy(hit, enemy, false);
            }
            else
            {
                audioSource.pitch = 1;
                audioSource.PlayOneShot(wallHitSound);
            }
        }
    }
    
    private void InRangeRaycast()
    {
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, equippedWeapon.GetComponent<PlayerWeaponValues>().weaponAttackDistance, attackLayer))
        {
            if ((hit.collider.gameObject.tag == "Enemy" || hit.collider.gameObject.tag == "Weak Point") && crossHairAttackChange)
                crosshair.sprite = crosshairCanAttack;
        }
        else
        {
            crosshair.sprite = crosshairDefault;
        }
    }

    void HitTarget(Vector3 pos)
    {
        // Create hit particle effect
        GameObject GO = Instantiate(hitEffect, pos, Quaternion.identity);
        Destroy(GO, 20);
    }

    private void PlayerHitEnemy(RaycastHit hit, Enemy enemy, bool weakPoint)
    {
        if (hit.collider == enemy.mainHitbox || weakPoint)
            {
                Debug.Log(weakPoint);
                //Power damage multiplier
                //float powerMultiplier;

                //Damage depends on how full the power bar is
                //powerMultiplier = (1 + powerBarUI.fillAmount) * powerDamageFactor;

                enemy.TakeDamage((int)(equippedWeapon.GetComponent<PlayerWeaponValues>().weaponAttackDamage * powerBarUI.fillAmount * powerDamageFactor), weakPoint, weakPointDamageFactor); 

                // Knock back enemy slightly if enemy is not currently attacking
                if (!enemy.GetComponent<Enemy>().enemyAttackProcess)
                {
                    enemy.GetComponent<Enemy>().EnemyKnockBack(gameObject, false);
                }

                //Reduce durability on weapon
                equippedWeapon.GetComponent<PlayerWeaponValues>().currentWeaponDurability -= weaponDurabilityLossHit;

                /* Momentum increases upon hitting an enemy */
                MomentumIncrease(false);

                audioSource.pitch = 1;
                audioSource.PlayOneShot(enemyHitSound);
            }
    }

    private void Block()
    {
        if (!waiting && !weaponSheathed)
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
                GetComponent<PlayerCollisions>().attackParried = false;
            }
        }
    }

    private void Backstep()
    {
        waiting = true;

        StartCoroutine(BackstepAction(backstepTime));
    }

    private IEnumerator BackstepAction(float waitTime)
    {
        backstepping = true;

        yield return new WaitForSeconds(waitTime);

        backstepping = false;
        waiting = false;
    }

    public void StopBlocking()
    {
        ChangeAnimationState(IDLE);
        blocking = false;
    }

    private void StartCast()
    {
        //TODO spell casted depends on which spell is currently selected
        if (!waiting)
        {
            playerInventory.GetComponent<PlayerInventory>().HUDSpell.GetComponent<QuickSelect>().StartFillUseCircle(this);
        }
        
    }

    private void StopCast()
    {
        playerInventory.GetComponent<PlayerInventory>().HUDSpell.GetComponent<QuickSelect>().StopFillUseCircle();
    }

    public void CastDragonSpell()
    {
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
        // Can only boost when weapon is out
        if (!waiting && !weaponSheathed && !boosting)
        {
            if (currMomentumValue > 0)
            {
                boosting = true;

                equippedWeapon.GetComponent<PlayerWeaponValues>().weaponAttackDelay -= currMomentumValue;
                moveSpeed += 2 * currMomentumValue;

                maxPowerTime /= moveSpeed / powerBarSpeedupFactor;

            }
        }
    }

    private void SheatheWeaponToggle()
    {
        audioSource.pitch = 1;

        // Can't sheathe or unsheathe whilst boosting
        if (!boosting && !waiting && !attacking)
        {
            if (!weaponSheathed)
            {
                //Sheathe weapon
                noWeaponHand.SetActive(true);
                equippedWeapon.SetActive(false);

                weaponSheathed = true;

                // Increase movement speed when sheathed
                moveSpeed *= moveSpeedSheathedFactor;

                // Increase jump height when sheathed
                jumpHeight *= jumpHeightSheathedFactor;

                //Reset power time and prevent it from increasing
                attackPowerBuilding = false;
                powerTime = 0;
                powerBarUI.fillAmount = 0;

                audioSource.PlayOneShot(sheatheSound);
            }
            else
            {
                //Unsheathe weapon
                noWeaponHand.SetActive(false);
                equippedWeapon.SetActive(true);

                weaponSheathed = false;

                // Return movement speed to default
                moveSpeed /= moveSpeedSheathedFactor;

                // Return jump height to default
                jumpHeight /= jumpHeightSheathedFactor;

                //Return power time increasing
                attackPowerBuilding = true;

                audioSource.PlayOneShot(unsheatheSound);
            }
        }
    }

    private void LanternToggle()
    {
        if (lantern.activeSelf)
        {
            lantern.SetActive(false);
        }
        else
        {
            lantern.SetActive(true);
        }
    }

    private void ItemQuickSelect()
    {
        if (!waiting)
        {
            playerInventory.GetComponent<PlayerInventory>().HUDItem.GetComponent<QuickSelect>().StartFillUseCircle(this);
        }
    }

    private void ItemStopSelecting()
    {
            playerInventory.GetComponent<PlayerInventory>().HUDItem.GetComponent<QuickSelect>().StopFillUseCircle();
    }

    private void WeaponDestructionAttack()
    {

    }

    private void Interact()
    {
        if (!waiting)
            Invoke(nameof(InteractRaycast), equippedWeapon.GetComponent<PlayerWeaponValues>().weaponAttackSpeed);
        //else if (playerInventory.enabled)
        //{
            //playerInventory.ItemIsSelected(null, null);
        //}
    }

    void InteractRaycast()
    {
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, interactRaycastDistance, attackLayer))
        {
            /* If NPC interacted */
            if (hit.transform.TryGetComponent<NPC>(out NPC N))
            {
                waiting = true;
                Cursor.lockState = CursorLockMode.None;
                dialogueTextBox.transform.parent.gameObject.SetActive(true);

                speakingNPC = N.GetComponent<NPC>();
                speakingNPC.PlayDialogue(dialogueTextBox);
                
            }

            /* If door interacted */
            if (hit.transform.tag == "Door")
            {
                if (hit.transform.parent.GetComponent<Door>() != null)
                {
                    //Check if door unlocked
                    if (!hit.transform.parent.GetComponent<Door>().locked)
                    {
                        hit.transform.parent.GetComponent<Door>().DoorOpenOrClose(hit.transform.gameObject.GetComponent<BoxCollider>());
                    }
                    else
                    {
                        if (hit.transform.parent.GetComponent<Door>().closed)
                        {
                            StatusMessageShow(lockedDoorStatusMessage);
                        }
                    }
                }
                else
                {
                    //Check if door unlocked
                    if (!hit.transform.parent.parent.GetComponent<Door>().locked)
                    {
                        hit.transform.parent.parent.GetComponent<Door>().DoorOpenOrClose(hit.transform.gameObject.GetComponent<BoxCollider>());
                    }
                    else
                    {
                        if (hit.transform.parent.parent.GetComponent<Door>().closed)
                        {
                            StatusMessageShow(lockedDoorStatusMessage);
                        }
                    }
                }
            }
            else if (hit.transform.tag == "Player Item")
            {
                playerInventory.AddToInventory(hit.transform.gameObject);
            }
            else if (hit.transform.tag == "Chest")
            {
                hit.transform.GetComponent<TreasureChest>().ChestOpen();
            }
            else if (hit.transform.tag == "Button")
            {
                hit.transform.parent.GetComponent<DungeonButton>().ButtonActivation(this);
            }
        } 
    }

    void SelectOptionOrNextDialog()
    {
        if (dialogueTextBox.isActiveAndEnabled)
        {
            //If text is still crawling, display the entire message at once
            if (!dialogueTextBox.boxFinished)
            {
                dialogueTextBox.showAllText = true;
                dialogueTextBox.boxFinished = true;
            }
            else
            {
                //TODO go to next dialog box if available
                if (speakingNPC.dialogueBoxIndex < speakingNPC.dialogue.Count - 1)
                {
                    speakingNPC.dialogueBoxIndex++;
                    speakingNPC.PlayDialogue(dialogueTextBox);
                }
                else
                {
                    //Close text box
                    dialogueTextBox.transform.parent.gameObject.SetActive(false);
                    Cursor.lockState = CursorLockMode.Locked;
                    waiting = false;
                }
            }
        }
    }

    void PauseToggle(bool forcePauseOn)
    {
        if (!paused || forcePauseOn)
        {
            //Pause
            pausedText.gameObject.SetActive(true);
            paused = true;

            Time.timeScale = 0;

            if (playerInventory.inventoryInterface.activeInHierarchy)
            {
                playerInventory.InventoryToggle();
            }
        }
        else
        {
            //Unpause
            pausedText.gameObject.SetActive(false);
            paused = false;

            Time.timeScale = 1;
        }
    }

    void InventoryToggle()
    {
        if (!attacking)
            playerInventory.InventoryToggle();
    }

    void InventorySelect(InventoryDir iDir)
    {
        if (playerInventory.inventoryInterface.activeInHierarchy)
        {
            playerInventory.GetComponent<PlayerInventory>().SelectInventoryPos(iDir);
        }
    }

    void InventoryTabChange(InventoryDir iDir)
    {
        if (playerInventory.inventoryInterface.activeInHierarchy)
        {
            playerInventory.GetComponent<PlayerInventory>().SelectInventoryTab(iDir);
        }
    }

    void EquipItem()
    {
        if (playerInventory != null && playerInventory.inventoryInterface.activeInHierarchy && playerInventory.GetComponent<PlayerInventory>().selectedItem != null && playerInventory.currInventoryIndex != 0)
        {
            playerInventory.GetComponent<PlayerInventory>().ItemIsSelected(playerInventory.GetComponent<PlayerInventory>().selectedItem, playerInventory.GetComponent<PlayerInventory>().selectedItemGObj, true);
        }
    }

    void ShowItemInfo()
    {

    }

    public void BronzeCollect(int coins)
    {
        GetComponent<PlayerValues>().bronze += (int)(coins * coinToBronzeFactor);
        bronzeAmountLabel.text = GetComponent<PlayerValues>().bronze.ToString();
    }

    public void KnockBack(Transform attackingEntityPos)
    {
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

            if (currMomentumValue > maxMomentum)
            {
                currMomentumValue = maxMomentum;
            }
        }
    }

    void ItemSwitch(int hotKeyNumber)
    {
        //playerInventory.HotKeyedItem(hotKeyNumber, weaponSheathed);
    }

    public void StatusMessageShow(string sMessage)
    {
        statusMessage.gameObject.SetActive(true);
        statusMessage.text = sMessage;
        StartCoroutine(StatusMessageDisappear(statusMessageVisibleTime));
    }

    private IEnumerator StatusMessageDisappear(float visTime)
    {
        yield return new WaitForSeconds(visTime);

        statusMessage.gameObject.SetActive(false);
    }

    private void Restart1(bool on)
    {
        if (on)
            restarting1 = true;
        else
            restarting1 = false;
    }

    private void Restart2(bool on)
    {
        if (on)
            restarting2 = true;
        else
            restarting2 = false;
    }

    private void checkRestartDemo()
    {
        if (restarting1 && restarting2)
        {
            string tutorialLevelName = "Dungeon Tutorial";

            // Go to tutorial level
            SceneManager.LoadScene(tutorialLevelName);
            Destroy(gameObject);
            Destroy(GameObject.Find("Canvas"));
        }
    }

    // TODO: use this method upon entering each new scene (Temporarily called in Awake())
    void NewLevelLoad()
    {
        foreach (GameObject eWall in GameObject.FindGameObjectsWithTag("Enemy Wall"))
        {
            Physics.IgnoreCollision(GetComponent<PlayerController>().controller, eWall.GetComponent<Collider>());
        }

        foreach (GameObject weakPointCol in GameObject.FindGameObjectsWithTag("Weak Point"))
        {
            Physics.IgnoreCollision(GetComponent<PlayerController>().controller, weakPointCol.GetComponent<Collider>());
        }
    }

    
}
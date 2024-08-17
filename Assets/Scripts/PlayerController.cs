using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;

public class PlayerController : MonoBehaviour
{
    PlayerInput playerInput;
    PlayerInput.MainActions input;

    [SerializeField] GameObject weaponBasis;

    [SerializeField] GameObject cameraBorder;

    CharacterController controller;
    Animator animator;
    AudioSource audioSource;

    public Image momentumBarUI;

    public Image recordingBarUI;

    public Image recordingDot;

    [Header("Controller")]
    public float moveSpeed = 3.5f;

    public float camMoveSpeed = 2;

    public float moveSpeedDefault;
    public float gravity = -9.8f;
    public float jumpHeight = 1.2f;

    Vector3 _PlayerVelocity;

    bool isGrounded;

    [Header("Camera")]
    public Camera cam;
    public Camera VHSCam;

    bool cameraLocked = false;

    public float sensitivity = 10f;
    public float camSensitivity = 6;
    public float sensitivityDefault;
    public float camNormalPOV = 60;
    public float camZoomedOutPOV = 35;

    public float camRecordDistance = 10;
    public float recordingAmount = 0;
    private float maxRecordingAmount = 100;
    public float recordingSpeed = 3.5f;
    //public float recordingDecreaseSpeed = 3.5f;
    bool recording = false;

    float xRotation = 0f;

    /* Animation variables */
    [Header("Animation")]
    public const string IDLE = "Null";
    public const string WALK = "Null";
    public const string SWINGACROSS = "Sword Swing Across";
    public const string SWINGDOWN = "Sword Swing Down";
    public const string SWINGBACK = "Sword Swing Across Back";
    public const string BLOCK = "Sword Block";

    string currentAnimationState;

    /* Attacking variables */
    [Header("Attacking")]
    public float attackDistance = 3f;
    public float attackDelay = 1.7f;
    public float attackDelayDefault;
    private float attackSpeed = 0.4f;
    public int attackDamage = 1;
    public LayerMask attackLayer;

    public GameObject hitEffect;
    public AudioClip swordSwing;
    public AudioClip hitSound;

    bool attacking = false;

    bool camcorderEnabled = false;
    

    bool blocking = false;
    bool readyToAttack = true;
    int attackCount;    

    /* Momentum bar variables */
    [Header("Momentum Bar")]
    public float currMomentumValue;
    public float maxMomentum = 1.2f;
    public float momumtumIncrease = 0.1f;
    private bool momentumDecreasing = false;
    public float maxTimeBeforeMomentumDecrease = 2f;
    private float timeBeforeMomentumDecrease;
    public float momentumDecreaseSpeed = 0.1f;

    void Awake()
    { 
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();

        playerInput = new PlayerInput();
        input = playerInput.Main;
        AssignInputs();

        cam.gameObject.SetActive(true);
        VHSCam.gameObject.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        momentumBarUI.fillAmount = 0;
        recordingBarUI.fillAmount = 0;

        moveSpeedDefault = moveSpeed;
        sensitivityDefault = sensitivity;
        attackDelayDefault = attackDelay;
       // animator.speed += 2;

       cameraBorder.SetActive(false);
       recordingDot.gameObject.SetActive(false);
    }

    void Update()
    {
        isGrounded = controller.isGrounded;

        // Repeat Inputs
        if(input.Attack.IsPressed())
        { Attack(); }

        SetAnimations();

        CamcorderRaycast();
    }

    void FixedUpdate() 
    { 
        MoveInput(input.Movement.ReadValue<Vector2>());
    }

    void LateUpdate() 
    { 
        LookInput(input.Look.ReadValue<Vector2>()); 

        momentumBarUI.fillAmount = currMomentumValue / maxMomentum;

        recordingBarUI.fillAmount = recordingAmount / maxRecordingAmount;

        timeBeforeMomentumDecrease -= Time.deltaTime;

        if (timeBeforeMomentumDecrease <= 0)
        {
            if (currMomentumValue >= 0)
            {
                currMomentumValue -= Time.deltaTime * momentumDecreaseSpeed;
                
                if (attackDelay < attackDelayDefault)
                    attackDelay += Time.deltaTime * momentumDecreaseSpeed;
                
                if (moveSpeed > moveSpeedDefault)
                    moveSpeed -= Time.deltaTime * (2 * momentumDecreaseSpeed);
            }
            else
            {
                currMomentumValue = 0;
            }
        } 
    }

    void MoveInput(Vector2 input)
    {
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;

        controller.Move(transform.TransformDirection(moveDirection) * moveSpeed * Time.deltaTime);
        _PlayerVelocity.y += gravity * Time.deltaTime;
        if(isGrounded && _PlayerVelocity.y < 0)
            _PlayerVelocity.y = -2f;
        controller.Move(_PlayerVelocity * Time.deltaTime);
    }

    void LookInput(Vector3 input)
    {
        float mouseX = input.x;
        float mouseY = input.y;

        xRotation -= (mouseY * Time.deltaTime * sensitivity);
        xRotation = Mathf.Clamp(xRotation, -80, 80);

        cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        transform.Rotate(Vector3.up * (mouseX * Time.deltaTime * sensitivity));
        
    }

    void OnEnable() 
    { input.Enable(); }

    void OnDisable()
    { input.Disable(); }

    void Jump()
    {
        // Adds force to the player rigidbody to jump
        if (isGrounded)
            _PlayerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
    }

    void AssignInputs()
    {
        input.Jump.performed += ctx => Jump();
        input.Attack.started += ctx => Attack();
        input.Block.started += ctx => SwitchCamcorderView();
        input.Record.started += ctx => CamcorderRecord();
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

        Invoke(nameof(ResetAttack), attackDelay);
        Invoke(nameof(AttackRaycast), attackSpeed);

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
    }

    void AttackRaycast()
    {
        GameObject weapon = weaponBasis.transform.parent.gameObject;

        if(Physics.Raycast(weapon.transform.position, weapon.transform.forward, out RaycastHit hit, attackDistance, attackLayer))
        { 
            HitTarget(hit.point);

            /* Enemy hit by melee */
            if(hit.transform.TryGetComponent<Enemy>(out Enemy T))
            { 
                timeBeforeMomentumDecrease = maxTimeBeforeMomentumDecrease;

                T.TakeDamage(attackDamage); 
                
                momentumDecreasing = false;

                /* Momentum increases upon hitting an enemy */
                momentumIncrease();
            }
        } 
    }

    void CamcorderRaycast()
    {
        GameObject weapon = weaponBasis.transform.parent.gameObject;

        if(Physics.Raycast(weapon.transform.position, weapon.transform.forward, out RaycastHit hit, camRecordDistance, attackLayer) && recording && camcorderEnabled)
        { 

            /* Enemy targeted by camcorder */
            if(hit.transform.TryGetComponent<Enemy>(out Enemy T))
            { 
                recordingAmount += Time.deltaTime * recordingSpeed;
            }
            else
            {
                recordingAmount -= Time.deltaTime * recordingSpeed;
                if (recordingAmount < 0) { recordingAmount = 0; }
            }
        } 
        else
        {
            recordingAmount -= Time.deltaTime * recordingSpeed;
            if (recordingAmount < 0) { recordingAmount = 0; }
        }
    }

    void CamcorderRecord()
    {
        if (camcorderEnabled)
        {
            if (recording)
            {
                recording = false;

                recordingDot.gameObject.SetActive(false);
            }
            else
            {
                recording = true;

                recordingDot.gameObject.SetActive(true);
            }
            
        }
    }

    IEnumerator MomentumDecreaseTime()
    {
        yield return new WaitForSeconds(timeBeforeMomentumDecrease);

        momentumDecreasing = true;
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

            ChangeAnimationState(BLOCK);
        }
        else
        {
            blocking = false;
        }
    }

    void SwitchCamcorderView()
    {
        /* Camcorder view */
        if (!camcorderEnabled)
        {
            camcorderEnabled = true;
            cameraBorder.SetActive(true);

            cam.gameObject.SetActive(false);
            VHSCam.gameObject.SetActive(true);

            cam.fieldOfView = camZoomedOutPOV;

            if (recording)
            {
                recordingDot.gameObject.SetActive(true);
            }

            moveSpeed = camMoveSpeed;
        }
        /* Normal view */
        else
        {
            camcorderEnabled = false;
            cameraBorder.SetActive(false);

            cam.gameObject.SetActive(true);
            VHSCam.gameObject.SetActive(false);

            cam.fieldOfView = camNormalPOV;

            moveSpeed = moveSpeedDefault;

            recordingDot.gameObject.SetActive(false);
        }

    }

    void LockCamera()
    {
        if (!cameraLocked)
        {
            cameraLocked = true;
            //transform.rotation = new Quaternion(transform.rotation.x, 0, transform.rotation.z, 0);
            cam.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            cameraLocked = false;          
        }
    }

    void momentumIncrease()
    {
        if (currMomentumValue < maxMomentum)
        {
            currMomentumValue += momumtumIncrease;

            attackDelay -= momumtumIncrease;
            moveSpeed += 2 * momumtumIncrease;
        }
    }

    
}
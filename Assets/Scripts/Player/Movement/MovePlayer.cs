
using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MovePlayer : MonoBehaviour
{
    [Header("Basic Movement")]
    public Rigidbody rb;
    [Range(1, 500)] public float walkSpeed;
    [Range(1, 500)] public float sprintSpeed;
    [Range(0, 500)] public float drag;
    public Transform orientation;
    float moveSpeed;
    float horizontalInput;
    float verticalInput;
    bool isMoving;


    PlayerStamina playerStamina;
    
    //adrenaline
    Adrenaline adrenaline;


    [Header("Jumping")]
    [Range(0, 500)] public float jumpForce;
    [Range(0, 2)] public float jumpCoolDown;
    [Range(0, 100)] public float jumpCost;
    [Range(0, 2)] public float airMultiplier;
    [Range(0, 500)] public float airdrag;
    bool jumpReady = true;

    [Header("Crouching")]
    [Range(1, 500)] public float crouchSpeed;
    [Range(0, 1)] public float crouchYScale;
    public GameObject body;
    private float startYScale;

    [Header("Slopes")]
    [Range(0, 180)] public float maxSlopeAngle;
    private RaycastHit slopehit;
    bool exitingSlope;

    [Header("GroundCheck")]
    public float playerHeight;
    public LayerMask groundLayer;
    bool isGrounded;

    HUD hud;
    public TMP_Text HUDUI;
    float altitude;

    //holder variables
    #region placeholder variables
    //basic movement

    float moveSpeed_;

    //stamina
    float timeleftuntilstim;
    float adrenalineTime;
    bool s = true;
    float i = 0;

    //jumping
    float jumpForce_;
    float jumpCost_;
    [HideInInspector] public float currentJumpCostMultiplier = 1;
    [HideInInspector] public float currentJumpForceMultiplier = 1;
    [HideInInspector] public float currentSpeedMultiplier = 1;
    #endregion

    Vector3 moveDirection;

    [HideInInspector]
    public MovementState state;

    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        prone,
        air,
    }

    // Start is called before the first frame update
    private void Start()
    {
        hud = GetComponent<HUD>();
        playerStamina = GetComponent<PlayerStamina>();
        adrenaline = GetComponent<Adrenaline>(); 
        startYScale = transform.localScale.y; 
        moveSpeed = walkSpeed;
        rb.freezeRotation = true;
    }

    // Update is called once per frame
    private void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, groundLayer);
        if (isGrounded)
        {
            rb.drag = drag;
        }
        else
        {
            rb.drag = airdrag;
        }
        CheckInput();
        ControlSpeed();
        UpdateUI();
        stateHandler();
    }

    private void FixedUpdate()
    {
        PlayerMove();
    }

    private void CheckInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");


        //jump
        if (Input.GetKey(KeyCode.Space) && jumpReady  && isGrounded && playerStamina.stamina - jumpCost > 0)
        {
            jumpReady = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCoolDown);
        }

        //crouch
        if (Input.GetKeyDown(KeyCode.LeftControl) && isGrounded && state != MovementState.sprinting)
        {
            playerStamina.currentGainRate = playerStamina.staminaGainRateWhenCrouching;
            body.transform.localScale = new Vector3(transform.localScale.x,crouchYScale,transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
        if (Input.GetKeyUp(KeyCode.LeftControl) && isGrounded && state != MovementState.sprinting)
        {
            playerStamina.currentGainRate = playerStamina.staminaGainRate;
            body.transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }


    void stateHandler()
    {
        if (moveDirection != new Vector3(0,0,0))
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        if (Input.GetKey(KeyCode.LeftControl) && isGrounded)
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }

        else if (Input.GetKey(KeyCode.LeftShift) && isGrounded && isMoving )
        {
             state = MovementState.sprinting;
             if (playerStamina.stamina > 0)
             {
                playerStamina.staminaGainAllowed = false;
                playerStamina.StaminaDrain();
                moveSpeed = sprintSpeed;
             }
             else
             {
                moveSpeed = walkSpeed+0.15f*sprintSpeed;
             }
        }

        else if (isGrounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }
        else
        {
            state = MovementState.air;
            moveSpeed = walkSpeed;
        }
    }

    void PlayerMove()
    {
        moveSpeed_ = moveSpeed * currentSpeedMultiplier;
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (OnSlope() & !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirecion() * moveSpeed_ * currentSpeedMultiplier * 200f * 4.17f, ForceMode.Force);
            if (rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f);
            }
        }

        if (isGrounded)
        {
           
            rb.AddForce(moveDirection.normalized * moveSpeed_ * currentSpeedMultiplier * 100f * 4.17f, ForceMode.Force);
        }
        else
        {
            rb.AddForce(moveDirection.normalized * moveSpeed_ * currentSpeedMultiplier * 100f * 4.17f *  airMultiplier, ForceMode.Force);
        }

        rb.useGravity = !OnSlope();
    }

    void ControlSpeed()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed_)
            {
                rb.velocity = rb.velocity.normalized * moveSpeed_;
            }
        }

        else if (flatVel.magnitude > moveSpeed_)
        {
            Vector3 limitVel = flatVel.normalized * moveSpeed_;
            rb.velocity = new Vector3(limitVel.x, rb.velocity.y, limitVel.z);
        }
    }

    void Jump()
    {
        jumpForce_ = jumpCost * currentJumpForceMultiplier * 100f;
        jumpCost_ = jumpCost * currentJumpCostMultiplier;
        exitingSlope = true;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce_, ForceMode.Impulse);
        playerStamina.stamina -= jumpCost_;
        playerStamina.staminaGainAllowed = false;
    }

    private void ResetJump()
    {
        exitingSlope = false;
        jumpReady = true;
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopehit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopehit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirecion()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopehit.normal).normalized;
    }

    public void UpdateUI()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            altitude = hit.distance - 1;
        }
        HUDUI.text = "Speed: " + Math.Round(rb.velocity.magnitude, 1) + "\nAltitude: " + Math.Round(altitude, 1) + "\nAdrenaline active: " + adrenaline.adrenalineActive + "\nStims: " + adrenaline.stims + "\n" + state;
    }
}

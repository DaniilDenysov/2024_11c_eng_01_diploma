using IngameDebugConsole;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float movementSpeed;
    [SerializeField] public float walkSpeed = 7f;
    [SerializeField] public float sprintSpeed = 10f;
    [SerializeField] public float groundDrag = 5f;

    [Header("Jumping")]
    [SerializeField] public float jumpForce = 12f;
    [SerializeField] public float jumpCooldown = 0.05f;
    [SerializeField] public float airMultiplier = 0.4f;
    private bool isReadyToJump;

    [Header("Crouching")]
    [SerializeField] public float crouchSpeed = 5f;
    public float crouchYScale = 0.5f;
    public float startYScale;

    [Header("Slope Handling")]
    public float maxSlopeAngle = 40f;
    private RaycastHit slopeHit;
    private bool exitingSlope;


//    [Header("Keybinds")]
//    [SerializeField] public KeyCode jumpKey = KeyCode.Space;
//    [SerializeField] public KeyCode sprintKey = KeyCode.LeftShift;
//    [SerializeField] public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("GroundCheck")]
    [SerializeField] public float playerHeight;
    [SerializeField] public LayerMask whatIsGround;
    private bool isGrounded;

    public Transform orientation;

    private float horizontalInput;
    private float verticalInput;
    private bool isJumping;
    private bool isCrouching;
    private bool isSprinting;
    private bool isCurrentlyCrouching = false;

    private DefaultInput inputActions;

    public MovementState state;

    Vector3 movementDirection;

    Rigidbody rb;

    public enum MovementState
    {
        walking,
        sprinting,
        air,
        crouching
    }

    #region Script Basics
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        isReadyToJump = true;

        startYScale = transform.localScale.y;

        inputActions = new DefaultInput();
        inputActions.Enable();

        inputActions.Player.Jump.started += context => isJumping = true;
        inputActions.Player.Crouch.started += context => isCrouching = true;
        inputActions.Player.Crouch.canceled += context => isCrouching = false;
        inputActions.Player.Run.started += context => isSprinting = true;
        inputActions.Player.Run.canceled += context => isSprinting = false;
    }

    private void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        
        MyInput();
        SpeedControl();
        StateHandler();

        if (isGrounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }
    #endregion

    #region Input Handling
    private void StateHandler()
    {

        if(isCrouching && isGrounded)
        {
            state = MovementState.crouching;
            movementSpeed = crouchSpeed;

            Debug.Log("Crouching");
        }

        if(isGrounded && isSprinting)
        {
            state = MovementState.sprinting;
            movementSpeed = sprintSpeed;

            Debug.Log("Sprinting");
        }
        else if (isGrounded)
        {
            state = MovementState.walking;
            movementSpeed = walkSpeed;

            Debug.Log("Walkinging");
        }
        else
        {
            state = MovementState.air;

            Debug.Log("Jumping");
        }
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(isJumping && isReadyToJump && isGrounded)
        {
            isReadyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
        isJumping = false;

        if (isCrouching && isGrounded && !isCurrentlyCrouching)
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
            isCurrentlyCrouching = true;
        }

        if (!isCrouching && isGrounded && isCurrentlyCrouching)
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
            isCurrentlyCrouching = false;
        }
    }
    #endregion

    #region Movement Handling
    private void MovePlayer()
    {
        if (rb != null)
        {
            movementDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

            if (isOnSlope() && !exitingSlope)
            {
                rb.AddForce(GetSlopeMovementDirection() * movementSpeed * 20f, ForceMode.Force);

                if(rb.velocity.y > 0)
                {
                    rb.AddForce(Vector3.down * 80f, ForceMode.Force);
                }
            }

            if (isGrounded)
                rb.AddForce(movementDirection.normalized * movementSpeed * 10f, ForceMode.Force);

            else if (!isGrounded)
                rb.AddForce(movementDirection.normalized * movementSpeed * 10f * airMultiplier, ForceMode.Force); 

            rb.useGravity = !isOnSlope();
        }
    }

    private void SpeedControl()
    {

        if (isOnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > movementSpeed)
                rb.velocity = rb.velocity.normalized * movementSpeed;
            Debug.Log("is on slope");
        }
        else
        {

            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if (flatVel.magnitude > movementSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * movementSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    private void Jump()
    {
        if(rb != null)
        {
            exitingSlope = true;

            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void ResetJump()
    {
        isReadyToJump = true;
        exitingSlope = false;
    }

    private bool isOnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMovementDirection()
    {
        return Vector3.ProjectOnPlane(movementDirection, slopeHit.normal).normalized;
    }
    #endregion
}
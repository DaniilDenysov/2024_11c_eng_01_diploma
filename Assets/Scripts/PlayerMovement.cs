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

    [Header("GroundCheck")]
    [SerializeField] public float playerHeight;
    [SerializeField] public LayerMask whatIsGround;
    private bool isGrounded;

  //  public Transform orientation;

    private float horizontalInput;
    private float verticalInput;
    private bool isJumping;
    private bool isCrouching;
    private bool isSprinting;
    private bool isCurrentlyCrouching = false;
    private bool isOnSlope = false;
    private DefaultInput inputActions;

    public MovementState state;

    Vector3 movementDirection;
    CapsuleCollider capsuleCollider;
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
        capsuleCollider = GetComponent<CapsuleCollider>();
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
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            isOnSlope = angle < maxSlopeAngle && angle != 0;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
            isOnSlope = false;
        }

        MyInput();
        //SpeedControl();
        StateHandler();

        /*if (isGrounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;*/
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
        }

        if(isGrounded && isSprinting)
        {
            state = MovementState.sprinting;
            movementSpeed = sprintSpeed;;
        }
        else if (isGrounded)
        {
            state = MovementState.walking;
            movementSpeed = walkSpeed;
        }
        else
        {
            state = MovementState.air;
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

        if (isGrounded && isCrouching)
        {
            capsuleCollider.height = playerHeight / 2;
            //transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            //rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
            
        }
        else
        {
            capsuleCollider.height = playerHeight;
            // transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
           
        }
    }
    #endregion

    #region Movement Handling
    private void MovePlayer()
    {
        if (rb != null)
        {
            movementDirection = transform.forward * verticalInput + transform.right * horizontalInput;

            if (isOnSlope && !exitingSlope)
            {
                Vector3 slopeForce = GetSlopeMovementDirection() * movementSpeed;
                rb.AddForce(slopeForce, ForceMode.Force);
            }
            else if (isGrounded)
            {
                Vector3 horizontalVelocity = movementDirection.normalized * movementSpeed;
                rb.velocity = new Vector3(horizontalVelocity.x, rb.velocity.y, horizontalVelocity.z);
            }
            else if (!isGrounded)
            {
                Vector3 targetVelocity = movementDirection.normalized * movementSpeed * airMultiplier * 1.2f;
                Vector3 smoothedVelocity = Vector3.Lerp(new Vector3(rb.velocity.x, 0, rb.velocity.z), targetVelocity, Time.fixedDeltaTime * 5f);
                rb.velocity = new Vector3(smoothedVelocity.x, rb.velocity.y, smoothedVelocity.z);

                rb.AddForce(movementDirection.normalized * movementSpeed * 0.1f, ForceMode.Impulse);
            }

            rb.drag = isGrounded ? groundDrag : 0;
        }
    }


    private void Jump()
    {
        if(rb != null)
        {
            // rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            exitingSlope = true;

            Vector3 currentVelocity = rb.velocity;

            rb.velocity = new Vector3(currentVelocity.x, 0f, currentVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void ResetJump()
    {
        isReadyToJump = true;
        exitingSlope = false;
    }

    private bool IsOnSlope()
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
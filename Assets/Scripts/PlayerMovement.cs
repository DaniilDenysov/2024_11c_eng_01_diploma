using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] public float movementSpeed;

    [SerializeField] public float groundDrag;

    [SerializeField] public float jumpForce;
    [SerializeField] public float jumpCooldown;
    [SerializeField] public float airMultiplier;
    private bool isReadyToJump;

    [Header("Keybinds")]
    [SerializeField] public KeyCode jumpKey = KeyCode.Space;

    
    
    [Header("GroundCheck")]
    [SerializeField] public float playerHeight;
    [SerializeField] public LayerMask whatIsGround;
    private bool isGrounded;


    public Transform orientation;

    private float horizontalInput;
    private float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        
        MyInput();
        SpeedControl();

        if (isGrounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(Input.GetKey(jumpKey) && isReadyToJump && isGrounded)
        {
            isReadyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        if (rb != null)
        {
            moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
            if (isGrounded)
                rb.AddForce(moveDirection.normalized * movementSpeed * 10f, ForceMode.Force);
            else if (!isGrounded)
                rb.AddForce(moveDirection.normalized * movementSpeed * 10f * airMultiplier, ForceMode.Force); 
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if(flatVel.magnitude > movementSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * movementSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        if(rb != null)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void ResetJump()
    {
        isReadyToJump = true;
    }
}
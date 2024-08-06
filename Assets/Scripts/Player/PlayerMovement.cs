using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //Variables
    [Header("---Player Values---")]
    [SerializeField] private float acceleration;
    [SerializeField] private float jumpPower;
    [SerializeField] private float slidingPower;
    [SerializeField] private float slidingEndSpeed;

    //Cooldowns
    [Header("---Cooldown Duration---")]
    [SerializeField] private Cooldown slideCooldown;

    //Ground Check Configuration
    [Header("---Ground Check Box Configuration---")]
    [SerializeField] private float castDistance;
    [SerializeField] private Vector2 boxSize;
    [SerializeField] private LayerMask groundLayer;

    //Player Component Reference
    [Header("---Component Reference---")]
    [SerializeField] protected Rigidbody2D _rigidbody2D;
    [SerializeField] protected Animator _animator;
    [SerializeField] private PlayerAnimationHandler playerAnimationHandler;

    //Boolean Conditions
    protected bool isMovingRight;
    protected bool isJumping;
    protected bool isFalling;
    protected bool isHanging;
    protected bool isRunning;
    protected bool isSliding;
    protected bool isGrounded;

    public bool IsMovingRight {  get { return isMovingRight; } }


    //Input
    private Vector2 _inputDirection;

    private void Start()
    {
        //Send error message if component reference is missing
        if (_rigidbody2D == null)
            Debug.LogWarning("Player Rigidbody2D not found");

        if (_animator == null)
            Debug.LogWarning("Player Animator not found");
    }

    private void Update()
    {
        HandleInput();
        GroundCheck();
        HorizontalMovement();
        SlideMovement();
    }

    private void FixedUpdate()
    {
        VerticalMovement();
    }

    private void HandleInput()
    {
        //Get input from x and y input
        _inputDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    //-----------Basic Movement------------//
    private void HorizontalMovement()
    {
        //Only allow x input when grounded
        if (isGrounded != true)
            return;

        if (isSliding == false)
        {
            _rigidbody2D.velocity = new Vector2(_inputDirection.x * acceleration, _rigidbody2D.velocity.y);
            
        }

        if (_rigidbody2D.velocity.x > 0 || _rigidbody2D.velocity.x < 0)
        {
            isRunning = true;

            if (_rigidbody2D.velocity.x > 0)
            {
                isMovingRight = true;
            }
            if (_rigidbody2D.velocity.x < 0)
            {
                isMovingRight = false;
            }
        }
        else
        {
            isRunning = false;
        }
    }

    private void VerticalMovement()
    {
        //Only jump when grounded
        if (Input.GetButton("Jump") && isGrounded == true)
        {
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, jumpPower);
        }
    }

    //----------Special Movement----------//
    private void SlideMovement()
    {
        //Slide when input key is pressed while character is runnning and ability is off cooldown
        if (Input.GetKey(KeyCode.LeftShift) && isRunning == true && slideCooldown.CurrentProgress is Cooldown.Progress.Ready)
        {
            if (isMovingRight == true)
            {
                _rigidbody2D.AddForce(Vector2.right * slidingPower);
            }
            if (isMovingRight == false)
            {
                _rigidbody2D.AddForce(Vector2.left * slidingPower);
            }

            slideCooldown.StartCooldown();
            isSliding = true;
        }

        //Set isSliding to false after reaching the end speed to allow the player to walk
        //Check end speed when sliding left or right
        if (_rigidbody2D.velocity.x > 0 && isSliding == true)
        {
            
            if (_rigidbody2D.velocity.x <= slidingEndSpeed)
            {
                isSliding = false;
            }
        }
        if (_rigidbody2D.velocity.x  < 0)
        {
            if (_rigidbody2D.velocity.x >= -slidingEndSpeed && isSliding == true)
            {
                isSliding = false;
            }   
        }

        //Reset slide cooldown when duration ends
        if (slideCooldown.CurrentProgress is Cooldown.Progress.Finished)
        {
            slideCooldown.ResetCooldown();
        }
        
    }

    private void GroundCheck()
    {
        //Creates a box that returns true when overlapping with groundLayer
        if (Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, castDistance, groundLayer))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void OnDrawGizmos()
    {
        //Makes the GroundCheck box visible
        Gizmos.DrawWireCube(transform.position - transform.up * castDistance, boxSize);
    }
}

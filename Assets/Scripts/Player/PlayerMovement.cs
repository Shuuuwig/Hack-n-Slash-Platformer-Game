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
    [SerializeField] private float fallingSpeedLimit;

    //Cooldowns
    [Header("---Cooldown Duration---")]
    [SerializeField] private Cooldown slideCooldown;

    //Ground Check Configuration
    [Header("---Ground Check Box Configuration---")]
    [SerializeField] private float castDistanceGround;
    [SerializeField] private Vector2 boxSizeGround;
    [SerializeField] private LayerMask groundLayer;

    //Wall Check Configuration
    [Header("---Ledge Check Box Configuration---")]
    [SerializeField] private float castDistanceLedge;
    [SerializeField] private Vector2 boxSizeLedge;

    //Player Component Reference
    [Header("---Component Reference---")]
    [SerializeField] protected Rigidbody2D _rigidbody2D;
    [SerializeField] protected Animator _animator;
    [SerializeField] protected Collider2D _mainCollider2D;
    [SerializeField] protected Collider2D _slideCollider2D;
    [SerializeField] private PlayerAnimationHandler playerAnimationHandler;

    //Boolean Conditions
    protected bool isMovingRight;
    protected bool isJumping;
    protected bool isFalling;
    protected bool isLetGO;
    protected bool isClimbing;
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
        LedgeCheck();
        HorizontalMovement();
        LedgeHang();
        SlideMovement();
        RigidbodyLimiter();
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
            isJumping = true;
        }

        if (_rigidbody2D.velocity.y < 0f && isGrounded == false)
        {
            Debug.Log("Is Falling");
            isFalling = true;
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

            //Enable Slide collider on slide and disable main collider
            _mainCollider2D.enabled = false;
            _slideCollider2D.enabled = true;

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
                _mainCollider2D.enabled = true; //Re-enable main collider when reaching slide end speed
                _slideCollider2D.enabled = false; //Disable slide collider when reaching slide end speed
            }
        }
        if (_rigidbody2D.velocity.x  < 0)
        {
            if (_rigidbody2D.velocity.x >= -slidingEndSpeed && isSliding == true)
            {
                isSliding = false;
                _mainCollider2D.enabled = true; //Re-enable main collider when reaching slide end speed
                _slideCollider2D.enabled = false; //Disable slide collider when reaching slide end speed
            }   
        }

        //Reset slide cooldown when duration ends
        if (slideCooldown.CurrentProgress is Cooldown.Progress.Finished)
        {
            slideCooldown.ResetCooldown();
        }
        
    }

    private void LedgeHang()
    {
        if (isHanging == true)
        {
            //Freeze position when near ledge
            _rigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;

            //Let go of the ledge
            if (Input.GetKey(KeyCode.LeftControl))
            {
                isHanging = false;
                isLetGO = true;
            }
            if (Input.GetKey(KeyCode.Space))
            {
                isHanging = false;
                isClimbing = true;
            }
        }
        else if (isHanging == false && isLetGO == true)
        {
            //Unfreeze position
            _rigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionX & RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
            _rigidbody2D.AddForce(Vector2.down); //Push the player down because the player gets stuck
        }
    }

    //Limiter
    private void RigidbodyLimiter()
    {
        if (_rigidbody2D.velocity.y > fallingSpeedLimit && isFalling == true)
        {
            Debug.Log(_rigidbody2D.velocity.y);
            _rigidbody2D.velocity = Vector3.ClampMagnitude(_rigidbody2D.velocity, fallingSpeedLimit);
        }
    }

    //Collision Checks
    private void LedgeCheck()
    {
        if (isGrounded == true)
            return;

        //Return if player lets go or climbs ledge
        if (isLetGO == true || isClimbing == true) 
            return;

        Debug.Log("Hung");
        //Creates a box that returns true when overlapping with groundLayer
        if (Physics2D.BoxCast(new Vector2(transform.position.x, transform.position.y + 0.775f), boxSizeLedge, 0, -transform.up, castDistanceLedge, groundLayer))
        {
            isHanging = true;
        }
        else
        {
            isHanging = false;
        }
    }

    private void GroundCheck()
    {
        //Creates a box that returns true when overlapping with groundLayer
        if (Physics2D.BoxCast(transform.position, boxSizeGround, 0, -transform.up, castDistanceGround, groundLayer))
        {
            isGrounded = true;
            isJumping = false;
            isFalling = false;
            isLetGO = false;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void OnDrawGizmos()
    {
        //Makes the Check Box Visible
        Gizmos.DrawWireCube(transform.position - transform.up * castDistanceGround, boxSizeGround);
        Gizmos.DrawWireCube(transform.position - transform.up * castDistanceLedge, boxSizeLedge);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("---Gizmo Configuration---")]
    [SerializeField] protected bool gizmoToggleOn = true;

    //Variables
    [Header("---Player Values---")]
    [SerializeField] private float acceleration;
    [SerializeField] private float jumpPower;
    [SerializeField] private float jumpApexGravityDivider;
    [SerializeField] private float pogoJumpPower;
    [SerializeField] private float slidingPower;
    [SerializeField] private float slidingEndSpeed;
    [SerializeField] private float fallingGravityMultiplier;
    [SerializeField] private float fallingSpeedLimit;
    [SerializeField] private float maxJumpForce;
    [SerializeField] private Vector2 wallJumpPower;
    [SerializeField] private Vector2 climbLedgePosition;

    //Cooldowns
    [Header("---Cooldown and Timer Duration---")]
    [SerializeField] private Cooldown coyoteTime;
    [SerializeField] private Cooldown bufferJumpTime;
    [SerializeField] private Cooldown slideCooldown;
    [SerializeField] private Cooldown knockedbackTimer; //Duration is passed by other gameobjects

    //Layer Masks
    [Header("---Layer Masks---")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    //Ground Check Configuration
    [Header("---Ground Check Box Configuration---")]
    [SerializeField] private float castDistanceGround;
    [SerializeField] private Vector2 boxSizeGround;

    //Ledge Check Configuration
    [Header("---Ledge Check Box Configuration---")]
    [SerializeField] private Transform ledgeDetector;
    [SerializeField] private Vector2 boxSizeLedge;

    //Wall Check Configuration
    [Header("---Wall Check Box Configuration---")]
    [SerializeField] private Transform wallDetector;
    [SerializeField] private Vector2 boxSizeWall;

    //Player Component Reference
    [Header("---Component Reference---")]
    [SerializeField] private Rigidbody2D _rigidbody2D;
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D mainCollider2D;
    [SerializeField] private Collider2D slideCollider2D;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private PlayerAnimationHandler playerAnimationHandler;
    [SerializeField] private PlayerCombat playerCombat;

    //Input
    private Vector2 inputDirection;

    //Values
    protected float defaultGravityScale;

    //Boolean Conditions
    protected bool jumpInputPressed;
    protected bool isMovingRight;
    protected bool isJumping;
    protected bool isFalling;
    protected bool isLetGO;
    protected bool isClimbingWall;
    protected bool isClimbingLedge;
    protected bool isHanging;
    protected bool isRunning;
    protected bool isSliding;
    protected bool isKnockedBack;
    protected bool isJumpingOffWall;
    protected bool isGrounded;

    public bool IsGrounded {  get { return isGrounded; } }
    public bool IsMovingRight {  get { return isMovingRight; } }
    public bool IsJumping { get { return isJumping; } }
    public bool IsFalling { get { return isFalling; } }
    public bool IsClimbingWall { get { return isClimbingWall; } }
    public bool IsClimbingLedge { get { return isClimbingLedge; } }
    public bool IsHanging { get { return isHanging; } }
    public bool IsRunning { get { return isRunning; } }
    public bool IsSliding { get { return isSliding; } }
    public Vector2 InputDirection { get { return inputDirection; } }


    private void Start()
    {
        //Send error message if component reference is missing
        if (_rigidbody2D == null)
            Debug.LogWarning("Player Rigidbody2D not found");

        if (animator == null)
            Debug.LogWarning("Player Animator not found");

        defaultGravityScale = _rigidbody2D.gravityScale;
    }

    private void Update()
    {
        HandleInput();

        //Overlap Checks
        WallCheck();
        GroundCheck();
        LedgeCheck();

        //Basic Movement
        VerticalMovement();
        HorizontalMovement();

        //Special Movement
        LedgeHang();
        SlideMovement();
        WallJump();
        Pogo();

        //Rigidbody Manipulation
        RigidbodyLimiter();
        GravityManipulation();

        //State Check
        CheckState();
    }

    private void FixedUpdate()
    {
        
    }

    private void HandleInput()
    {
        //Get input from x and y input
        inputDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    //-----------Basic Movement------------//
    private void HorizontalMovement()
    {
        if (isSliding == true || isKnockedBack == true)
            return;

        _rigidbody2D.velocity = new Vector2(inputDirection.x * acceleration, _rigidbody2D.velocity.y);

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
        if (isSliding == true || isKnockedBack == true)
            return;
        
        //--Buffer Jump--
        if (Input.GetButtonDown("Jump"))
        {
            bufferJumpTime.StartCooldown(); //Start the cooldown when jump is pressed
        }
        else if (bufferJumpTime.CurrentProgress is Cooldown.Progress.Finished)
        {
            bufferJumpTime.ResetCooldown();
        }
        Debug.Log(jumpInputPressed);
       
        
        //--Only allows jump when buffer jump window is active--
        if (bufferJumpTime.CurrentProgress is Cooldown.Progress.InProgress)
        {
            //Jump when grounded or when coyote time is active
            if (isGrounded == true || coyoteTime.CurrentProgress is Cooldown.Progress.InProgress)
            {
                _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, jumpPower);
                isJumping = true;
                jumpInputPressed = true;
            }
        }

        //--Start coyote time--
        if (isGrounded == false && jumpInputPressed == false && coyoteTime.CurrentProgress is Cooldown.Progress.Ready)
        {
            Debug.Log("Coyote started");
            coyoteTime.StartCooldown();
        }
        //Reset coyote time
        else if (isGrounded == true && coyoteTime.CurrentProgress is Cooldown.Progress.Finished)
        {
            coyoteTime.ResetCooldown();
        }
    }

    //----------Special Movement----------//
    private void SlideMovement()
    {
        if (isGrounded == false)
            return;

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
            mainCollider2D.enabled = false;
            slideCollider2D.enabled = true;

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
                mainCollider2D.enabled = true; //Re-enable main collider when reaching slide end speed
                slideCollider2D.enabled = false; //Disable slide collider when reaching slide end speed
            }
        }
        else if (_rigidbody2D.velocity.x  < 0 && isSliding == true)
        {
            if (_rigidbody2D.velocity.x >= -slidingEndSpeed )
            {
                isSliding = false;
                mainCollider2D.enabled = true; //Re-enable main collider when reaching slide end speed
                slideCollider2D.enabled = false; //Disable slide collider when reaching slide end speed
            }   
        }
        else if (_rigidbody2D.velocity.x == 0f && isSliding) //End slide upon hitting a wall
        {
            isSliding = false;
            mainCollider2D.enabled = true; //Re-enable main collider when reaching slide end speed
            slideCollider2D.enabled = false; //Disable slide collider when reaching slide end speed
        }

        //Reset slide cooldown when duration ends
        if (slideCooldown.CurrentProgress is Cooldown.Progress.Finished)
        {
            isSliding = false;
            slideCooldown.ResetCooldown();
            mainCollider2D.enabled = true; //Re-enable main collider when reaching slide end speed
            slideCollider2D.enabled = false; //Disable slide collider when reaching slide end speed
        }
        
    }

    private void LedgeHang()
    {
        if (isHanging != true)
            return;

        //Freeze position when near ledge
        _rigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;

        //Let go of the ledge
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isHanging = false;
            isLetGO = true;

            //Unfreeze position
            _rigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionX & RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
            _rigidbody2D.AddForce(Vector2.down); //Push the player down because the player gets stuck
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isHanging = false;
            isClimbingLedge = true;

            //Unfreeze position
            _rigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionX & RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;

            if (isMovingRight == false)
            {
                _rigidbody2D.position = _rigidbody2D.position + new Vector2(-climbLedgePosition.x, climbLedgePosition.y);
            }
            else if (isMovingRight == true)
            {
                _rigidbody2D.position = _rigidbody2D.position + climbLedgePosition;
            }
        }
    }

    private void WallJump()
    {
        if (isClimbingWall != true)
            return;

        Debug.Log("Wall Jump enabled");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isClimbingWall = false;
            isJumpingOffWall = true;

            //Unfreeze position
            _rigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionX & RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
            _rigidbody2D.gravityScale = defaultGravityScale;
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x * wallJumpPower.x, wallJumpPower.y);
        }
    }

    private void Pogo()
    {
        if (playerCombat.HitObstacle == false)
            return;

        _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, pogoJumpPower);
    }

    private void GrappleSwing()
    {

    }

    //----------Current Active State----------
    private void CheckState()
    {
        if (knockedbackTimer.CurrentProgress is Cooldown.Progress.Finished)
        {
            isKnockedBack = false;
            _rigidbody2D.velocity = Vector2.zero;
            knockedbackTimer.ResetCooldown();
        }
    }

    public void KnockedBackState(Transform enemyTransform, float enemyKnockbackForce, float stunDuration)
    {
        isKnockedBack = true; //Change knockedback bool to true
        Vector2 knockbackDirection = (transform.position - enemyTransform.position).normalized; //Determine knockback direction by comparing player and enemy position
        _rigidbody2D.velocity = knockbackDirection * enemyKnockbackForce; //Multiply knockback with knockback force

        knockedbackTimer.Duration = stunDuration;
        knockedbackTimer.StartCooldown(); //Start cooldown that acts as stun timer

        Debug.Log("Knocked Back");
    }

    private void InvulnerableState()
    {

    }

    //Rigidbody Manipulatation
    private void RigidbodyLimiter()
    {
        Vector2 maxFallVelocity = _rigidbody2D.velocity;
        if (_rigidbody2D.velocity.y > fallingSpeedLimit && isFalling == true)
        {
            maxFallVelocity.y = Mathf.Clamp(maxFallVelocity.y, fallingSpeedLimit, maxJumpForce);
            _rigidbody2D.velocity = maxFallVelocity;
        }
    }

    private void GravityManipulation()
    {
        //Check if player is falling
        if (_rigidbody2D.velocity.y <= 0f && isGrounded == false)
        {
            isFalling = true;
        }
        else
        {
            isFalling = false;
        }

        //Reduce gravity scale at jump apex
        if (isGrounded == false && _rigidbody2D.velocity.y <= 1f && _rigidbody2D.velocity.y >= -1f)
        {
            //Debug.Log("Current gs: " + _rigidbody2D.gravityScale);
            _rigidbody2D.gravityScale = defaultGravityScale / jumpApexGravityDivider;
        }
        else if (isFalling == true) //Increase gravity scale when falling
        {
            _rigidbody2D.gravityScale = defaultGravityScale * fallingGravityMultiplier;
        }
        else
        {
            _rigidbody2D.gravityScale = defaultGravityScale;
        }
    }

    //----------Collision Checks-----------
    private void WallCheck()
    {
        if (isGrounded == true)
            return;

        if (IsClimbingWall == true)
            return;

        if (Physics2D.OverlapBox(wallDetector.position, boxSizeWall, 0, wallLayer))
        {
            //Requires button input to climb
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                isClimbingWall = true;
                isJumpingOffWall = false;
                _rigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                Debug.Log("Climbing Wall");
            }
        }
        else
        {
            isClimbingWall = false;
        }
    }

    private void LedgeCheck()
    {
        if (isGrounded == true)
            return;

        //Stop checking if player is already hanging
        if (isHanging == true)
            return;

        //Return if player lets go or climbs ledge
        if (isLetGO == true || isClimbingLedge == true) 
            return;

        //Creates a box that returns true when overlapping with groundLayer
        if (Physics2D.OverlapBox(ledgeDetector.position, boxSizeLedge, 0, groundLayer))
        {
            Debug.Log("Near Ledge");
            //Requires button input to hang
            if (Input.GetKeyDown(KeyCode.E)) 
            {
                Debug.Log("Hung on Ledge");
                isHanging = true;
            }
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
            //Reset all values to default
            isGrounded = true;
            isJumping = false;
            isFalling = false;
            isLetGO = false;
            isClimbingLedge = false;
            isClimbingWall = false;
            isJumpingOffWall = false;

            jumpInputPressed = false;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

    }

    //--------------------------------
    private void OnDrawGizmos()
    {
        if (gizmoToggleOn != true)
            return;

        //Makes the Check Box Visible
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position - transform.up * castDistanceGround, boxSizeGround); //Ground wire cube
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(ledgeDetector.position, boxSizeLedge); //Ledge wire cube
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(wallDetector.position, boxSizeWall); //Wall wire cube
    }

    
}

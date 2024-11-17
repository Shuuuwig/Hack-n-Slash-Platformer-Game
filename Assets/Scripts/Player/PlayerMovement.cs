using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("===Gizmo Configuration===")]
    [SerializeField] protected bool gizmoToggleOn = true;

    //Variables
    [Header("===Player Configuration===")]
    [Header("---Run---")]
    [SerializeField] private float acceleration;
    [Header("---Dash---")]
    [SerializeField] private float dashPower;
    [SerializeField] private Cooldown dashDuration;
    [SerializeField] private Cooldown dashCooldown;
    [Header("---Jump---")]
    [SerializeField] private float jumpPower;
    [SerializeField] private Cooldown coyoteTime;
    [SerializeField] private Cooldown bufferJumpTime;
    [Header("---Pogo Jump---")]
    [SerializeField] private float pogoPower;
    [Header("---Wall Jump---")]
    [SerializeField] private Vector2 wallJumpPower;
    [SerializeField] private Cooldown wallJumpAppliedForceDuration;
    [Header("---Ledge Hang---")]
    [SerializeField] private Vector2 climbLedgePosition;
    [Header("---Grapple---")]
    [SerializeField] private float grapplePower;
    [SerializeField] private Cooldown grappleMomentumDuration;
    [Header("---Gravity---")]
    [SerializeField] private float jumpApexGravityDivider;
    [SerializeField] private float fallingGravityMultiplier;
    [Header("---Limiter---")]
    [SerializeField] private float fallingSpeedLimit;
    [SerializeField] private float maxJumpForce;
    
    private Vector2 storedPlayerMomentum;

    //States
    [Header("---Knockback State---")]
    [SerializeField] private float knockedbackForce;
    [SerializeField] private Cooldown knockedbackTimer;
    private Vector2 enemyCollisionPoint;

    [Header("===Collision/Trigger Checks Configuration===")]
    [Header("---Ground Check---")]
    [SerializeField] private float castDistanceGround;
    [SerializeField] private Vector2 boxSizeGround;
    [SerializeField] private LayerMask groundLayer;
    RaycastHit2D groundBoxcast;
    [Header("---Ledge Check---")]
    [SerializeField] private Transform ledgeDetector;
    [SerializeField] private Vector2 boxSizeLedge;
    Collider2D ledgeOverlapBox;
    [Header("---Wall Check---")]
    [SerializeField] private Transform wallDetector;
    [SerializeField] private Vector2 boxSizeWall;
    [SerializeField] private LayerMask wallLayer;
    Collider2D wallOverlapBox;
    [Header("---Grapple Check---")]
    [SerializeField] private Transform grappleDetector;
    [SerializeField] private float circleRadiusGrapple;
    [SerializeField] private LayerMask grappleLayer;
    [SerializeField] private LayerMask grappleObstacleLayers;
    private Collider2D grappleOverlapCircle;
    private Transform targetedGrapplePoint;
    private RaycastHit2D grappleRaycast;
    private Vector2 grappleRaycastDirection;
    private float grappleRaycastDistance;

    //Player Component Reference
    [Header("---Component Reference---")]
    [SerializeField] private Rigidbody2D _rigidbody2D;
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D mainCollider;
    [SerializeField] private GameObject playerGraphic;
    [SerializeField] private PlayerAnimationHandler playerAnimationHandler;
    [SerializeField] private PlayerCombat playerCombat;
    [SerializeField] private PlayerInputTally playerInputTally;

    //Input
    private Vector2 inputDirection;

    //Float
    protected float defaultGravityScale;
    protected float downInputTally;

    //Boolean Conditions
    protected bool jumpInputPressed;
    protected bool horizontalJumpInputReleased;
    protected bool grappleReleased;
    protected bool isMovingRight;

    protected bool isTransitionToAirDash;
    protected bool isAirDashing;
    
    protected bool isJumping;
    protected bool isSuperJumping;
    protected bool isFalling;
    protected bool isLetGO;
    protected bool isClimbingWall;
    protected bool isClimbingLedge;
    protected bool isHanging;
    protected bool isRunning;
    protected bool isDashing;
    protected bool isSubmerged;
    protected bool isGrappling;
    protected bool isKnockedBack;
    protected bool isJumpingOffWall;
    protected bool isGrounded;

    public bool IsGrounded {  get { return isGrounded; } }
    public bool IsMovingRight {  get { return isMovingRight; } }
    public bool IsJumping { get { return isJumping; } }
    public bool IsSuperJumping {  get { return isSuperJumping; } }
    public bool IsFalling { get { return isFalling; } }
    public bool IsClimbingWall { get { return isClimbingWall; } }
    public bool IsClimbingLedge { get { return isClimbingLedge; } }
    public bool IsHanging { get { return isHanging; } }
    public bool IsRunning { get { return isRunning; } }
    public bool IsSubmerged { get { return isSubmerged; } }
    public bool IsDashing {  get { return isDashing; } }
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
        if (Time.timeScale == 0)
            return;

        HandleInput();

        //Overlap Checks
        WallCheck();
        GroundCheck();
        LedgeCheck();
        GrappleCheck();

        //Basic Movement
        VerticalMovement();
        HorizontalMovement();

        //Special Movement
        Submerge();
        Dash();
        //LedgeHang();
        WallJump();
        Pogo();
        Grapple();

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
        if (isClimbingWall == true || isGrappling == true)
            return;

        //Get input from x and y input
        inputDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    //-----------Basic Movement------------//
    private void HorizontalMovement()
    {
        if (isDashing == true || isKnockedBack == true || isJumpingOffWall == true)
            return;

        _rigidbody2D.velocity = new Vector2(inputDirection.x * acceleration, _rigidbody2D.velocity.y);


        if (_rigidbody2D.velocity.x > 0)
        {
            isMovingRight = true;
            isRunning = true;
        }
        else if (_rigidbody2D.velocity.x < 0)
        {
            isMovingRight = false;
            isRunning = true;
        }
        else
        {
            isRunning = false;
        }

        if (inputDirection.x != 0 && isJumping == true) //Maintain momentum when input is gone during jump
        { 
            storedPlayerMomentum = _rigidbody2D.velocity;
            horizontalJumpInputReleased = true;
        }
        else if (horizontalJumpInputReleased == true)
        {
            Debug.Log("Momentum preserved");
            _rigidbody2D.velocity = new Vector2(storedPlayerMomentum.x / 1.3f, _rigidbody2D.velocity.y);
        }
    }

    private void VerticalMovement()
    {
        if (isKnockedBack == true)
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
       
        //--Only allows jump when buffer jump window is active--
        if (bufferJumpTime.CurrentProgress is Cooldown.Progress.InProgress)
        {
            //Jump when grounded or when coyote time is active and jump input is not pressed
            if (isGrounded == true || coyoteTime.CurrentProgress is Cooldown.Progress.InProgress && jumpInputPressed == false)
            {
                if (playerInputTally.DownInputTally > 0)
                {
                    Debug.Log("Super Jump");
                    _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, jumpPower * 1.5f);
                    isSuperJumping = true;
                    jumpInputPressed = true;
                }
                else
                {
                    Debug.Log("Jump");
                    _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, jumpPower);
                    isJumping = true;
                    jumpInputPressed = true;
                }
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
    private void Submerge()
    {
        if (isGrounded == false)
            return;

        if (Input.GetKey(KeyCode.S))
        {
            isSubmerged = true;
        }
        else
        {
            isSubmerged = false;
        }
    }

    private void Dash()
    {
        if (Input.GetKey(KeyCode.K) && dashCooldown.CurrentProgress is Cooldown.Progress.Ready)
        {
            dashDuration.StartCooldown(); //Start duration and cooldown at the same time
            dashCooldown.StartCooldown();
            isDashing = true;
        }

        //Horizontal speed is set to dash power during dash duration
        if (isDashing == true)
        {
            if (inputDirection.x > 0 || isMovingRight == true)
            {
                _rigidbody2D.velocity = new Vector2(dashPower, _rigidbody2D.velocity.y);
            }
            else if (inputDirection.x < 0 || isMovingRight == false)
            {
                _rigidbody2D.velocity = new Vector2(-dashPower, _rigidbody2D.velocity.y);
            }

            _rigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation; //Retain Y-value of player during dash
            storedPlayerMomentum = _rigidbody2D.velocity; //Store velocity value
        }

        //Stop dashing
        if (dashDuration.CurrentProgress is Cooldown.Progress.Finished)
        {
            isDashing = false;
            _rigidbody2D.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
        }

        //Reset duration and cooldown to allow for dashing again
        if (dashCooldown.CurrentProgress is Cooldown.Progress.Finished)
        {
            dashDuration.ResetCooldown();
            dashCooldown.ResetCooldown();
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
        if (isClimbingWall == true)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Wall Jump enabled");
                isClimbingWall = false;
                isJumpingOffWall = true;

                //Unfreeze position
                _rigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionX & RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                _rigidbody2D.gravityScale = defaultGravityScale;

                wallJumpAppliedForceDuration.StartCooldown();
            }
        }
        else if (isJumpingOffWall == true && wallJumpAppliedForceDuration.CurrentProgress is Cooldown.Progress.InProgress)
        {
            if (transform.localScale.x < 0) //Facing left
            {
                Debug.Log("Jumped to left");
                _rigidbody2D.velocity = new Vector2(-wallJumpPower.x, wallJumpPower.y);
            }
            if (transform.localScale.x > 0) //Facing right
            {
                Debug.Log("Jumped to right");
                _rigidbody2D.velocity = new Vector2(wallJumpPower.x, wallJumpPower.y);
            }
            storedPlayerMomentum = _rigidbody2D.velocity;
        }
        else if (wallJumpAppliedForceDuration.CurrentProgress is Cooldown.Progress.Finished)
        {
            _rigidbody2D.velocity = new Vector2(storedPlayerMomentum.x / 1.3f, _rigidbody2D.velocity.y);
            isJumpingOffWall = false;

            if (isGrounded == true || isClimbingWall == true)
            {
                wallJumpAppliedForceDuration.ResetCooldown();
            }
        }
    }

    private void Pogo()
    {
        if (playerCombat.HitObstacle == false || isGrappling == true)
            return;

        if (playerCombat.OverheadAttack == true)
        {
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, -pogoPower);
        }
        else if (playerCombat.LowAttack == true)
        {
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, pogoPower);
            Debug.Log("Pogo Up");
        }
        else if (playerCombat.NeutralAttack == true)
        {
            if (transform.localScale.x > 0)
            {
                Debug.Log("Attack");
                _rigidbody2D.velocity = new Vector2(-pogoPower, _rigidbody2D.velocity.y);
            }
            else if (transform.localScale.x < 0)
            {
                _rigidbody2D.velocity = new Vector2(pogoPower, _rigidbody2D.velocity.y);
            }
        }

        storedPlayerMomentum = _rigidbody2D.velocity; //Store velocity value
    }

    private void Grapple()
    {
        if (isGrappling == true)
        {
            Debug.Log("Grappling");
            Vector2 grappleDirection = (transform.position - targetedGrapplePoint.position).normalized;
            _rigidbody2D.velocity = -grappleDirection * grapplePower;
            storedPlayerMomentum = _rigidbody2D.velocity;
        }

        if (grappleMomentumDuration.CurrentProgress is Cooldown.Progress.InProgress || grappleMomentumDuration.CurrentProgress is Cooldown.Progress.Finished)
        {
            isGrappling = false;
            _rigidbody2D.velocity = new Vector2(storedPlayerMomentum.x / 1.5f, _rigidbody2D.velocity.y);
        }

        if (isGrounded == true && grappleMomentumDuration.CurrentProgress is Cooldown.Progress.Finished || isClimbingWall == true && grappleMomentumDuration.CurrentProgress is Cooldown.Progress.Finished)
        {
            grappleMomentumDuration.ResetCooldown();
            isGrappling = false;
        }
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

    private void KnockedBackState()
    {
        isKnockedBack = true; //Change knockedback bool to true
        Vector2 knockbackDirection = new Vector2(transform.position.x - enemyCollisionPoint.x, 1); //Determine knockback direction by comparing player and enemy position
        _rigidbody2D.velocity = knockbackDirection * knockedbackForce; //Multiply knockback with knockback force
        storedPlayerMomentum = _rigidbody2D.velocity;

        knockedbackTimer.StartCooldown(); //Start cooldown that acts as stun timer

        Debug.Log("Knocked Back");
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

        //Zero gravity during dash
        if (isDashing == true || isGrappling == true)
        {
            _rigidbody2D.gravityScale = 0;
        }
        else if (isGrounded == false && _rigidbody2D.velocity.y <= 1f && _rigidbody2D.velocity.y >= -1f) //Reduce gravity scale at jump apex
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
        if (isGrounded == true || isClimbingWall == true)
            return;

        wallOverlapBox = Physics2D.OverlapBox(wallDetector.position, boxSizeWall, 0, wallLayer);

        if (wallOverlapBox)
        {
            //Requires button input to climb
            if (Input.GetKeyDown(KeyCode.J))
            {
                isClimbingWall = true;
                isJumpingOffWall = false;

                horizontalJumpInputReleased = false;

                _rigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation; //Freeze position

                transform.localScale *= new Vector2(-1, 1); 

                wallJumpAppliedForceDuration.ResetCooldown();
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
        if (isGrounded == true || isHanging == true || isLetGO == true || isClimbingLedge == true)
            return;

        //Creates a box that returns true when overlapping with groundLayer


        if (Physics2D.OverlapBox(ledgeDetector.position, boxSizeLedge, 0, groundLayer))
        {
            Debug.Log("Near Ledge");
            //Requires button input to hang
            if (Input.GetKeyDown(KeyCode.N)) 
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

    private void GrappleCheck()
    {
        grappleOverlapCircle = Physics2D.OverlapCircle(grappleDetector.position, circleRadiusGrapple, grappleLayer);

        if (grappleOverlapCircle == false)
        {
            isGrappling = false;
            targetedGrapplePoint = null;
            return;
        }
        else
        {
            Debug.Log("Near grapple point");
        }
            
        grappleRaycastDistance = circleRadiusGrapple;
        grappleRaycastDirection = grappleOverlapCircle.transform.position - grappleDetector.position;
        grappleRaycast = Physics2D.Raycast(grappleDetector.position, grappleRaycastDirection, grappleRaycastDistance, grappleObstacleLayers);

        
        if (grappleRaycast)
        {
            Debug.Log("Grapple point blocked");
            isGrappling = false;
            targetedGrapplePoint = null;
            return;
        }
        else
        {
            Debug.Log("Can grapple to point");
            if (targetedGrapplePoint == null)
            {
                targetedGrapplePoint = grappleOverlapCircle.transform;
            }

            if (Input.GetKey(KeyCode.T) && grappleMomentumDuration.CurrentProgress is Cooldown.Progress.Ready)
            {
                Debug.Log("Input to grapple");
                isGrappling = true;
            }
        }
    }

    private void GroundCheck()
    {
        //Creates a box that returns true when overlapping with groundLayer
        groundBoxcast = Physics2D.BoxCast(transform.position, boxSizeGround, 0, -transform.up, castDistanceGround, groundLayer);

        if (groundBoxcast)
        {
            //Reset all values to default
            isGrounded = true;
            isJumping = false;
            isJumping = false;
            isFalling = false;
            isLetGO = false;
            isClimbingLedge = false;
            isClimbingWall = false;
            isJumpingOffWall = false;

            jumpInputPressed = false;
            horizontalJumpInputReleased = false;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            enemyCollisionPoint = collision.transform.position;
            KnockedBackState();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Check for player trigger with grappling point
        if (collision.CompareTag("GrapplePoint"))
        {
            grappleMomentumDuration.StartCooldown();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
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
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(grappleDetector.position, circleRadiusGrapple); //Grapple wire sphere
        Gizmos.DrawRay(grappleDetector.position, grappleRaycastDirection); //Grapple ray
    }

    
}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //Gizmo Toggle
    [Header("===Gizmo Configuration===")]
    [SerializeField] protected bool gizmoToggleOn = true;

    //Variables
    [Header("===Player Configuration===")]
    [Header("---Run---")]
    [SerializeField] protected float speedForwards;
    [SerializeField] protected float speedBackwards;
    [Header("---Dash---")]
    [SerializeField] protected float dashPower;
    [SerializeField] protected Cooldown dashDuration;
    [SerializeField] protected Cooldown dashCooldown;
    [Header("---Jump---")]
    [SerializeField] protected float jumpPower;
    [SerializeField] protected Cooldown coyoteTime;
    [SerializeField] protected Cooldown bufferJumpTime;
    [Header("---Pogo Jump---")]
    [SerializeField] protected float pogoPower;
    [Header("---Wall Jump---")]
    [SerializeField] protected Vector2 wallJumpPower;
    [SerializeField] protected Cooldown wallJumpAppliedForceDuration;
    [Header("---Grapple---")]
    [SerializeField] protected float grapplePower;
    [SerializeField] protected Cooldown linkToGrapplePointTime;
    [Header("---Gravity---")]
    [SerializeField] protected float jumpApexGravityDivider;
    [SerializeField] protected float fallingGravityMultiplier;
    [Header("---Limiter---")]
    [SerializeField] protected float fallingSpeedLimit;
    [SerializeField] protected float maxJumpForce;

    //States
    [Header("---Knockback State---")]
    [SerializeField] private float knockedbackForce;
    [SerializeField] private Cooldown knockedbackTimer;
    private Vector2 enemyCollisionPoint;

    //Collision Check
    [Header("===Collision/Trigger Checks Configuration===")]
    [Header("---Ground Check---")]
    [SerializeField] private float castDistanceGround;
    [SerializeField] private Vector2 boxSizeGround;
    [SerializeField] private LayerMask groundLayer;
    private RaycastHit2D groundBoxcast;
    [Header("---Submerge Overhead Check---")]
    [SerializeField] private Transform submergeOverheadDetector;
    [SerializeField] private Vector2 boxSizeSubmergeOverhead;
    [SerializeField] private LayerMask submergeOverheadDetectableLayer;
    private Collider2D submergeOverheadOverlapBox;
    //[Header("---Wall Check---")]
    //[SerializeField] private Transform wallCheck;
    //[SerializeField] private Vector2 boxSizeWallCheck;
    //[SerializeField] private LayerMask wallLayer;
    //private Collider2D wallOverlapBoxFront;
    //private Collider2D wallOverlapBoxBack;
    [Header("---Grapple Check---")]
    [SerializeField] private Transform grappleDetector;
    [SerializeField] private float grappleRadius;
    [SerializeField] private float grappleInvalidRadius;
    [SerializeField] private LayerMask grappleLayer;
    [SerializeField] private LayerMask grappleObstacleLayers;
    private float grappleRaycastDistance;
    private Vector2 grappleRaycastDirection;
    private RaycastHit2D grappleRaycast;
    private Transform targetedGrapplePoint;
    private Collider2D grappleOverlapCircle;
    private Collider2D grappleInvalidCircle;

    //Player Component Reference
    [Header("---Component Reference---")]
    [SerializeField] protected Rigidbody2D playerRigidbody;
    [SerializeField] protected Collider2D mainCollider;
    [SerializeField] protected GameObject playerGraphic;
    [SerializeField] protected PlayerCombat playerCombat;

    //Vectors
    protected Vector2 savedScale;
    protected Vector2 inputDirection;
    protected Vector2 knockbackDirection;
    protected Vector2 storedPlayerMomentum;

    //Float
    protected float defaultGravityScale;
    protected float downInputTally;

    //Boolean Variables
    protected bool jumpInputPressed;
    protected bool keepGrappleMomentum;
    protected bool obstacleAboveSubmerge;
    protected bool tooCloseToGrapplePoint;

    protected bool isFacingRight;
    protected bool isMovingForward;
    protected bool isMovingBackward;
    protected bool isJumping;
    protected bool isSuperJumping;
    protected bool isJumpingOffWall;
    protected bool isDashingForward;
    protected bool isDashingBackward;
    protected bool isFalling;
    protected bool isClimbingWall;
    protected bool isSubmerged;
    protected bool isGrappling;
    protected bool isKnockedBack;
    protected bool isGrounded;
   
    //Public References
    public bool IsFacingRight {  get { return isFacingRight; } }
    public bool IsMovingForward { get { return isMovingForward; } }
    public bool IsMovingBackward { get { return isMovingBackward; } }
    public bool IsJumping { get { return isJumping; } }
    public bool IsSuperJumping {  get { return isSuperJumping; } }
    public bool IsJumpingOffWall { get { return isJumpingOffWall; } }
    public bool IsDashingForward {  get { return isDashingForward; } }
    public bool IsDashingBackward {  get { return isDashingBackward; } }
    public bool IsFalling { get { return isFalling; } }
    public bool IsClimbingWall { get { return isClimbingWall; } }  
    public bool IsSubmerged { get { return isSubmerged; } }
    public bool IsGrappling { get { return isGrappling; } }
    public bool IsGrounded { get { return isGrounded; } }

    public Vector2 InputDirection { get { return inputDirection; } }


    private void Start()
    {
        if (playerRigidbody == null)
            Debug.LogWarning("Player Rigidbody2D not found");

        defaultGravityScale = playerRigidbody.gravityScale;
        savedScale = transform.localScale;
    }

    private void Update()
    {
        if (Time.timeScale == 0)
            return;

        HandleInput();

        //Overlap Checks
        //WallCheck();
        GroundCheck();
        SubmergeOverheadCheck();
        GrappleCheck();

        //Basic Movement
        VerticalMovement();
        HorizontalMovement();

        //Special Movement
        Submerge();
        Dash();
        //WallJump();
        Pogo();
        Grapple();

        RigidbodyManipulator();
        KnockedBackState();
        //State Check
        //CheckState();
        
    }

    private void FixedUpdate()
    {

    }

    //-------------------------------------------------------- EDITOR DISPLAY --------------------------------------------------------//
    //==================== GIZMOS ====================//
    private void OnDrawGizmos()
    {
        if (gizmoToggleOn != true)
            return;

        //Makes the Check Box Visible
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position - transform.up * castDistanceGround, boxSizeGround); //Ground wire cube
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(submergeOverheadDetector.position, boxSizeSubmergeOverhead); //Submerge Overhead wire cube
        Gizmos.color = Color.green;
        //Gizmos.DrawWireCube(wallCheck.position, boxSizeWallCheck); //Wall Check wire cube
        //Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(grappleDetector.position, grappleRadius); //Grapple wire sphere
        Gizmos.DrawRay(grappleDetector.position, grappleRaycastDirection); //Grapple ray
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(grappleDetector.position, grappleInvalidRadius); //Grapple wire sphere
    }
    //-------------------------------------------------------- GENERAL FUNCTIONS --------------------------------------------------------//
    //==================== HANDLE INPUT ====================//
    private void HandleInput()
    {
        if (isClimbingWall == true || isGrappling == true)
            return;
        
        //Prevent movement when attacking
        if (playerCombat.NeutralAttack == true)
        {
            inputDirection = Vector2.zero;
            return;
        }
            
        //Get input from x and y input
        inputDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    //==================== FLIP CHECK ====================//
    protected void FlipCheck()
    {
        //Flip X
        if (inputDirection.x > 0)
        {
            isFacingRight = true;
            transform.localScale = savedScale;
            Debug.Log("Player now facing right");
        }
        else if (inputDirection.x < 0)
        {
            isFacingRight = false;
            transform.localScale = new Vector2(savedScale.x * -1, savedScale.y);
            Debug.Log("Player now facing left");
        }
    }

    //==================== PLAYER KNOCKEDBACK ====================//
    private void KnockedBackState()
    {
        if (isKnockedBack == false)
            return;

        if (knockedbackTimer.CurrentProgress is Cooldown.Progress.Ready)
        {
            isKnockedBack = true;
            knockedbackTimer.StartCooldown(); //Start cooldown that acts as stun timer
            //Determine knockback direction by comparing player and enemy position
            knockbackDirection = new Vector2(transform.position.x - enemyCollisionPoint.x, 0); 
        }
        if (knockedbackTimer.CurrentProgress is Cooldown.Progress.InProgress)
        {
            if (knockbackDirection.x > 0)
            {
                playerRigidbody.velocity = new Vector2(knockedbackForce, 0); 
            }
            else if (knockbackDirection.x < 0)
            {
                playerRigidbody.velocity = new Vector2(-knockedbackForce, 0);
            }
            storedPlayerMomentum = playerRigidbody.velocity;
        }
        if (knockedbackTimer.CurrentProgress is Cooldown.Progress.Finished)
        {
            isKnockedBack = false;
            knockedbackTimer.ResetCooldown();
        }

        Debug.Log("Knocked Back");
    }

    //==================== RIGIDBODY MANIPULATOR ====================//
    private void RigidbodyManipulator()
    {
        Vector2 maxFallVelocity = playerRigidbody.velocity;
        if (playerRigidbody.velocity.y > fallingSpeedLimit && isFalling == true)
        {
            maxFallVelocity.y = Mathf.Clamp(maxFallVelocity.y, fallingSpeedLimit, maxJumpForce);
            playerRigidbody.velocity = maxFallVelocity;
        }

        //Check if player is falling
        if (playerRigidbody.velocity.y <= 0f && isGrounded == false)
        {
            isFalling = true;
            isJumping = false;
            isSuperJumping = false;
            Debug.Log("Player falling");
        }
        else
        {
            isFalling = false;
        }

        if (dashDuration.CurrentProgress is Cooldown.Progress.Finished && keepGrappleMomentum == false && isGrappling == false) //Freeze player after dash for short duration for cleaner effect
        {
            playerRigidbody.velocity = new Vector2(storedPlayerMomentum.x / 2, playerRigidbody.velocity.y);
        }

        //Zero gravity
        if (isDashingForward == true || isDashingBackward == true || isGrappling == true)
        {
            playerRigidbody.gravityScale = 0;
        }
        else if (isGrounded == false && playerRigidbody.velocity.y <= 1f && playerRigidbody.velocity.y >= -1f) //Reduce gravity scale at jump apex
        {
            //Debug.Log("Current gs: " + _rigidbody2D.gravityScale);
            playerRigidbody.gravityScale = defaultGravityScale / jumpApexGravityDivider;
        }
        else if (isFalling == true) //Increase gravity scale when falling
        {
            playerRigidbody.gravityScale = defaultGravityScale * fallingGravityMultiplier;
        }
        else
        {
            playerRigidbody.gravityScale = defaultGravityScale;
        }
    }

    //==================== GROUND CHECK ====================//
    private void GroundCheck()
    {
        //Creates a box that returns true when overlapping with groundLayer
        groundBoxcast = Physics2D.BoxCast(transform.position, boxSizeGround, 0, -transform.up, castDistanceGround, groundLayer);

        if (groundBoxcast)
        {
            //Reset all values to default
            isGrounded = true;
            isJumping = false;
            isSuperJumping = false;
            isFalling = false;
            isClimbingWall = false;
            isJumpingOffWall = false;

            jumpInputPressed = false;
        }
        else
        {
            isGrounded = false;
            isSubmerged = false;
        }
    }

    //==================== GENERAL PLAYER COLLISION CHECKS ====================//
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            enemyCollisionPoint = collision.transform.position;
            isKnockedBack = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("GrapplePoint") && isGrappling == true)
        {
            isGrappling = false;
            keepGrappleMomentum = true;
            Debug.Log("Leaving grapple point");
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

    }


    private void OnTriggerExit2D(Collider2D collision)
    {

    }

    //-------------------------------------------------------- MOVEMENT FUNCTIONS --------------------------------------------------------//
    //==================== HORIZONTAL MOVEMENT ====================//
    private void HorizontalMovement()
    {
        if (isJumping == true || isSuperJumping == true || isDashingForward == true || isDashingBackward == true || isFalling == true || isKnockedBack == true || isJumpingOffWall == true || isGrappling == true)
            return;

        //Move player with varying speed based on forward or backward movement
        if (isMovingBackward == true)
        {
            playerRigidbody.velocity = new Vector2(inputDirection.x * speedBackwards, playerRigidbody.velocity.y);

        }
        else
        {
            playerRigidbody.velocity = new Vector2(inputDirection.x * speedForwards, playerRigidbody.velocity.y);
        }

        //Lock player flip when moving
        if (playerCombat.FlipLocked == true)
        {
            if (playerRigidbody.velocity.x > 0.1f && isFacingRight == false || playerRigidbody.velocity.x < -0.1f && isFacingRight == true) //Move backwards
            {
                isMovingForward = false;
                isMovingBackward = true;
                Debug.Log("Player running backwards");
            }
            else if (playerRigidbody.velocity.x > 0.1f && isFacingRight == true || playerRigidbody.velocity.x < -0.1f && isFacingRight == false)
            {
                isMovingForward = true;
                isMovingBackward = false;
            }
            else
            {
                isMovingForward = false;
                isMovingBackward = false;
            }
        }
        else if (playerRigidbody.velocity.x > 0.1f || playerRigidbody.velocity.x < -0.1f) //Walking normally
        {
            isMovingForward = true;
            isMovingBackward = false;
            FlipCheck();
            Debug.Log("Player running forwards");
            Debug.Log(playerRigidbody.velocity.x);
        }
        else //Not walking
        {
            isMovingForward = false;
            isMovingBackward = false;
        }
    }

    //==================== VERTICAL MOVEMENT ====================//
    private void VerticalMovement()
    {
        if (isKnockedBack == true)
            return;
        
        //--Buffer Jump--
        if (Input.GetKeyDown(KeyCode.Space))
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
                if (isSubmerged == true) //Super Jump
                {
                    Debug.Log("Super Jump");
                    playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, jumpPower * 1.3f);
                    isSuperJumping = true;
                    jumpInputPressed = true;
                }
                else //Normal Jump
                {
                    Debug.Log("Jump");
                    playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, jumpPower);
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

    //==================== SUBMERGE ====================//
    private void Submerge()
    {
        if (isGrounded == false || isDashingForward == true || isDashingBackward == true)
            return;

        if (Input.GetKey(KeyCode.S)) //Hold input to submerge
        {
            isSubmerged = true;
            Debug.Log("Submerged");
        }
        else if (obstacleAboveSubmerge == false) //Stay submerged if object is above player
        {
            isSubmerged = false;
        }
    }

    private void SubmergeOverheadCheck()
    {
        if (isSubmerged == false)
            return;

        submergeOverheadOverlapBox = Physics2D.OverlapBox(submergeOverheadDetector.position, boxSizeSubmergeOverhead, 0, submergeOverheadDetectableLayer);

        if (submergeOverheadOverlapBox)
        {
            if (isSubmerged)
            {
                obstacleAboveSubmerge = true;
            }
        }
        else
        {
            obstacleAboveSubmerge = false;
        }
    }

    //==================== DASH ====================//
    private void Dash()
    {
        if (isGrappling == true || isSuperJumping == true || isSubmerged == true)
            return;

        if (Input.GetKey(KeyCode.K) && dashCooldown.CurrentProgress is Cooldown.Progress.Ready) //Begin dash duration and cooldown
        {
            dashDuration.StartCooldown(); //Start duration and cooldown at the same time
            dashCooldown.StartCooldown();
            isGrappling = false;
        }

        //Horizontal speed is set to dash power during dash duration
        if (dashDuration.CurrentProgress is Cooldown.Progress.InProgress && isDashingForward == false && isDashingBackward == false)
        {
            //Check for backdash or forward dash
            if (isFacingRight == true)
            {
                if (inputDirection.x < 0)
                {
                    playerRigidbody.velocity = new Vector2(-dashPower / 1.2f, playerRigidbody.velocity.y);
                    isDashingBackward = true;
                    Debug.Log("Dashing backwards");
                }
                else
                {
                    playerRigidbody.velocity = new Vector2(dashPower, playerRigidbody.velocity.y);
                    isDashingForward = true;
                    Debug.Log("Dashing forwards");
                }
            }
            else if (isFacingRight == false)
            {
                if (inputDirection.x > 0)
                {
                    playerRigidbody.velocity = new Vector2(dashPower / 1.2f, playerRigidbody.velocity.y);
                    isDashingBackward = true;
                    Debug.Log("Dashing backwards");
                }
                else
                {
                    playerRigidbody.velocity = new Vector2(-dashPower, playerRigidbody.velocity.y);
                    isDashingForward = true;
                    Debug.Log("Dashing forwards");
                }
            }

            storedPlayerMomentum = playerRigidbody.velocity; //Store momentum value
            playerRigidbody.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation; //Retain Y-value of player during dash
        }

        if (isDashingBackward == true) //Backdash has shorter duration for shorter dash distance
        {
            if (dashDuration.CurrentDuration < dashDuration.Duration / 1.8f) //Stop backdash after specific dash duration is reached
            {
                dashDuration.EndCooldown();
                Debug.Log("Backdash ended");
            }
        }

        //Stop dashing
        if (dashDuration.CurrentProgress is Cooldown.Progress.Finished)
        {
            isDashingForward = false;
            isDashingBackward = false;
            playerRigidbody.constraints &= ~RigidbodyConstraints2D.FreezePositionY;

            if (isGrounded == true) //Reset duration after landing
            {
                dashDuration.ResetCooldown();
            }
        }

        //Reset cooldown when finished and grounded
        if (dashCooldown.CurrentProgress is Cooldown.Progress.Finished && isGrounded == true)
        {
            dashCooldown.ResetCooldown();
        }
    }

    //==================== WALL CLING ====================//
    //private void WallJump()
    //{
    //    if (isClimbingWall == true)
    //    {
    //        if (Input.GetKeyDown(KeyCode.Space))
    //        {
    //            Debug.Log("Wall Jump enabled");
    //            isClimbingWall = false;
    //            isJumpingOffWall = true;

    //            //Unfreeze position
    //            playerRigidbody.constraints = RigidbodyConstraints2D.FreezePositionX & RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
    //            playerRigidbody.gravityScale = defaultGravityScale;

    //            wallJumpAppliedForceDuration.StartCooldown();
    //        }
    //    }
    //    else if (isJumpingOffWall == true && wallJumpAppliedForceDuration.CurrentProgress is Cooldown.Progress.InProgress)
    //    {
    //        if (transform.localScale.x < 0) //Facing left
    //        {
    //            Debug.Log("Jumped to left");
    //            playerRigidbody.velocity = new Vector2(-wallJumpPower.x, wallJumpPower.y);
    //        }
    //        if (transform.localScale.x > 0) //Facing right
    //        {
    //            Debug.Log("Jumped to right");
    //            playerRigidbody.velocity = new Vector2(wallJumpPower.x, wallJumpPower.y);
    //        }
    //        storedPlayerMomentum = playerRigidbody.velocity;
    //    }
    //    else if (wallJumpAppliedForceDuration.CurrentProgress is Cooldown.Progress.Finished)
    //    {
    //        playerRigidbody.velocity = new Vector2(storedPlayerMomentum.x / 1.3f, playerRigidbody.velocity.y);
    //        isJumpingOffWall = false;

    //        if (isGrounded == true || isClimbingWall == true)
    //        {
    //            wallJumpAppliedForceDuration.ResetCooldown();
    //        }
    //    }
    //}

    //private void WallCheck()
    //{
    //    if (isGrounded == true || isClimbingWall == true)
    //        return;

    //    wallOverlapBoxFront = Physics2D.OverlapBox(wallCheck.position, boxSizeWallCheck, 0, wallLayer);

    //    if (wallOverlapBoxFront)
    //    {
    //        isClimbingWall = true;
    //        //Requires button input to climb
    //        if (Input.GetKeyDown(KeyCode.U))
    //        {
    //            isJumpingOffWall = false;

    //            playerRigidbody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation; //Freeze position

    //            transform.localScale *= new Vector2(-1, 1);

    //            wallJumpAppliedForceDuration.ResetCooldown();
    //            Debug.Log("Climbing Wall");
    //        }
    //    }
    //    else
    //    {
    //        isClimbingWall = false;
    //    }
    //}

    //==================== GRAPPLE ====================//
    private void Grapple()
    {
        if (isGrappling == true)
        {
            if (linkToGrapplePointTime.CurrentProgress is Cooldown.Progress.Ready) //Pause in position for grapple animation
            {
                linkToGrapplePointTime.StartCooldown();

                playerRigidbody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
            }
            else if (linkToGrapplePointTime.CurrentProgress is Cooldown.Progress.Finished) //Unpause after animation duration
            {
                storedPlayerMomentum = Vector2.zero;
                playerRigidbody.constraints = RigidbodyConstraints2D.FreezePositionX & RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;

                Debug.Log("Start Grappling");

                if (targetedGrapplePoint == null)
                {
                    storedPlayerMomentum = playerRigidbody.velocity;
                    return;
                }

                Vector2 grappleDirection = (transform.position - targetedGrapplePoint.position).normalized;
                playerRigidbody.velocity = -grappleDirection * grapplePower;
                storedPlayerMomentum = playerRigidbody.velocity;
            }
        }

        //Maintain momentum after colliding with grapple point (Refer to "PLAYER GENERAL COLLISION CHECK > ONTRIGGERENTER2D")
        if (keepGrappleMomentum == true)
        {
            playerRigidbody.velocity = new Vector2(storedPlayerMomentum.x / 1.5f, playerRigidbody.velocity.y);
            Debug.Log("Let go of grapple point");
        }

        if ((isGrounded == true || isClimbingWall == true) && keepGrappleMomentum == true)
        {
            linkToGrapplePointTime.ResetCooldown();
            keepGrappleMomentum = false;
        }
    }

    private void GrappleCheck()
    {
        if (isDashingForward == true || isDashingBackward == true)
            return;

        grappleOverlapCircle = Physics2D.OverlapCircle(grappleDetector.position, grappleRadius, grappleLayer);
        grappleInvalidCircle = Physics2D.OverlapCircle(grappleDetector.position, grappleInvalidRadius, grappleLayer);

        if (grappleOverlapCircle == false) //Detection radius for grapple
        {
            targetedGrapplePoint = null;
            return;
        }
        else if (grappleInvalidCircle) //Detection radius to check if player is too close to a grapple point
        {
            tooCloseToGrapplePoint = true;
        }
        else
        {
            tooCloseToGrapplePoint = false;
            Debug.Log("Near grapple point");
        }

        grappleRaycastDistance = grappleRadius;
        grappleRaycastDirection = grappleOverlapCircle.transform.position - grappleDetector.position;
        grappleRaycast = Physics2D.Raycast(grappleDetector.position, grappleRaycastDirection, grappleRaycastDistance, grappleObstacleLayers);


        if (grappleRaycast) //Vision line in detection radius to check for obstacles
        {
            Debug.Log("Grapple point blocked");
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

            if (Input.GetKey(KeyCode.L) && tooCloseToGrapplePoint == false) //Grapple when not too close to grapple point
            {
                if (keepGrappleMomentum == false && isGrappling == false)
                {
                    Debug.Log("Input to grapple");
                    isGrappling = true;
                }
            }
        }
    }

    //==================== POGO ====================//
    private void Pogo()
    {
        if (playerCombat.HitObstacle == false || isGrappling == true)
            return;

        //Y velocity power set to opposite direction of attack contact point
        if (playerCombat.AirOverheadAttack == true)
        {
            playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, -pogoPower);
        }
        else if (playerCombat.AirLowAttack == true)
        {
            playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, pogoPower);
            Debug.Log("Pogo Up");
        }
        else if (playerCombat.NeutralAttack == true)
        {
            if (transform.localScale.x > 0)
            {
                Debug.Log("Attack");
                playerRigidbody.velocity = new Vector2(-pogoPower, playerRigidbody.velocity.y);
            }
            else if (transform.localScale.x < 0)
            {
                playerRigidbody.velocity = new Vector2(pogoPower, playerRigidbody.velocity.y);
            }
        }
    }

    

    //===========================================================================================================//
}

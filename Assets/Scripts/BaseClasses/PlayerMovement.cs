using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : Movement
{
    [Header("========== Additional Configuration ==========")]

    // Dash Configuration
    [Header("--- Dash ---")]
    [SerializeField] protected float dashPower;
    [SerializeField] protected Cooldown dashDuration;
    [SerializeField] protected Cooldown dashCooldown;

    //// Pogo Configuration
    //[Header("--- Pogo ---")]
    //[SerializeField] protected float pogoPower;

    //// Wall Jump Configuration
    //[Header("--- Wall Jump ---")]
    //[SerializeField] protected Vector2 wallJumpPower;
    //[SerializeField] protected Cooldown wallJumpAppliedForceDuration;

    //// Grapple Configuration
    //[Header("--- Grapple ---")]
    //[SerializeField] protected float grapplePower;
    //[SerializeField] protected Cooldown linkToGrapplePointTime;

    //Additional Jump Checks
    [Header("--- Additional Jump Checks ---")]
    [SerializeField] protected Cooldown coyoteTime;
    [SerializeField] protected float bufferJumpCastDistance;
    [SerializeField] protected Cooldown bufferJumpWindow;
    [SerializeField] protected Transform bufferJumpTransform;
    [SerializeField] protected Vector2 bufferJumpBoxSize;
    [SerializeField] protected LayerMask bufferJumpLayer;
    protected RaycastHit2D bufferJumpBoxcast;


    // Collision Checks
    [Header("--- Submerge Overhead Check ---")]
    [SerializeField] protected float overheadCastDistance;
    [SerializeField] protected Transform overheadTransform;
    [SerializeField] protected Vector2 overheadBoxSize;
    [SerializeField] protected LayerMask overheadCheckLayer;
    protected RaycastHit2D overheadOverlapBox;

    // Movement States
    protected bool isInputLeft;
    protected bool isInputRight;
    protected bool isDashing;
    protected bool isDashingForward;
    protected bool isDashingBackward;
    protected bool isSubmerging;
    protected bool isSubmergingForward;
    protected bool isSubmergingBackward;
    protected bool isGrappling;
    protected bool isOnWall;

    protected bool objectAbove;
    protected bool coyoteStandbyActive;
    protected bool bufferedJump;

    // Input Handling
    private Vector2 inputDirection;

    protected bool IsInputLeft { get { return isInputLeft; } }
    protected bool IsInputRight { get { return isInputRight; } }

    // Component References
    //[SerializeField] protected PlayerStatus status;
    //[SerializeField] protected PlayerStats stats;

    //============================================= GIZMO =============================================//
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.DrawWireCube(overheadTransform.position + transform.up * (overheadCastDistance / 2), overheadBoxSize * 2);
        Gizmos.DrawLine(overheadTransform.position, overheadTransform.position + transform.up * overheadCastDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(bufferJumpTransform.position + -transform.up * (bufferJumpCastDistance / 2), bufferJumpBoxSize * 2);
        Gizmos.DrawLine(bufferJumpTransform.position, bufferJumpTransform.position + -transform.up * bufferJumpCastDistance);
    }

    //============================================= INPUT =============================================//
    protected void HandleInput()
    {
        inputDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    //============================================= LIFE CYCLE =============================================//
    protected override void Start()
    {
        base.Start();

        if (animationHandler == null)
        {
            animationHandler = GetComponent<PlayerAnimationHandler>();
        }
        if (animationHandler == null)
        {
            Debug.Log("Component Animation Handler not found");
            return;
        }
    }

    protected override void Update()
    { 
        base.Update();
        BufferJumpCheck();
        SubmergeOverheadCheck();

        CoyoteTime();
        Debug.Log(coyoteStandbyActive);
        HandleInput();
        HorizontalMovement();
        VerticalMovement();
        Dash();
        Submerge();

    }

    //============================================= COLLISION CHECK =============================================//
    protected override void GroundCheck()
    {
        base.GroundCheck();
        if (groundBoxcast)
        {
            isGrounded = true;
            isJumping = false;
            isFalling = false;

            coyoteStandbyActive = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    protected void BufferJumpCheck()
    {
        bufferJumpBoxcast = Physics2D.BoxCast(bufferJumpTransform.position, bufferJumpBoxSize, 0, -transform.up, bufferJumpCastDistance, bufferJumpLayer);

        if (bufferJumpBoxcast)
        {
            if (Input.GetKeyDown(KeyCode.Space) && isFalling) 
            {
                Debug.Log("Buffered");
                bufferedJump = true;
            }
        }
    }

    protected void SubmergeOverheadCheck()
    {
        if (isSubmerging)
            return;

        overheadOverlapBox = Physics2D.BoxCast(overheadTransform.position, overheadBoxSize, 0, transform.up, overheadCastDistance, overheadCheckLayer);
        objectAbove = overheadOverlapBox;
    }

    //============================================= COOLDOWN/DURATION =============================================//
    protected void CoyoteTime()
    {
        if (isJumping)
            coyoteStandbyActive = false;

        if (!isGrounded && coyoteStandbyActive && coyoteTime.CurrentProgress == Cooldown.Progress.Ready)
        {
            coyoteTime.StartCooldown();
        }

        if (isGrounded && coyoteTime.CurrentProgress == Cooldown.Progress.Finished)
        {
            coyoteTime.ResetCooldown();
        }
    }

    //============================================= BASIC MOVEMENT =============================================//
    protected override void UpdateMovementStates()
    {
        base.UpdateMovementStates();
        isInputLeft = inputDirection.x < 0;
        isInputRight = inputDirection.x > 0;
        isDashing = dashCooldown.CurrentProgress == Cooldown.Progress.InProgress;
        isDashingForward = isDashing && ((isFacingRight && isInputRight) || (!isFacingRight && isInputLeft));
        isDashingBackward = isDashing && ((isFacingRight && isInputLeft) || (!isFacingRight && isInputRight));
        //isSubmerging = ;
        isSubmergingForward = isSubmerging && isMovingForward;
        isSubmergingBackward = isSubmerging && !isMovingForward;

    }

    protected override void HorizontalMovement()
    {
        attachedRigidbody.velocity = new Vector2(inputDirection.x * speedForwards, attachedRigidbody.velocity.y);
    }

    protected override void VerticalMovement()
    {
        bool returnCondition = isJumping;
       

        if (returnCondition)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded || coyoteTime.CurrentProgress == Cooldown.Progress.InProgress)
            {
                attachedRigidbody.velocity = new Vector2(attachedRigidbody.velocity.x, jumpPower);

                bufferedJump = false;
            }
        }

        

        if (isGrounded && bufferedJump)
        {
            attachedRigidbody.velocity = new Vector2(attachedRigidbody.velocity.x, jumpPower);

            coyoteStandbyActive = false;
            bufferedJump = false;
        }
    }

    //============================================= SPECIAL MOVEMENT =============================================//
    protected void Dash()
    {
        bool returnConditions = isSubmerging;

        if (!returnConditions)
            return;

        if (Input.GetKey(KeyCode.K) && dashCooldown.CurrentProgress == Cooldown.Progress.Ready)
        {
            dashDuration.StartCooldown();
            dashCooldown.StartCooldown();
        }

        if (isDashing)
        {
            float dashDirection = 0;

            if (isInputRight)
            {
                dashDirection = 1;
            }
            else
            {
                dashDirection = -1;
            }

            if (isDashingBackward)
            {
                dashDirection *= -1; // Reverse direction for backdash
            }

            attachedRigidbody.velocity = new Vector2(dashDirection * dashPower, attachedRigidbody.velocity.y);
        }

        // Stop backdash sooner for a quick stop
        if (isDashingBackward && dashDuration.CurrentDuration < dashDuration.Duration / 1.8f)
        {
            dashDuration.EndCooldown();
            Debug.Log("Backdash ended early");
        }

        // End dash when duration is finished
        if (dashDuration.CurrentProgress == Cooldown.Progress.Finished)
        {
            dashDuration.ResetCooldown();
        }

        // Reset cooldown when finished and grounded
        if (isGrounded && dashCooldown.CurrentProgress == Cooldown.Progress.Finished)
        {
            dashCooldown.ResetCooldown();
        }
    }

    protected void Submerge() //**CHANGE TO CONSUMING METER BASED** 
    {
        if (Input.GetKey(KeyCode.S)) //Hold input to submerge
        {
            isSubmerging = true;
            Debug.Log("Submerged");
        }
        else if (objectAbove == false) //Stay submerged if object is above player
        {
            isSubmerging = false;
        }
    }

    protected void Pogo()
    {
        // TODO: Implement pogo logic
    }

    

    //============================================= DISPLACEMENT EFFECTS =============================================//
}
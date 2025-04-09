using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : Movement
{
    [Header("========== Additional Configuration ==========")]

    // Dash Configuration
    [Header("--- Dash ---")]
    [SerializeField] protected float dashPower;
    [SerializeField] protected Timer dashDuration;
    [SerializeField] protected Timer dashCooldown;

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
    [SerializeField] protected Timer coyoteTime;
    [SerializeField] protected float bufferJumpCastDistance;
    [SerializeField] protected Timer bufferJumpWindow;
    [SerializeField] protected Transform bufferJumpTransform;
    [SerializeField] protected Vector2 bufferJumpBoxSize;
    [SerializeField] protected LayerMask bufferJumpLayer;
    protected RaycastHit2D bufferJumpBoxcast;


    // Collision Checks
    [Header("--- Submerge Overhead Check ---")]
    [SerializeField] protected float maxSubmergeMeter;
    [SerializeField] protected float submergeConsumptionPerSecond;
    [SerializeField] protected float overheadCastDistance;
    [SerializeField] protected Transform overheadTransform;
    [SerializeField] protected Vector2 overheadBoxSize;
    [SerializeField] protected LayerMask overheadCheckLayer;
    protected RaycastHit2D overheadOverlapBox;

    [SerializeField] protected LayerMask playerLayer;
    [SerializeField] protected LayerMask enemyLayer;

    // Movement States
    protected bool inputLeft;
    protected bool inputRight;

    protected bool superJumping;
    protected bool dashing;
    protected bool dashingForward;
    protected bool dashingBackward;
    protected bool airDashingForward;
    protected bool airDashingBackward;
    protected bool submerging;
    protected bool submergingForward;
    protected bool submergingBackward;
    protected bool grappling;

    protected bool objectAbove;
    protected bool coyoteStandbyActive;
    protected bool bufferedJump;

    public bool Dashing { get { return dashing; } }
    public bool DashingForward { get { return dashingForward; } }
    public bool DashingBackward { get { return dashingBackward; } }
    public bool AirDashingForward {  get { return airDashingForward; } }
    public bool AirDashingBackward {  get { return airDashingBackward; } }
    public bool Submerging { get { return submerging; } }
    public bool SubmergingForward { get { return submergingForward; } }
    public bool SubmergingBackward { get { return submergingBackward; } }
    public bool Grappling { get { return grappling; } }

    protected PlayerInputTracker inputTracker;

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

    //============================================= LIFE CYCLE =============================================//
    protected override void Start()
    {
        base.Start();

        if (animationHandler == null)
        {
            animationHandler = GetComponent<PlayerAnimationHandler>();
        }

        if (inputTracker == null)
        {
            inputTracker = GetComponent<PlayerInputTracker>();
        }

        if (combat == null)
        {
            combat = GetComponent<PlayerCombat>();
        }
    }

    protected override void Update()
    { 
        base.Update();
        BufferJumpCheck();
        SubmergeOverheadCheck();

        CoyoteTime();

        Dash();
        Submerge();
    }

    //============================================= COLLISION CHECK =============================================//
    protected override void GroundCheck()
    {
        base.GroundCheck();
        if (groundBoxcast)
        {
            superJumping = true;

            coyoteStandbyActive = true;
        }
        else
        {
            submerging = false;
        }
    }

    protected void BufferJumpCheck()
    {
        bufferJumpBoxcast = Physics2D.BoxCast(bufferJumpTransform.position, bufferJumpBoxSize, 0, -transform.up, bufferJumpCastDistance, bufferJumpLayer);

        if (bufferJumpBoxcast)
        {
            if (Input.GetKeyDown(KeyCode.Space) && falling) 
            {
                Debug.Log("Buffered");
                bufferedJump = true;
            }
        }
    }

    protected void SubmergeOverheadCheck()
    {
        if (submerging)
            return;

        overheadOverlapBox = Physics2D.BoxCast(overheadTransform.position, overheadBoxSize, 0, transform.up, overheadCastDistance, overheadCheckLayer);
        objectAbove = overheadOverlapBox;
    }

    //============================================= OTHERS =============================================//

    protected override void Timers()
    {
        base.Timers();

        if (dashDuration.CurrentProgress == Timer.Progress.Finished)
        {
            dashCooldown.StartCooldown();
        }
        if (dashDuration.CurrentProgress == Timer.Progress.Finished)
        {
            attachedRigidbody.gravityScale = 5f;
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);
            dashDuration.ResetCooldown();
        }
            
        if (grounded && dashCooldown.CurrentProgress == Timer.Progress.Finished)
            dashCooldown.ResetCooldown();
    }

    protected void CoyoteTime()
    {
        if (jumping)
            coyoteStandbyActive = false;

        if (!grounded && coyoteStandbyActive && coyoteTime.CurrentProgress == Timer.Progress.Ready)
        {
            coyoteTime.StartCooldown();
        }

        if (grounded && coyoteTime.CurrentProgress == Timer.Progress.Finished)
        {
            coyoteTime.ResetCooldown();
        }
    }

    protected override void KnockedbackState()
    {
        base.KnockedbackState();
    }

    //============================================= BASIC MOVEMENT =============================================//
    protected override void UpdateMovementStates()
    {
        base.UpdateMovementStates();
        inputLeft = inputTracker.InputLeft;
        inputRight = inputTracker.InputRight;

        dashing = dashDuration.CurrentProgress == Timer.Progress.InProgress;
        dashingBackward = dashing && ((facingRight && inputLeft) || (!facingRight && inputRight));
        dashingForward = dashing && !dashingBackward;
        airDashingForward = dashingForward && !grounded;
        airDashingBackward = dashingBackward && !grounded;
        submergingForward = submerging && movingForward;
        submergingBackward = submerging && movingBackward;
    }

    protected override void HorizontalMovement()
    {
        if (combat.IsAttacking && grounded)
        {
            attachedRigidbody.velocity = Vector2.zero;
            return;
        }
            

        if (dashing || jumping || falling)
            return;

        if (((PlayerCombat)combat).IsDirectionLocked && ((facingLeft && inputRight) || (facingRight && inputLeft)))
        {
            attachedRigidbody.velocity = new Vector2(inputTracker.PlayerDirectionalInput.x * speedBackwards, attachedRigidbody.velocity.y);
        }
        else
        {
            attachedRigidbody.velocity = new Vector2(inputTracker.PlayerDirectionalInput.x * speedForwards, attachedRigidbody.velocity.y);
        }
    }

    protected override void VerticalMovement()
    {
        if (combat.IsAttacking)
            return;

        if (jumping)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (grounded || coyoteTime.CurrentProgress == Timer.Progress.InProgress)
            {
                if (inputTracker.InputDown)
                {
                    attachedRigidbody.velocity = new Vector2(attachedRigidbody.velocity.x, jumpPower * 1.3f);
                    superJumping = true;
                }
                else
                {
                    attachedRigidbody.velocity = new Vector2(attachedRigidbody.velocity.x, jumpPower);
                }

                bufferedJump = false;
            }
        }

        if (grounded && bufferedJump)
        {
            if (inputTracker.InputDown)
            {
                attachedRigidbody.velocity = new Vector2(attachedRigidbody.velocity.x, jumpPower * 1.3f);
                superJumping = true;
            }
            else
            {
                attachedRigidbody.velocity = new Vector2(attachedRigidbody.velocity.x, jumpPower);
            }

            coyoteStandbyActive = false;
            bufferedJump = false;
        }
    }

    //============================================= SPECIAL MOVEMENT =============================================//
    protected void Dash()
    {
        if ((!submerging && !jumping && !falling) || dashCooldown.CurrentProgress != Timer.Progress.Ready)
            return;

        if (Input.GetKey(inputTracker.DashButton) && dashDuration.CurrentProgress == Timer.Progress.Ready)
        {
            dashDuration.StartCooldown();
            attachedRigidbody.gravityScale = 0f;
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);
        }

        float alteredDashPower = dashPower;

        if (airDashingForward)
        {
            alteredDashPower = dashPower * 1.5f;
        }
        else
        {
            alteredDashPower = dashPower;
        }

        if (dashingForward)
        {
            attachedRigidbody.velocity = new Vector2(alteredDashPower * transform.localScale.x, 0);
        }
        else if (dashingBackward)
        {
            attachedRigidbody.velocity = new Vector2(alteredDashPower * -transform.localScale.x, 0);
        }

        if (dashingBackward && dashDuration.CurrentDuration < dashDuration.Duration / 1.8f)
        {
            dashDuration.EndCooldown();
            Debug.Log("Backdash ended early");
        }
    }

    protected void Submerge() //**CHANGE TO CONSUMING METER BASED** 
    {
        if (!grounded)
            return;

        if (inputTracker.InputDown) 
        {
            submerging = true;
            Debug.Log("Submerged");
        }
        else if (objectAbove || dashing || ((PlayerCombat)combat).IsSubmergeLight)
        {
            submerging = true;
            Debug.Log("Forced Submerged");
        }
        else
        {
            submerging = false;
        }
    }

    protected void Pogo()
    {
        
    }

    

    
}
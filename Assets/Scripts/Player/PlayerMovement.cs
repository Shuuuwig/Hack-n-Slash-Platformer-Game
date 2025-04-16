using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("========== Configuration ==========")]
    [Header("--- Gizmo Configuration ---")]
    [SerializeField] protected bool gizmoToggleOn = true;

    [Header("--- Run ---")]
    [SerializeField] protected float walkForwardMultiplier;
    [SerializeField] protected float walkBackwardMultiplier;
    [SerializeField] protected float runForwardMultiplier;

    [Header("--- Jump ---")]
    [SerializeField] protected float jumpPowerMultiplier;
    [SerializeField] protected float superJumpPowerMultiplier;
    [SerializeField] protected Timer coyoteTime;
    [SerializeField] protected float bufferJumpCastDistance;
    [SerializeField] protected Timer bufferJumpWindow;
    [SerializeField] protected Transform bufferJumpTransform;
    [SerializeField] protected Vector2 bufferJumpBoxSize;
    [SerializeField] protected LayerMask bufferJumpLayer;
    protected RaycastHit2D bufferJumpBoxcast;

    [Header("--- Dash ---")]
    [SerializeField] protected float dashPower;
    [SerializeField] protected Timer dashDuration;
    [SerializeField] protected Timer dashCooldown;

    [Header("--- Ground Check ---")]
    [SerializeField] protected float groundCastDistance;
    [SerializeField] protected Transform groundTransform;
    [SerializeField] protected Vector2 groundBoxSize;
    [SerializeField] protected LayerMask groundLayer;
    protected RaycastHit2D groundBoxcast;

    [Header("--- Submerge Overhead Check ---")]
    [SerializeField] protected float overheadCastDistance;
    [SerializeField] protected Transform overheadTransform;
    [SerializeField] protected Vector2 overheadBoxSize;
    [SerializeField] protected LayerMask overheadCheckLayer;
    protected RaycastHit2D overheadOverlapBox;

    [SerializeField] protected LayerMask playerLayer;
    [SerializeField] protected LayerMask enemyLayer;

    [Header("--- Hurtbox Colliders ---")]
    [SerializeField] private GameObject hurtboxIdle;
    [SerializeField] private GameObject hurtboxWalkForward;
    [SerializeField] private GameObject hurtboxWalkBackward;
    [SerializeField] private GameObject hurtboxRun;
    [SerializeField] private GameObject hurtboxJump;
    [SerializeField] private GameObject hurtboxFall;
    [SerializeField] private GameObject hurtboxDash;
    [SerializeField] private GameObject hurtboxSubmerge;

    private GameObject currentHurtbox;

    protected float finalizedSpeed;
    protected float finalizedJump;

    protected bool inputLeft;
    protected bool inputRight;

    protected bool moving;
    protected bool walkingForward;
    protected bool walkingBackward;
    protected bool runningForward;
    protected bool jumping;
    protected bool jumpingForward;
    protected bool jumpingBackward;
    protected bool superJumping;
    protected bool superJumpingForward;
    protected bool superJumpingBackward;
    protected bool falling;
    protected bool fallingForward;
    protected bool fallingBackward;
    protected bool submerging;
    protected bool submergingForward;
    protected bool submergingBackward;
    protected bool dashing;
    protected bool submergingDashingForward;
    protected bool submergingDashingBackward;
    protected bool airDashingForward;
    protected bool airDashingBackward;

    protected bool facingLeft;
    protected bool facingRight;
    protected bool knockedback;
    protected bool objectAbove;
    protected bool coyoteStandbyActive;
    protected bool bufferedJump;
    protected bool grounded;

    protected Vector2 knockedbackDirection;
   
    protected Rigidbody2D attachedRigidbody;
    protected PlayerInputTracker inputTracker;
    protected PlayerAnimationHandler animationHandler;
    protected PlayerStatus status;
    protected PlayerStats stats;
    protected PlayerCombat combat;

    public float FinalizedSpeed { get { return finalizedSpeed; } }

    public bool Moving { get { return moving; } }
    public bool WalkingForward { get { return walkingForward; } }
    public bool WalkingBackward { get { return walkingBackward; } }
    public bool RunningForward { get { return runningForward; } }
    public bool Jumping { get { return jumping; } }
    public bool JumpingForward { get { return jumpingForward; } }
    public bool JumpingBackward { get { return jumpingBackward; } }
    public bool SuperJumping { get { return superJumping; } }
    public bool SuperJumpingForward { get { return superJumpingForward; } }
    public bool SuperJumpingBackward { get { return superJumpingBackward; } }
    public bool Falling { get { return falling; } }
    public bool FallingForward { get { return fallingForward; } }
    public bool FallingBackward { get { return fallingBackward; } }
    public bool Submerging { get { return submerging; } }
    public bool SubmergingForward { get { return submergingForward; } }
    public bool SubmergingBackward { get { return submergingBackward; } }
    public bool Dashing { get { return dashing; } }
    public bool SubmergingDashingForward { get { return submergingDashingForward; } }
    public bool SubmergingDashingBackward { get { return submergingDashingBackward; } }
    public bool AirDashingForward { get { return airDashingForward; } }
    public bool AirDashingBackward { get { return airDashingBackward; } }

    public bool Knockedback { get { return knockedback; } }
    public bool Grounded { get { return grounded; } }
    public Rigidbody2D AttachedRigidBody { get { return attachedRigidbody; } }


    protected void OnDrawGizmos()
    {
        if (!gizmoToggleOn)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(groundTransform.position + -transform.up * (groundCastDistance / 2), groundBoxSize * 2);
        Gizmos.DrawLine(groundTransform.position, groundTransform.position + -transform.up * groundCastDistance);
        Gizmos.DrawWireCube(overheadTransform.position + transform.up * (overheadCastDistance / 2), overheadBoxSize * 2);
        Gizmos.DrawLine(overheadTransform.position, overheadTransform.position + transform.up * overheadCastDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(bufferJumpTransform.position + -transform.up * (bufferJumpCastDistance / 2), bufferJumpBoxSize * 2);
        Gizmos.DrawLine(bufferJumpTransform.position, bufferJumpTransform.position + -transform.up * bufferJumpCastDistance);
    }

    protected void Start()
    {
        attachedRigidbody = GetComponent<Rigidbody2D>();
        animationHandler = GetComponent<PlayerAnimationHandler>();
        inputTracker = GetComponent<PlayerInputTracker>();
        combat = GetComponent<PlayerCombat>();
        status = GetComponent<PlayerStatus>();
        stats = GetComponent<PlayerStats>();
    }

    protected void Update()
    {
        UpdateMovementConditions();
        UpdateHurtbox();
        GroundCheck();
        BufferJumpCheck();
        SubmergeOverheadCheck();

        CoyoteTime();
        Timers();

        HorizontalMovement();
        VerticalMovement();
        Dash();
        Submerge();
    }


    protected void GroundCheck()
    {
        groundBoxcast = Physics2D.BoxCast(groundTransform.position, groundBoxSize, 0, -transform.up, groundCastDistance, groundLayer);

        if (groundBoxcast)
        {
            grounded = true;
            jumping = false;
            superJumping = true;
            falling = false;
            coyoteStandbyActive = true;
        }
        else
        {
            grounded = false;
            submerging = false;

            if (falling && attachedRigidbody.velocity.y < -15f)
            {
                attachedRigidbody.velocity = new Vector2(attachedRigidbody.velocity.x, -15f);
            }
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

    protected void Timers()
    {
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

    //============================================= BASIC MOVEMENT =============================================//
    protected void UpdateMovementConditions()
    {
        inputLeft = inputTracker.InputLeft;
        inputRight = inputTracker.InputRight;
        facingLeft = animationHandler.FacingLeft;
        facingRight = animationHandler.FacingRight;

        moving = Mathf.Abs(attachedRigidbody.velocity.x) > 0.01f;
        walkingForward = animationHandler.MovingForward;
        walkingBackward = animationHandler.MovingBackward;
        jumping = attachedRigidbody.velocity.y > 0.01f;
        jumpingForward = jumping && walkingForward;
        jumpingBackward = jumping && walkingBackward;
        falling = attachedRigidbody.velocity.y < -0.01f;
        fallingForward = falling && walkingForward;
        fallingBackward = falling && walkingBackward;
        dashing = dashDuration.CurrentProgress == Timer.Progress.InProgress;
        submergingDashingBackward = dashing && ((facingRight && inputLeft) || (!facingRight && inputRight));
        submergingDashingForward = dashing && !submergingDashingBackward;
        airDashingForward = submergingDashingForward && !grounded;
        airDashingBackward = submergingDashingBackward && !grounded;
        submergingForward = submerging && walkingForward;
        submergingBackward = submerging && walkingBackward;
    }

    protected void UpdateHurtbox()
    {
        GameObject nextHurtbox = hurtboxIdle;

        if (dashing)
            nextHurtbox = hurtboxDash;
        else if (submerging)
            nextHurtbox = hurtboxSubmerge;
        //else if (jumping)
        //    nextHurtbox = hurtboxJump;
        //else if (falling)
        //    nextHurtbox = hurtboxFall;
        else if (runningForward)
            nextHurtbox = hurtboxRun;
        else if (walkingForward)
            nextHurtbox = hurtboxWalkForward;
        else if (walkingBackward)
            nextHurtbox = hurtboxWalkBackward;
        else
            nextHurtbox = hurtboxIdle;

        Debug.Log(nextHurtbox.name);
        // Only switch if different
        if (currentHurtbox != nextHurtbox)
        {
            if (currentHurtbox != null)
                currentHurtbox.SetActive(false);

            if (nextHurtbox != null)
                nextHurtbox.SetActive(true);

            currentHurtbox = nextHurtbox;
        }
    }
    protected void HorizontalMovement()
    {
        if ((combat.IsAttacking && grounded) || combat.IsParry)
        {
            attachedRigidbody.velocity = Vector2.zero;
            return;
        }
            
        if (status.IsKnockedback)
        {
            knockedbackDirection = new Vector2(transform.position.x - status.CollisionPoint.x, 0);
            attachedRigidbody.velocity = new Vector2(status.KnockedbackForce * Mathf.Sign(knockedbackDirection.x), attachedRigidbody.velocity.y);
            //Debug.Log($"Knockback Force: {status.KnockedbackForce}, Knockback Direction: {knockedbackDirection}, Rigibody Velocity: {attachedRigidbody.velocity}");
        }

        if (dashing || jumping || falling || status.IsKnockedback)
            return;

        if (combat.IsDirectionLocked && ((facingLeft && inputRight) || (facingRight && inputLeft)))
        {
            finalizedSpeed = stats.BaseSpeed * walkBackwardMultiplier;
            attachedRigidbody.velocity = new Vector2(inputTracker.PlayerDirectionalInput.x * finalizedSpeed, attachedRigidbody.velocity.y);
        }
        else
        {
            if (combat.IsShowdown)
            {
                finalizedSpeed = stats.BaseSpeed * walkForwardMultiplier;
                attachedRigidbody.velocity = new Vector2(inputTracker.PlayerDirectionalInput.x * finalizedSpeed, attachedRigidbody.velocity.y);
            }
            else
            {
                finalizedSpeed = stats.BaseSpeed * runForwardMultiplier;
                attachedRigidbody.velocity = new Vector2(inputTracker.PlayerDirectionalInput.x * finalizedSpeed, attachedRigidbody.velocity.y);
            }
                
        }
    }

    protected void VerticalMovement()
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
                    finalizedJump = stats.BaseJumpPower * superJumpPowerMultiplier;
                    attachedRigidbody.velocity = new Vector2(attachedRigidbody.velocity.x, finalizedJump);
                    superJumping = true;
                }
                else
                {
                    finalizedJump = stats.BaseJumpPower * jumpPowerMultiplier;
                    attachedRigidbody.velocity = new Vector2(attachedRigidbody.velocity.x, finalizedJump);
                }

                bufferedJump = false;
            }
        }

        if (grounded && bufferedJump)
        {
            if (inputTracker.InputDown)
            {
                finalizedJump = stats.BaseJumpPower * superJumpPowerMultiplier;
                attachedRigidbody.velocity = new Vector2(attachedRigidbody.velocity.x, finalizedJump);
                superJumping = true;
            }
            else
            {
                finalizedJump = stats.BaseJumpPower * jumpPowerMultiplier;
                attachedRigidbody.velocity = new Vector2(attachedRigidbody.velocity.x, finalizedJump);
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

        if (submergingDashingForward)
        {
            attachedRigidbody.velocity = new Vector2(alteredDashPower * transform.localScale.x, 0);
        }
        else if (submergingDashingBackward)
        {
            attachedRigidbody.velocity = new Vector2(alteredDashPower * -transform.localScale.x, 0);
        }

        if (submergingDashingBackward && dashDuration.CurrentDuration < dashDuration.Duration / 1.8f)
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
        else if (objectAbove || dashing || combat.IsSubmergeLight)
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
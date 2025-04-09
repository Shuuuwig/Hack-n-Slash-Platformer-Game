using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Movement : MonoBehaviour
{
    [Header("========== Base Configuration ==========")]

    // Gizmo Toggle
    [Header("--- Gizmo Configuration ---")]
    [SerializeField] protected bool gizmoToggleOn = true;

    // Movement Configuration
    [Header("--- Run ---")]
    [SerializeField] protected float speedForwards;
    [SerializeField] protected float speedBackwards;

    [Header("--- Jump ---")]
    [SerializeField] protected float jumpPower;
    
    // Collision Check
    [Header("--- Ground Check ---")]
    [SerializeField] protected float groundCastDistance;
    [SerializeField] protected Transform groundTransform;
    [SerializeField] protected Vector2 groundBoxSize;
    [SerializeField] protected LayerMask groundLayer;
    protected RaycastHit2D groundBoxcast;

    // Knockback
    [Header("--- Knockback Effect ---")]
    [SerializeField] protected float knockedbackForce;
    [SerializeField] protected Timer knockedbackTimer;
    protected Vector2 knockedbackDirection;
    protected Vector2 collisionPoint;

    // Movement States
    protected bool moving;
    protected bool movingForward;
    protected bool movingBackward;
    protected bool jumping;
    protected bool jumpingForward;
    protected bool jumpingBackward;
    protected bool falling;
    protected bool fallingForward;
    protected bool fallingBackward;

    protected bool facingLeft;
    protected bool facingRight;
    protected bool knockedback;

    // Collision States
    protected bool grounded;

    protected Collider2D hurtbox;
    protected Rigidbody2D attachedRigidbody;

    public bool Grounded {  get { return grounded; } }
    public Rigidbody2D AttachedRigidBody { get { return attachedRigidbody; } }
    //private Status status;
    //private Stats stats;

    public bool Moving {  get { return moving; } }
    public bool MovingForward { get { return movingForward; } }
    public bool MovingBackward { get { return movingBackward; } }
    public bool Jumping { get { return jumping; } }
    public bool JumpingForward { get { return jumpingForward; } }
    public bool JumpingBackward { get { return jumpingBackward; } }
    public bool Falling { get { return falling; } }
    public bool FallingForward { get { return fallingForward; } }
    public bool FallingBackward { get { return fallingBackward; } }
    public bool FacingRight { get { return facingRight; } }
    public bool Knockedback { get { return knockedback; } }

    protected AnimationHandler animationHandler;
    protected Status status;
    protected Combat combat;

    //============================================= GIZMO =============================================//
    protected virtual void OnDrawGizmos()
    {
        if (!gizmoToggleOn)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(groundTransform.position + -transform.up * (groundCastDistance / 2), groundBoxSize * 2);
        Gizmos.DrawLine(groundTransform.position, groundTransform.position + -transform.up * groundCastDistance);
    }

    //============================================= LIFE CYCLE =============================================//
    protected virtual void Start()
    {
        attachedRigidbody = GetComponent<Rigidbody2D>();
        hurtbox = GetComponent<Collider2D>();
    }

    protected virtual void Update()
    {
        UpdateMovementStates();
        GroundCheck();

        Timers();

        HorizontalMovement();
        VerticalMovement();
    }

    //============================================= OTHERS =============================================//
    protected virtual void Timers()
    {
        if (knockedbackTimer.CurrentProgress == Timer.Progress.InProgress)
        {
            attachedRigidbody.velocity = new Vector2(knockedbackForce * Mathf.Sign(knockedbackDirection.x), 0);
        }
        else if (knockedbackTimer.CurrentProgress == Timer.Progress.Finished)
        {
            status.IsKnockedback = false;
            knockedbackTimer.ResetCooldown();
        }
    }

    protected virtual void KnockedbackState()
    {
        if (knockedbackTimer.CurrentProgress == Timer.Progress.Ready)
        {
            status.IsKnockedback = true;
            knockedbackTimer.StartCooldown();
            knockedbackDirection = new Vector2(transform.position.x - collisionPoint.x, 0);
        }

    }

    //============================================= COLLISION CHECK =============================================//
    protected virtual void GroundCheck()
    {
        groundBoxcast = Physics2D.BoxCast(groundTransform.position, groundBoxSize, 0, -transform.up, groundCastDistance, groundLayer);

        if (groundBoxcast)
        {
            grounded = true;
            jumping = false;
            falling = false;
        }
        else
        {
            grounded = false;
        }
    }

    //============================================= BASIC MOVEMENT =============================================//
    protected virtual void UpdateMovementStates()
    {
        facingLeft = animationHandler.FacingLeft;
        facingRight = animationHandler.FacingRight;
        moving = Mathf.Abs(attachedRigidbody.velocity.x) > 0.01f;
        movingForward = animationHandler.MovingForward;
        movingBackward = animationHandler.MovingBackward;
        jumping = attachedRigidbody.velocity.y > 0.01f;
        jumpingForward = jumping && movingForward;
        jumpingBackward = jumping && movingBackward;
        falling = attachedRigidbody.velocity.y < -0.01f;
        fallingForward = falling && movingForward;
        fallingBackward = falling && movingBackward;    
    }

    protected abstract void HorizontalMovement();
    protected abstract void VerticalMovement();

   
}
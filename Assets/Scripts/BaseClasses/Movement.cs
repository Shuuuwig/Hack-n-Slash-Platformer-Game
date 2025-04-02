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
    [SerializeField] protected Cooldown knockedbackTimer;
    protected Vector2 knockedbackDirection;
    protected Vector2 collisionPoint;

    // Movement States
    protected bool isIdle;
    protected bool isMoving;
    protected bool isMovingForward;
    protected bool isMovingBackward;
    protected bool isJumping;
    protected bool isFalling;
    protected bool isFacingRight;
    protected bool isKnockedback;

    // Collision States
    protected bool isGrounded;

    protected Rigidbody2D attachedRigidbody;
    public Rigidbody2D AttachedRigidBody { get { return attachedRigidbody; } }
    //private Status status;
    //private Stats stats;

    public bool IsMoving {  get { return isMoving; } }
    public bool IsIdle { get { return isIdle; } }

    protected AnimationHandler animationHandler;

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
        if (attachedRigidbody == null)
        {
            attachedRigidbody = GetComponent<Rigidbody2D>();
        }
        if (attachedRigidbody == null)
        {
            Debug.LogError("Component Attached Rigidbody not found", this);
            return;
        }

        if (animationHandler == null)
        {
            animationHandler = GetComponent<AnimationHandler>();
        }
        if (animationHandler == null)
        {
            Debug.Log("Component Animation Handler not found");
            return;
        }
    }

    protected virtual void Update()
    {
        UpdateMovementStates();
        GroundCheck();
    }

    //============================================= COLLISION CHECK =============================================//
    protected virtual void GroundCheck()
    {
        groundBoxcast = Physics2D.BoxCast(groundTransform.position, groundBoxSize, 0, -transform.up, groundCastDistance, groundLayer);

        if (groundBoxcast)
        {
            isGrounded = true;
            isJumping = false;
            isFalling = false;
        }
        else
        {
            isGrounded = false;
        }
    }

    //============================================= BASIC MOVEMENT =============================================//
    protected virtual void UpdateMovementStates()
    {
        isFacingRight = animationHandler.IsFacingRight;
        isMoving = Mathf.Abs(attachedRigidbody.velocity.x) > 0.01f;
        isMovingForward = animationHandler.IsMovingForward;
        isMovingBackward = isMoving && !isMovingForward;
        isJumping = attachedRigidbody.velocity.y > 0.01f;
        isFalling = attachedRigidbody.velocity.y < -0.01f;
        isIdle = !isMoving && !isJumping && !isFalling;
    }

    protected abstract void HorizontalMovement();
    protected abstract void VerticalMovement();

    //============================================= DISPLACEMENT EFFECTS =============================================//
    protected virtual void Knockedback()
    {
        if (knockedbackTimer.CurrentProgress == Cooldown.Progress.Ready)
        {
            //status.IsKnockedback = true;
            knockedbackTimer.StartCooldown();
            knockedbackDirection = new Vector2(transform.position.x - collisionPoint.x, 0);
        }
        else if (knockedbackTimer.CurrentProgress == Cooldown.Progress.InProgress)
        {
            attachedRigidbody.velocity = new Vector2(knockedbackForce * Mathf.Sign(knockedbackDirection.x), 0);
        }
        else if (knockedbackTimer.CurrentProgress == Cooldown.Progress.Finished)
        {
            //status.IsKnockedback = false;
            knockedbackTimer.ResetCooldown();
        }
    }
}
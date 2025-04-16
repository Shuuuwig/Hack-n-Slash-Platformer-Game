using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyMovement : MonoBehaviour
{
    [Header("========== Base Configuration ==========")]
    [Header("--- Gizmo Configuration ---")]
    [SerializeField] protected bool gizmoToggleOn = true;

    [Header("--- Run ---")]
    [SerializeField] protected float speedForwardsMultiplier;
    [SerializeField] protected float speedBackwardsMultiplier;

    [Header("--- Jump ---")]
    [SerializeField] protected float jumpPower;
    
    [Header("--- Ground Check ---")]
    [SerializeField] protected float groundCastDistance;
    [SerializeField] protected Transform groundTransform;
    [SerializeField] protected Vector2 groundBoxSize;
    [SerializeField] protected LayerMask groundLayer;
    protected RaycastHit2D groundBoxcast;

    protected int selectedBehaviour;
    protected float finalizedSpeed;

    protected bool moving;
    protected bool walkingForward;
    protected bool walkingBackward;
    protected bool runningForward;
    protected bool jumping;
    protected bool jumpingForward;
    protected bool jumpingBackward;
    protected bool falling;
    protected bool fallingForward;
    protected bool fallingBackward;

    protected bool facingLeft;
    protected bool facingRight;
    protected bool knockedback;
    protected bool behaviourDetermined;
    protected bool playerTooClose;

    protected bool grounded;

    protected Collider2D hurtbox;
    protected Rigidbody2D attachedRigidbody;
    protected Transform playerTarget;

    protected EnemyAnimationHandler animationHandler;
    protected EnemyStatus status;
    protected EnemyStats stats;
    protected EnemyCombat combat;

    public float FinalizedSpeed { get { return finalizedSpeed; } }

    public bool Moving {  get { return moving; } }
    public bool WalkingForward { get { return walkingForward; } }
    public bool WalkingBackward { get { return walkingBackward; } }
    public bool RunningForward {  get { return runningForward; } }
    public bool Jumping { get { return jumping; } }
    public bool JumpingForward { get { return jumpingForward; } }
    public bool JumpingBackward { get { return jumpingBackward; } }
    public bool Falling { get { return falling; } }
    public bool FallingForward { get { return fallingForward; } }
    public bool FallingBackward { get { return fallingBackward; } }
    public bool FacingRight { get { return facingRight; } }
    public bool Knockedback { get { return knockedback; } }

    public bool Grounded { get { return grounded; } }
    public Rigidbody2D AttachedRigidBody { get { return attachedRigidbody; } }


    protected virtual void OnDrawGizmos()
    {
        if (!gizmoToggleOn)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(groundTransform.position + -transform.up * (groundCastDistance / 2), groundBoxSize * 2);
        Gizmos.DrawLine(groundTransform.position, groundTransform.position + -transform.up * groundCastDistance);
    }

    protected virtual void Start()
    {
        attachedRigidbody = GetComponent<Rigidbody2D>();
        hurtbox = GetComponent<Collider2D>();
    }

    protected virtual void Update()
    {
        UpdateMovementConditions();
        GroundCheck();

        Timers();

        HorizontalMovement();
        VerticalMovement();
    }

    protected virtual void Timers()
    {

    }

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

    protected virtual void UpdateMovementConditions()
    {
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
    }

    protected virtual void BehaviourManager()
    {

    }

    protected abstract void HorizontalMovement();
    protected abstract void VerticalMovement();

   
}
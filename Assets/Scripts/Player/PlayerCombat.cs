using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("========== Configuration ==========")]
    [Header("--- Gizmo Configuration ---")]
    [SerializeField] protected bool gizmoToggleOn = true;

    [Header("--- Attack Duration ---")]
    [SerializeField] protected float neutralLightDuration;
    [SerializeField] protected float forwardLightDuration;
    [SerializeField] protected float submergedLightDuration;
    [SerializeField] protected float dashLightDuration;
    [SerializeField] protected float airLightDuration;
    [SerializeField] protected float airLightLowDuration;
    [SerializeField] protected float airLightHighDuration;
    [SerializeField] protected float sludgeBombDuration;

    [Header("--- Attack Damage Modifier ---")]
    [SerializeField] protected float neutralLightMultiplier;
    [SerializeField] protected float forwardLightMultiplier;
    [SerializeField] protected float submergedLightMultiplier;
    [SerializeField] protected float dashLightMultiplier;
    [SerializeField] protected float airLightMultiplier;
    [SerializeField] protected float airLightLowMultiplier;
    [SerializeField] protected float airLightHighMultiplier;
    [SerializeField] protected float sludgeBombMultiplier;

    [Header("--- Attack Cooldown ---")]
    [SerializeField] protected Timer attackCooldown;
    [SerializeField] protected Timer comboActiveTime;
    [SerializeField] protected Timer attackTime;

    [Header("--- Attack Colliders ---")]
    [SerializeField] protected Collider2D neutralLightCollider;
    [SerializeField] protected Collider2D forwardLightCollider;
    [SerializeField] protected Collider2D submergedLightCollider;
    [SerializeField] protected Collider2D dashLightCollider;
    [SerializeField] protected Collider2D airLightCollider;
    [SerializeField] protected Collider2D airLightLowCollider;
    [SerializeField] protected Collider2D airLightHighCollider;

    [Header("--- Recovery Time ---")]
    [SerializeField] protected Timer recoveryTime;

    [Header("--- Parry ---")]
    [SerializeField] protected float parryDuration;
    [SerializeField] protected Timer parryActiveTime;
    [SerializeField] protected Timer parrySuccessTime;
    [SerializeField] protected Transform parryTransform;
    [SerializeField] protected Vector2 parryBoxSize;
    [SerializeField] protected LayerMask parryableLayer;
    protected Collider2D parryCollider;

    [Header("--- Hitstop ---")]
    [SerializeField] protected Timer hitstopDuration;

    protected float finalizedDamage;
    protected float maxBasicCombo;
    protected float currentCombo;

    protected bool cancellable;
    protected bool hitTarget;
    protected bool parriedAttack;

    protected bool parry;
    protected bool neutralLight;
    protected bool forwardLight;
    protected bool submergeLight;
    protected bool dashLight;
    protected bool airLight;
    protected bool airLightHigh;
    protected bool airLightLow;
    protected bool sludgeBomb;

    protected bool isShowdown;
    protected bool isAttacking;
    protected bool isDirectionLocked;
    protected bool isParry;
    protected bool isNeutralLight;
    protected bool isForwardLight;
    protected bool isSubmergeLight;
    protected bool isDashLight;
    protected bool isAirLight;
    protected bool isAirLightHigh;
    protected bool isAirLightLow;
    protected bool isSludgeBomb;

    protected bool canParry = true;
    protected bool canNeutralLight = true;
    protected bool canForwardLight = true;
    protected bool canSubmergeLight = true;
    protected bool canDashLight = true;
    protected bool canAirLight = true;
    protected bool canAirLightHigh = true;
    protected bool canAirLightLow = true;
    protected bool canSludgeBomb = true;

    protected PlayerMovement movement;
    protected PlayerStats stats;
    protected PlayerStatus status;

    public float FinalizedDamage { get { return finalizedDamage; } }
    public bool Parry { get { return parry; } }
    public bool NeutralLight { get { return neutralLight; } }
    public bool ForwardLight { get { return forwardLight; } }
    public bool SubmergeLight { get { return submergeLight; } }
    public bool DashLight { get { return dashLight; } }
    public bool AirLight { get { return airLight; } }
    public bool AirLightHigh { get { return airLightHigh; } }
    public bool AirLightLow { get { return airLightLow; } }
    public bool SludgeBomb { get { return sludgeBomb; } }

    public bool IsShowdown { get { return isShowdown; } }
    public bool IsAttacking { get { return isAttacking; } }
    public bool IsDirectionLocked 
    { 
        get { return isDirectionLocked; } 
        set {  isDirectionLocked = value; }
    }
    public bool IsParry { get { return isParry; } }
    public bool IsNeutralLight { get { return isNeutralLight; } }
    public bool IsForwardLight { get { return isForwardLight; } }
    public bool IsSubmergeLight { get { return isSubmergeLight; } }
    public bool IsDashLight { get { return isDashLight; } }
    public bool IsAirLight { get { return isAirLight; } }
    public bool IsAirLightHigh { get { return isAirLightHigh; } }
    public bool IsAirLightLow { get { return isAirLightLow; } }
    public bool IsSludgeBomb { get { return isSludgeBomb; } }

    public Collider2D NeutralLightCollider
    {
        get { return neutralLightCollider; }
        set { neutralLightCollider = value; }
    }

    public Collider2D ForwardLightCollider
    {
        get { return forwardLightCollider; }
        set { forwardLightCollider = value; }
    }

    public Collider2D SubmergedLightCollider
    {
        get { return submergedLightCollider; }
        set { submergedLightCollider = value; }
    }

    public Collider2D DashLightCollider
    {
        get { return dashLightCollider; }
        set { dashLightCollider = value; }
    }

    public Collider2D AirLightCollider
    {
        get { return airLightCollider; }
        set { airLightCollider = value; }
    }


    public Collider2D AirLightLowCollider
    {
        get { return airLightLowCollider; }
        set { airLightLowCollider = value; }
    }

    public Collider2D AirLightHighCollider
    {
        get { return airLightHighCollider; }
        set { airLightHighCollider = value; }
    }

    protected PlayerInputTracker inputTracker;
    protected PlayerAnimationHandler animationHandler;

    private void OnDrawGizmos()
    {
        if (gizmoToggleOn != true)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(parryTransform.position, parryBoxSize);

    }

    protected void Start()
    {
        stats = GetComponent<PlayerStats>();
        status = GetComponent<PlayerStatus>();
        movement = GetComponent<PlayerMovement>();
        inputTracker = GetComponent<PlayerInputTracker>();
        animationHandler = GetComponent<PlayerAnimationHandler>();
    }
    protected void Update()
    {
        DetermineCombatState();
        DirectionLock();

        Timers();

        Attack();
        ParryState();
    }

    protected void DirectionLock()
    {
        if (!status.IsKnockedback && Input.GetKeyDown(inputTracker.CameraLock))
        {
            isDirectionLocked = !isDirectionLocked;
        }

        if (status.IsKnockedback)
        {
            isDirectionLocked = true;
        }

        if (status.IsKnockedback)
        {
            isDirectionLocked = true;
        }
        else if (status.KnockbackEnd && Mathf.Abs(movement.AttachedRigidBody.velocity.x) < 0.01f)
        {
            isDirectionLocked = false;
        }
    }

    protected void HitStop()
    {
        if (hitstopDuration.CurrentProgress is Timer.Progress.Ready)
        {
            hitstopDuration.StartCooldownRealtime();
            Time.timeScale = 0;
            Debug.Log("hitstopped");
        }
        if (hitstopDuration.CurrentProgress is Timer.Progress.Finished)
        {
            hitstopDuration.ResetCooldown();
            Time.timeScale = 1;
            Debug.Log("hitstop ended");
        }
    }

    protected void Timers()
    {
        if (attackTime.CurrentProgress == Timer.Progress.Finished)
        {
            attackTime.ResetCooldown();
            recoveryTime.StartCooldown();
        }

        if (recoveryTime.CurrentProgress == Timer.Progress.Finished)
        {
            canForwardLight = true;
            canNeutralLight = true;
            canSubmergeLight = true;
            canDashLight = true;
            canAirLight = true;
            canAirLightHigh = true;
            canAirLightLow = true;

            canSludgeBomb = true;

            recoveryTime.ResetCooldown();
        }

        if (comboActiveTime.CurrentProgress == Timer.Progress.Finished)
        {
            comboActiveTime.ResetCooldown();
        }
    }

    protected void DetermineCombatState()
    {
        isAttacking = attackTime.CurrentProgress == Timer.Progress.InProgress;
        parry = parryActiveTime.CurrentProgress == Timer.Progress.InProgress;
        parriedAttack = parryCollider;

        if (attackTime.CurrentProgress == Timer.Progress.InProgress)
            return;

        neutralLight = movement.Grounded && !inputTracker.InputLeft && !inputTracker.InputRight;
        forwardLight = movement.Grounded && ((inputTracker.InputLeft && animationHandler.FacingLeft) || (inputTracker.InputRight && animationHandler.FacingRight));
        submergeLight = movement.Submerging && !inputTracker.InputLeft && !inputTracker.InputRight;
        dashLight = movement.Dashing && movement.Grounded;
        airLightHigh = !movement.Grounded && inputTracker.InputUp;
        airLightLow = !movement.Grounded && inputTracker.InputDown;
        airLight = !movement.Grounded && !airLightHigh && !airLightLow;
        sludgeBomb = movement.Grounded && inputTracker.QuarterCircleForwardMotion;

        if (Input.GetKeyDown(inputTracker.LightButton))
        {
            isNeutralLight = neutralLight;
            isForwardLight = forwardLight;
            isSubmergeLight = submergeLight;
            isDashLight = dashLight;
            isAirLight = airLight;
            isAirLightHigh = airLightHigh;
            isAirLightLow = airLightLow;
            isSludgeBomb = sludgeBomb;
        }
        else
        {
            isNeutralLight = false;
            isForwardLight = false;
            isSubmergeLight = false;
            isDashLight = false;
            isAirLight = false;
            isAirLightHigh = false;
            isAirLightLow = false;
            isSludgeBomb = false;
        }
    }

    protected void Attack()
    {
        if (recoveryTime.CurrentProgress != Timer.Progress.Ready)
            return;
        if (isAttacking)
            return;
        //&& comboActiveTime.CurrentProgress != Timer.Progress.InProgress
        if (Input.GetKeyDown(inputTracker.LightButton))
        {
            DetermineCombatState();

            if (sludgeBomb && canSludgeBomb)
            {
                Debug.Log("SludgeBomb");
                attackTime.Duration = sludgeBombDuration;
                attackTime.StartCooldown();
                canSludgeBomb = false;
                
            }
            else if (neutralLight && canNeutralLight)
            {
                Debug.Log("NeutralBasic");
                attackTime.Duration = neutralLightDuration;
                attackTime.StartCooldown();
                canNeutralLight = false;
                finalizedDamage = stats.LightDamage * neutralLightMultiplier;
            }
            else if (forwardLight && canForwardLight)
            {
                Debug.Log("ForwardBasic");
                attackTime.Duration = forwardLightDuration;
                attackTime.StartCooldown();
                canForwardLight = false;
                finalizedDamage = stats.LightDamage * forwardLightMultiplier;
            }
            else if (submergeLight && canSubmergeLight)
            {
                Debug.Log("SubmergeBasic");
                attackTime.Duration = submergedLightDuration;
                attackTime.StartCooldown();
                canSubmergeLight = false;
                finalizedDamage = stats.LightDamage * submergedLightMultiplier;
            }
            else if (dashLight && canDashLight)
            {
                Debug.Log("DashLight");
                attackTime.Duration = dashLightDuration;
                attackTime.StartCooldown();
                canDashLight = false;
                finalizedDamage = stats.LightDamage * dashLightMultiplier;
            }
            else if (airLight && canAirLight)
            {
                Debug.Log("AirLight");
                attackTime.Duration = airLightDuration;
                attackTime.StartCooldown();
                canAirLight = false;
                finalizedDamage = stats.LightDamage * airLightMultiplier;
            }
            else if (airLightHigh && canAirLightHigh)
            {
                Debug.Log("AirLightHigh");
                attackTime.Duration = airLightHighDuration;
                attackTime.StartCooldown();
                canAirLightHigh = false;
                finalizedDamage = stats.LightDamage * airLightHighMultiplier;
            }
            else if (airLightLow && canAirLightLow)
            {
                Debug.Log("AirLightLow");
                attackTime.Duration = airLightLowDuration;
                attackTime.StartCooldown();
                canAirLightLow = false;
                finalizedDamage = stats.LightDamage * airLightLowMultiplier;
            }
        }
    }

    protected void CheckCombo()
    {
        
    }

    protected void ParryState()
    {
        if (parryActiveTime.CurrentProgress != Timer.Progress.Ready)
            return;

        if (Input.GetKeyDown(inputTracker.ParryButton))
        {
            if (parryActiveTime.CurrentProgress == Timer.Progress.Ready)
            {
                parryActiveTime.StartCooldown();
            }

            if (parriedAttack)
            {
                parryActiveTime.ResetCooldown();
                parrySuccessTime.StartCooldown();
            }

            if (parrySuccessTime.CurrentProgress == Timer.Progress.Finished)
            {
                parrySuccessTime.ResetCooldown();
            }

            if (parryActiveTime.CurrentProgress == Timer.Progress.Finished)
            {
                parryActiveTime.ResetCooldown();
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCombat : Combat
{
    [Header("========== Additional Configuration ==========")]
    [Header("--- Attack Duration ---")]
    [SerializeField] protected float neutralLightDuration;
    [SerializeField] protected float forwardLightDuration;
    [SerializeField] protected float submergedLightDuration;
    [SerializeField] protected float dashLightDuration;
    [SerializeField] protected float airLightDuration;
    [SerializeField] protected float airLightLowDuration;
    [SerializeField] protected float airLightHighDuration;
    [SerializeField] protected float sludgeBombDuration;

    //Bools
    protected bool neutralLight;
    protected bool forwardLight;
    protected bool submergeLight;
    protected bool dashLight;
    protected bool airLight;
    protected bool airLightHigh;
    protected bool airLightLow;
    protected bool sludgeBomb;

    protected bool isNeutralLight;
    protected bool isForwardLight;
    protected bool isSubmergeLight;
    protected bool isDashLight;
    protected bool isAirLight;
    protected bool isAirLightHigh;
    protected bool isAirLightLow;
    protected bool isSludgeBomb;

    protected bool canNeutralLight = true;
    protected bool canForwardLight = true;
    protected bool canSubmergeLight = true;
    protected bool canDashLight = true;
    protected bool canAirLight = true;
    protected bool canAirLightHigh = true;
    protected bool canAirLightLow = true;
    protected bool canSludgeBomb = true;

    public bool NeutralLight { get { return neutralLight; } }
    public bool ForwardLight { get { return forwardLight; } }
    public bool SubmergeLight { get { return submergeLight; } }
    public bool DashLight { get { return dashLight; } }
    public bool AirLight { get { return airLight; } }
    public bool AirLightHigh { get { return airLightHigh; } }
    public bool AirLightLow { get { return airLightLow; } }
    public bool SludgeBomb { get { return sludgeBomb; } }

    public bool IsNeutralLight { get { return isNeutralLight; } }
    public bool IsForwardLight { get { return isForwardLight; } }
    public bool IsSubmergeLight { get { return isSubmergeLight; } }
    public bool IsDashLight { get { return isDashLight; } }
    public bool IsAirLight { get { return isAirLight; } }
    public bool IsAirLightHigh { get { return isAirLightHigh; } }
    public bool IsAirLightLow { get { return isAirLightLow; } }
    public bool IsSludgeBomb { get { return isSludgeBomb; } }

    protected PlayerInputTracker inputTracker;
    protected PlayerAnimationHandler animationHandler;

    protected override void Start()
    {
        if (stats == null)
        {
            stats = GetComponent<PlayerStats>();
        }

        if (movement == null)
        {
            movement = GetComponent<PlayerMovement>();
        }

        if (inputTracker == null)
        {
            inputTracker = GetComponent<PlayerInputTracker>();
        }

        if (animationHandler == null)
        {
            animationHandler = GetComponent<PlayerAnimationHandler>();
        }

        
    }
    protected override void Update()
    {
        base.Update();
    }

    protected override void DirectionLock()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            isDirectionLocked = !isDirectionLocked;
        }
    }

    protected override void Timers()
    {
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
        }

        base.Timers();
    }

    protected override void DetermineCombatState()
    {
        base.DetermineCombatState();

        if (attackTime.CurrentProgress == Timer.Progress.InProgress)
            return;

        neutralLight = movement.Grounded && !inputTracker.InputLeft && !inputTracker.InputRight;
        forwardLight = movement.Grounded && ((inputTracker.InputLeft && animationHandler.FacingLeft) || (inputTracker.InputRight && animationHandler.FacingRight));
        submergeLight = ((PlayerMovement)movement).Submerging && !inputTracker.InputLeft && !inputTracker.InputRight;
        dashLight = ((PlayerMovement)movement).Dashing && movement.Grounded;
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

    protected override void Attack()
    {
        if (recoveryTime.CurrentProgress != Timer.Progress.Ready)
            return;
        if (attackTime.CurrentProgress == Timer.Progress.InProgress)
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
            }
            else if (forwardLight && canForwardLight)
            {
                Debug.Log("ForwardBasic");
                attackTime.Duration = forwardLightDuration;
                attackTime.StartCooldown();
                canForwardLight = false;
            }
            else if (submergeLight && canSubmergeLight)
            {
                Debug.Log("SubmergeBasic");
                attackTime.Duration = submergedLightDuration;
                attackTime.StartCooldown();
                canSubmergeLight = false;
            }
            else if (dashLight && canDashLight)
            {
                Debug.Log("DashLight");
                attackTime.Duration = dashLightDuration;
                attackTime.StartCooldown();
                canDashLight = false;
            }
            else if (airLight && canAirLight)
            {
                Debug.Log("AirLight");
                attackTime.Duration = airLightDuration;
                attackTime.StartCooldown();
                canAirLight = false;
            }
            else if (airLightHigh && canAirLightHigh)
            {
                Debug.Log("AirLightHigh");
                attackTime.Duration = airLightHighDuration;
                attackTime.StartCooldown();
                canAirLightHigh = false;
            }
            else if (airLightLow && canAirLightLow)
            {
                Debug.Log("AirLightLow");
                attackTime.Duration = airLightLowDuration;
                attackTime.StartCooldown();
                canAirLightLow = false;
            }
        }
    }

    protected override void ParryState()
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

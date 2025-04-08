using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Combat : MonoBehaviour
{
    [Header("========== Base Configuration ==========")]
    [Header("--- Gizmo Configuration ---")]
    [SerializeField] protected bool gizmoToggleOn = true;

    //[Header("--- Basic Config ---")]
    //[SerializeField] protected float damage;
    //[SerializeField] protected float knockbackPower;

    [Header("--- Attack Cooldown ---")]
    [SerializeField] protected Timer attackCooldown;
    [SerializeField] protected Timer comboActiveTime;
    [SerializeField] protected Timer attackTime;

    [Header("--- Recovery Time ---")]
    [SerializeField] protected Timer recoveryTime;

    [Header("--- Parry ---")]
    [SerializeField] protected Timer parryActiveTime;
    [SerializeField] protected Timer parrySuccessTime;
    [SerializeField] protected Transform parryTransform;
    [SerializeField] protected Vector2 parryBoxSize;
    [SerializeField] protected LayerMask parryableLayer;
    protected Collider2D parryCollider;

    [Header("--- Hitstop ---")]
    [SerializeField] protected Timer hitstopDuration;

    protected float maxBasicCombo;
    protected float currentCombo;

    protected bool hitTarget;
    protected bool parriedAttack;

    protected bool parry;

    protected bool isAttacking;
    protected bool isDirectionLocked;
    protected bool isParry;

    protected bool canParry = true;

    [Header("--- Parry Duration ---")]
    
    [SerializeField] protected float parryDuration;

    protected Movement movement;
    protected Stats stats;

    public bool IsAttacking { get { return isAttacking; } }
    public bool IsDirectionLocked { get { return isDirectionLocked; } }
    public bool IsParry { get { return isParry; } }

    public bool Parry { get { return parry; } }

    //==================== GIZMOS ====================//
    private void OnDrawGizmos()
    {
        if (gizmoToggleOn != true)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(parryTransform.position, parryBoxSize);

    }

    protected virtual void Start()
    {
        if (stats == null)
        {
            stats = GetComponent<Stats>();
        }

        if (movement == null)
        {
            movement = GetComponent<Movement>();
        }
    }

    protected virtual void Update()
    {
        DetermineCombatState();
        DirectionLock();

        Timers();

        Attack();
        ParryState();
    }

    protected virtual void DirectionLock()
    {

    }

    protected virtual void Timers()
    {
        if (attackTime.CurrentProgress == Timer.Progress.Finished)
        {
            attackTime.ResetCooldown();
            recoveryTime.StartCooldown();
        }

        if (recoveryTime.CurrentProgress == Timer.Progress.Finished)
        {
            recoveryTime.ResetCooldown();
        }

        if (comboActiveTime.CurrentProgress == Timer.Progress.Finished)
        {
            comboActiveTime.ResetCooldown();
        }
    }

    protected virtual void DetermineCombatState()
    {
        isAttacking = attackTime.CurrentProgress == Timer.Progress.InProgress;
        parry = parryActiveTime.CurrentProgress == Timer.Progress.InProgress;
        parriedAttack = parryCollider;
    }

    protected virtual void Attack()
    {
        
    }

    protected virtual void ParryState()
    {
        //parryCollider = Physics2D.OverlapBox(parryTransform.position, parryBoxSize, 0, parryableLayer);

        //if (parryActiveTime.CurrentProgress is Timer.Progress.Ready)
        //{
        //    parryActiveTime.StartCooldown();
        //}

        //if (parriedAttack)
        //{
        //    attackCooldown.ResetCooldown();
        //    parrySuccessTime.StartCooldown();
        //}

        //if (parrySuccessTime.CurrentProgress is Timer.Progress.Finished)
        //{
        //    parrySuccessTime.ResetCooldown();
        //}

        //if (parryActiveTime.CurrentProgress is Timer.Progress.Finished)
        //{
        //    parryActiveTime.ResetCooldown();
        //}
    }

    protected virtual void HitStop()
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
}

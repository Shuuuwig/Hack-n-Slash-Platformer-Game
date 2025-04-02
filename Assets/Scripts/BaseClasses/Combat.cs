using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Combat : MonoBehaviour
{
    [Header("---Gizmo Configuration---")]
    [SerializeField] protected bool gizmoToggleOn = true;

    [Header("===Combat Configuration===")]
    [SerializeField] protected float damage;
    [SerializeField] protected float knockbackPower;

    [SerializeField] protected Cooldown attackDuration;
    [SerializeField] protected Cooldown attackCooldown;

    [SerializeField] protected Cooldown parryActiveTime;
    [SerializeField] protected Cooldown parrySuccessTime;
    [SerializeField] protected Transform parryTransform;
    [SerializeField] protected Vector2 parryBoxSize;
    [SerializeField] protected LayerMask parryableLayer;
    protected Collider2D parryCollider;

    protected bool hitEnemy;
    protected bool parriedAttack;
    protected bool neutralAttack;

    protected bool isDirectionLocked;
    protected bool isParrying;

    protected Movement movement;
    protected Stats stats;
    public bool IsDirectionLocked { get { return isDirectionLocked; } }

    

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
        
    }

    protected virtual void UpdateMovementStates()
    {
        isParrying = parryActiveTime.CurrentProgress == Cooldown.Progress.InProgress;
        parriedAttack = parryCollider;
    }

    protected virtual void DirectionLock()
    {
        
    }

    protected virtual void Attack()
    {
        
    }

    protected virtual void Parry()
    {
        parryCollider = Physics2D.OverlapBox(parryTransform.position, parryBoxSize, 0, parryableLayer);

        if (parriedAttack)
        {
            attackCooldown.ResetCooldown();
            parrySuccessTime.StartCooldown();
        }

        if (parrySuccessTime.CurrentProgress is Cooldown.Progress.Finished)
        {
            parrySuccessTime.ResetCooldown();
        }

        if (parryActiveTime.CurrentProgress is Cooldown.Progress.Finished)
        {
            parryActiveTime.ResetCooldown();
        }
    }
}

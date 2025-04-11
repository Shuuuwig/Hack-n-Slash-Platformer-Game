using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.AI;

public class HauntingBladeCombat : EnemyCombat
{
    [Header("========== Additional Configuration ==========")]
    [SerializeField] protected float neutralSlashDamageMultiplier;
    [SerializeField] protected float neutralSlashKnockbackMultiplier;
    [SerializeField] protected float neutralSlashDuration;
    [SerializeField] protected float neutralSlashRadius;
    [SerializeField] protected Collider2D neutralSlashCollider;
    protected Collider2D neutralSlashRange;

    [SerializeField] protected LayerMask attackLayer;

    protected bool neutralSlash;
 
    public bool NeutralSlash { get { return neutralSlash; } }
    public Collider2D NeutralSlashCollider 
    {  
        get { return neutralSlashCollider; } 
        set { neutralSlashCollider = value; }
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, neutralSlashRadius);
    }

    protected override void Start()
    {
        base.Start();
        movement = GetComponent<HauntingBladeMovement>();
        stats = GetComponent<HauntingBladeStats>();
    }

    protected override void Update()
    {
        base.Update();
        if (stats == null)
        {
            stats = GetComponent<HauntingBladeStats>();
            Debug.Log(stats);
        }
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        
    }

    protected override void DirectionLock()
    {

    }

    protected override void Timers()
    {
        if (attackTime.CurrentProgress == Timer.Progress.Finished)
        {
            neutralSlash = false;
        }
        base.Timers();
    }

    protected override void DetermineCombatState()
    {
        base.DetermineCombatState();
        //aggroPlayer = ((HauntingBladeMovement)movement).Aggressive;
        
        if (isAttacking || attackDowntime.CurrentProgress != Timer.Progress.Ready)
            return;

        int moveChoosen = Random.Range(0, 1);
        neutralSlashRange = Physics2D.OverlapCircle(transform.position, neutralSlashRadius, attackLayer);
        
        if (moveChoosen == 0)
        {
            neutralSlash = neutralSlashRange;
        }
    }

    protected override void Attack()
    {
        //if (!aggroPlayer)
        //    return;

        if (isAttacking || attackDowntime.CurrentProgress != Timer.Progress.Ready)
            return;

        if (neutralSlash)
        {
            attackTime.Duration = neutralSlashDuration;
            attackTime.StartCooldown();
            neutralSlash = true;
            finalizedDamage = stats.BaseDamage * neutralSlashDamageMultiplier;
            finalizedKnockback = stats.BaseKnockback * neutralSlashKnockbackMultiplier;
        }
    }
}

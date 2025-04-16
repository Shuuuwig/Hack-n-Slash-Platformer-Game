using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class EnemyCombat : MonoBehaviour
{
    [Header("========== Base Configuration ==========")]
    [Header("--- Gizmo Configuration ---")]
    [SerializeField] protected bool gizmoToggleOn = true;

    [Header("--- Attack Cooldown ---")]
    [SerializeField] protected Timer attackTime;
    [SerializeField] protected Timer attackDowntime;

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
    protected float finalizedKnockback;
    protected float bodyDamage;

    protected float maxBasicCombo;
    protected float currentCombo;

    protected bool aggroPlayer;
    protected bool parriedByPlayer;
    protected bool cancellable;
    protected bool hitTarget;
    protected bool parriedAttack;

    protected bool parry;

    protected bool isNearPlayer;
    protected bool isAttacking;
    protected bool isDirectionLocked;
    protected bool isParry;

    protected bool canAttack = true;
    protected bool canParry = true;

    protected EnemyMovement movement;
    protected EnemyStats stats;
    protected EnemyAnimationHandler animationHandler;

    [SerializeField] protected Transform playerTransform;
    [SerializeField] protected PlayerMovement playerMovement;
    [SerializeField] protected PlayerCombat playerCombat;
    [SerializeField] protected PlayerStats playerStats;

    public Timer AttackDowntime {  get { return attackDowntime; } }
    public float FinalizedDamage { get { return finalizedDamage; } }
    public float FinalizedKnockback { get { return finalizedKnockback; } }
    public float BodyDamage {  get { return bodyDamage; } }
    public bool IsAttacking { get { return isAttacking; } }
    public bool IsDirectionLocked { get { return isDirectionLocked; } }
    public bool IsParry { get { return isParry; } }

    public bool AggroPlayer { get { return aggroPlayer; } }
    public bool Parry { get { return parry; } }

    //==================== GIZMOS ====================//
    protected virtual void OnDrawGizmos()
    {
        if (gizmoToggleOn != true)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(parryTransform.position, parryBoxSize);

    }

    protected virtual void Start()
    {
        GameObject playerObject = GameObject.FindWithTag("Player");
        playerTransform = playerObject.transform;
        playerMovement = playerObject.GetComponent<PlayerMovement>();
        playerCombat = playerObject.GetComponent<PlayerCombat>();
        stats = GetComponent<EnemyStats>();

        bodyDamage = stats.BodyDamage;
    }

    protected virtual void Update()
    {
        DetermineCombatState();
        DirectionLock();

        Timers();

        Attack();
    }

    protected virtual void DirectionLock()
    {
        
    }

    protected virtual void Timers()
    {
        if (attackTime.CurrentProgress == Timer.Progress.Finished)
        {
            attackTime.ResetCooldown();
            attackDowntime.StartCooldown();
        }

        if (attackDowntime.CurrentProgress == Timer.Progress.Finished)
        {
            attackDowntime.ResetCooldown();
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

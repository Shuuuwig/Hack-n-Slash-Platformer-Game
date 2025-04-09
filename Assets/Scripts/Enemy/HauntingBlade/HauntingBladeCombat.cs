using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.AI;

public class HauntingBladeCombat : Combat
{
    [Header("========== Additional Configuration ==========")]

    //Component References
    [Header("---Component References---")]
    [SerializeField] protected Transform attackRangeTransform;
    [SerializeField] protected Transform parryAreaTransform;
    [SerializeField] protected Transform visionAreaTransform;
    [SerializeField] protected Transform effectiveRangeTransform;
    [SerializeField] protected Transform detectionAreaTransform;
    [SerializeField] protected Rigidbody2D enemyRigidbody;

    //Player References
    [SerializeField] protected Transform playerTransform;
    [SerializeField] protected PlayerMovement playerMovement;
    [SerializeField] protected PlayerCombat playerCombat;
    [SerializeField] protected PlayerStats playerStats;

    //Bools
    protected bool playerDetected;
    protected bool parriedByPlayer;
    protected bool canAttack;
    protected bool isStaggered;
    protected bool isNearPlayer;

    [SerializeField] private Collider2D slashCollider;
    [SerializeField] private Timer slashAttackDuration;
    [SerializeField] private Timer slashAttackCooldown;

    private bool neutralSlash;

    public bool NeutralSlash { get { return neutralSlash; } }
    public Collider2D SlashCollider
    {
        get { return slashCollider; }
        set { slashCollider = value; }
    }


    protected override void Start()
    {
       
    }

    protected override void Update()
    {

    }


    protected override void ParryState()
    {
    
    }

    protected void EnemyMoveset()
    {
        if (canAttack == true && slashAttackDuration.CurrentProgress is Timer.Progress.Ready && slashAttackCooldown.CurrentProgress is Timer.Progress.Ready)
        {
            neutralSlash = true;
            slashAttackDuration.StartCooldown();
            Debug.Log("Slashing");
        }

        if (slashAttackDuration.CurrentProgress is Timer.Progress.Finished)
        {
            neutralSlash = false;
            slashAttackCooldown.StartCooldown();
            slashAttackDuration.ResetCooldown();
        }

        if (slashAttackCooldown.CurrentProgress is Timer.Progress.Finished)
        {
            slashAttackCooldown.ResetCooldown();
        }
    }
}

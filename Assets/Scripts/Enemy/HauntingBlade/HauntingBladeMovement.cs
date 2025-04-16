using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HauntingBladeMovement : EnemyMovement
{
    [Header("========== Additional Configuration ==========")]
    [SerializeField] protected float detectorRadius;
    [SerializeField] protected LayerMask detectorLayer;
    protected Collider2D detectionCollider;

    [SerializeField] protected float obstacleCastDistance;
    [SerializeField] protected LayerMask obstacleLayer;
    protected RaycastHit2D obstacleCast;

    protected bool awaking;
    [SerializeField] protected Timer awakingTime;
    [SerializeField] protected Timer defensiveTime;
    [SerializeField] protected Timer passiveTimer;
    [SerializeField] protected float minPassiveTime;
    [SerializeField] protected float maxPassiveTime;
    protected bool passive;
    protected bool aggressive;

    public Timer AwakingTime { get { return awakingTime; } }
    public bool Aggressive 
    {
        get { return aggressive; }
        set { aggressive = value; }
    }
    public Timer DefensiveTime {  get { return defensiveTime; } }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, detectorRadius);

        if (playerTarget == null)
            return;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, playerTarget.position);

    }

    protected override void Start()
    {
        base.Start();
        playerTarget = GameObject.FindWithTag("Player").transform;
        animationHandler = GetComponent<HauntingBladeAnimationHandler>();

        combat = GetComponent<HauntingBladeCombat>();
        stats = GetComponent<HauntingBladeStats>();
    }

    protected override void Update()
    {
        PlayerCheck();
        BehaviourManager();
        base.Update();
    }

    protected override void Timers()
    {
        if (passiveTimer.CurrentProgress == Timer.Progress.Finished)
        {
            passiveTimer.ResetCooldown();
            passive = false;
        }
    }

    protected void PlayerCheck()
    {
        obstacleCastDistance = detectorRadius;
        detectionCollider = Physics2D.OverlapCircle(transform.position, detectorRadius, detectorLayer);
        obstacleCast = Physics2D.Raycast(transform.position, playerTarget.position - transform.position, obstacleCastDistance, obstacleLayer);

        if (detectionCollider && !obstacleCast)
        {
            if (awakingTime.CurrentProgress == Timer.Progress.Ready)
            {
                awaking = true;
                detectorRadius /= 2.5f;
                awakingTime.StartCooldown();
            }

            playerTooClose = true;
        }
        else
        {
            playerTooClose = false;
        }
    }

    protected override void BehaviourManager()
    {
        if (awakingTime.CurrentProgress != Timer.Progress.Finished || playerTooClose)
            return;

        if (passive || aggressive)
            return;

        float chooseBehaviour = Random.Range(0,2);

        if (chooseBehaviour == 0)
        {
            passive = true;
            aggressive = false;

            passiveTimer.Duration = Random.Range(minPassiveTime, maxPassiveTime);
            passiveTimer.StartCooldown();
            return;
        }

        if (chooseBehaviour == 1)
        {
            passive = false;
            aggressive = true;

            
            return;
        }

    }

    protected override void HorizontalMovement()
    {
        if (playerTarget == null || awakingTime.CurrentProgress != Timer.Progress.Finished)
            return;

        if (combat.IsAttacking)
        {
            attachedRigidbody.velocity = Vector2.zero;
            aggressive = false;
            return;
        }

        float direction = Mathf.Sign(playerTarget.position.x - transform.position.x);

        if (playerTooClose || passive)
        {
            finalizedSpeed = stats.BaseSpeed * speedBackwardsMultiplier;
            attachedRigidbody.velocity = new Vector2(-direction * finalizedSpeed, attachedRigidbody.velocity.y);
        }
        else if (aggressive)
        {
            finalizedSpeed = stats.BaseSpeed * speedForwardsMultiplier;
            attachedRigidbody.velocity = new Vector2(direction * finalizedSpeed, attachedRigidbody.velocity.y);
        }
    }

    protected override void VerticalMovement()
    {
    }

}

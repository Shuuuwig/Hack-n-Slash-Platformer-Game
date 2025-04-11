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
    [SerializeField] protected bool aggressive;
    [SerializeField] protected Timer defensiveTime;

    private List<Node> openList = new List<Node>();
    private List<Node> closedList = new List<Node>();
    [SerializeField] private Tilemap groundTilemap;

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
        base.Update();
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
            //BehaviourManager();
        }
        else
        {
            playerTooClose = false;
        }
    }

    protected override void BehaviourManager()
    {
        if (awakingTime.CurrentProgress != Timer.Progress.Finished)
            return;

        

        //if (!behaviourDetermined)
        //{
        //    selectedBehaviour = Random.Range(0, 2);

        //    if (selectedBehaviour == 0)
        //    {
        //        Debug.Log("Aggro");
        //        aggressive = true;
        //    }
        //    else if (selectedBehaviour == 1 && defensiveTime.CurrentProgress == Timer.Progress.Ready)
        //    {
        //        defensiveTime.StartCooldown();
        //    }

        //    behaviourDetermined = true;
        //}
    }

    protected void BehaviourReset()
    {

    }

    protected override void HorizontalMovement()
    {
        if (playerTarget == null || awakingTime.CurrentProgress != Timer.Progress.Finished)
            return;

        


        if (combat.IsAttacking)
        {
            attachedRigidbody.velocity = Vector2.zero;
            return;
        }

        if (playerTooClose)
        {
            finalizedSpeed = stats.BaseSpeed * speedBackwardsMultiplier;
            attachedRigidbody.velocity = new Vector2(Mathf.Sign(playerTarget.position.x - transform.position.x) * -finalizedSpeed, attachedRigidbody.velocity.y);
        }
        else
        {
            finalizedSpeed = stats.BaseSpeed * speedForwardsMultiplier;
            attachedRigidbody.velocity = new Vector2(Mathf.Sign(playerTarget.position.x - transform.position.x) * finalizedSpeed, attachedRigidbody.velocity.y);
        }
    }

    protected override void VerticalMovement()
    {
    }

}

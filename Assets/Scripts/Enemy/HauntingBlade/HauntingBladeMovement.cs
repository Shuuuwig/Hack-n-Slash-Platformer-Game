using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HauntingBladeMovement : Movement
{
    [Header("========== Additional Configuration ==========")]
    [SerializeField] protected float detectorRadius;
    [SerializeField] protected LayerMask detectorLayer;
    protected Collider2D detectionCollider;

    [SerializeField] protected float obstacleCastDistance;
    [SerializeField] protected LayerMask obstacleLayer;
    protected RaycastHit2D obstacleCast;

    [SerializeField] protected Transform playerTarget;

    protected bool awoking;
    [SerializeField] protected Timer awakingTime;

    private List<Node> openList = new List<Node>();
    private List<Node> closedList = new List<Node>();
    [SerializeField] private Tilemap groundTilemap;

    public Timer AwakingTime { get { return awakingTime; } }

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
            awoking = true;
            if (awakingTime.CurrentProgress == Timer.Progress.Ready)
                awakingTime.StartCooldown();

        }
    }

    protected override void HorizontalMovement()
    {
        if (playerTarget == null || awakingTime.CurrentProgress != Timer.Progress.Finished)
            return;

        attachedRigidbody.velocity = new Vector2(Mathf.Sign(playerTarget.position.x - transform.position.x) * speedForwards, attachedRigidbody.velocity.y);
    }

    protected override void VerticalMovement()
    {
    }

}

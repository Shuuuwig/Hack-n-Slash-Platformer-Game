using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyClass : MonoBehaviour
{
    [Header("---Gizmo Configuration---")]
    [SerializeField] protected bool gizmoToggleOn = true;

    [Header("---Enemy Configuration---")]
    [SerializeField] protected float health;
    [SerializeField] protected float damage;
    [SerializeField] protected float attackSpeed;
    [SerializeField] protected float knockbackForce;
    [SerializeField] protected float stunDuration;
    [SerializeField] protected float movementSpeed;
    
    [SerializeField] protected LayerMask detectableLayerMask;

    //Component References
    [Header("---Component References---")]
    [SerializeField] protected Transform enemyTransform;
    [SerializeField] protected Transform playerTransform;
    [SerializeField] protected Transform visionTransform;
    [SerializeField] protected Vector2 visionBoxSize;
    [SerializeField] protected Transform mainDetectionTransform;
    [SerializeField] protected Vector2 mainDetectionBoxSize;
    [SerializeField] protected PlayerMovement playerMovement;

    //Bools
    protected bool playerDetected;

    //Private Configuration
    protected Collider2D mainDetectionZone;
    protected Collider2D enemyVision;

    protected GameObject target;

    public float Health { get { return health; } set { health = value; } }
    public float Damage { get { return damage; } set { damage = value; } }
    public float AttackSpeed { get { return attackSpeed; } set { attackSpeed = value; } }
    public float KnockbackForce { get { return knockbackForce; } set { knockbackForce = value; } }
    public float StunDuration { get { return stunDuration; } set { stunDuration = value; } }
    public float MovementSpeed { get { return movementSpeed; } set { attackSpeed = value; } }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    protected virtual void EnemyMoveset()
    {

    }

    protected void KnockbackEffect()
    {
        playerMovement.KnockedBackState(enemyTransform, knockbackForce, stunDuration);
    }

    protected void TakingDamage()
    {
        Debug.Log("Took Damage");
    }

    protected void PlayerDetection()
    {
        //Player detection colliders
        mainDetectionZone = Physics2D.OverlapBox(mainDetectionTransform.position, mainDetectionBoxSize, 0, detectableLayerMask);
        enemyVision = Physics2D.OverlapBox(visionTransform.position, visionBoxSize, 0, detectableLayerMask);

        //Return if no player collider detected
        if (enemyVision == null || mainDetectionBoxSize == null)
        {
            playerDetected = false;
            return;
        }

        //Move towards player if player collider detected
        if (enemyVision.CompareTag("Player") && mainDetectionZone.CompareTag("Player"))
        {
            target = enemyVision.gameObject;
            playerDetected = true;
            Debug.Log("Sees player");
        }
    }

    private void OnDrawGizmos()
    {
        if (gizmoToggleOn != true)
            return;

        Gizmos.DrawWireCube(visionTransform.position, visionBoxSize);
        Gizmos.DrawWireCube(mainDetectionTransform.position, mainDetectionBoxSize);
    }

}

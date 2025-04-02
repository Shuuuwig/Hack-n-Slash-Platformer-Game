using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.AI;

public abstract class EnemyClass : MonoBehaviour
{
    [Header("---Gizmo Configuration---")]
    [SerializeField] protected bool gizmoToggleOn = true;

    [Header("---Enemy Configuration---")]
    [SerializeField] protected float maxHealth;
    protected float currentHealth;
    [SerializeField] protected float damage;
    [SerializeField] protected float speed;
    [SerializeField] protected Vector2 parryArea;
    [SerializeField] protected Vector2 visionArea;
    [SerializeField] protected float effectiveRangeRadius;
    [SerializeField] protected Vector2 detectableArea;
    [SerializeField] protected LayerMask playerLayer;
    protected Collider2D parryCollider;
    protected Collider2D visionCollider;
    protected Collider2D effectiveRangeCollider;
    protected Collider2D detectableCollider;
    protected Collider2D obstacleFrontCollider;
    protected Collider2D obstacleBackCollider;
    protected Transform target;

    [Header("---Cooldowns---")]
    [SerializeField] protected Cooldown attackCooldown;
    [SerializeField] protected Cooldown staggeredDuration;

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

    //
    protected Vector2 movementDirection;

    //Bools
    protected bool playerDetected;
    protected bool parriedByPlayer;
    protected bool canAttack;
    protected bool canParry;
    protected bool isStaggered;
    protected bool isNearPlayer;

    public float Health { get { return currentHealth; } set { currentHealth = value; } }
    public float Damage { get { return damage; } set { damage = value; } }

    protected void Start()
    {
        currentHealth = maxHealth;
        GameObject Player = GameObject.FindWithTag("Player");
        if (Player == null)
        {
            Debug.Log("NO PLAYER FOUND");
        }
        playerMovement = Player.GetComponentInParent<PlayerMovement>();
        playerCombat = Player.GetComponentInParent<PlayerCombat>();
        playerStats = Player.GetComponentInParent<PlayerStats>();
        playerTransform = Player.transform;
    }

    protected virtual void Update()
    {
        VisionCheck();
        DetectionCheck();
        EffectiveRangeCheck();

        EnemyMoveset();
        EnemyMovement();
        EnemyState();
        AggroPlayer();
    }

    //===========================Collision Check===========================
    protected virtual void VisionCheck()
    {
        visionCollider = Physics2D.OverlapBox(visionAreaTransform.position, visionArea, 0, playerLayer);
    }

    protected virtual void DetectionCheck()
    {
        detectableCollider = Physics2D.OverlapBox(detectionAreaTransform.position, detectableArea, 0, playerLayer);
    }

    protected virtual void ParryCheck()
    {
        parryCollider = Physics2D.OverlapBox(parryAreaTransform.position, parryArea, 0, playerLayer);

        if (parryCollider == true)
        {
            canParry = true;
        }
        else
        {
            canParry = false;
        }
    }

    protected virtual void EffectiveRangeCheck()
    {
        effectiveRangeCollider = Physics2D.OverlapCircle(effectiveRangeTransform.position, effectiveRangeRadius, playerLayer);

        if (effectiveRangeCollider)
        {
            isNearPlayer = true;
            Debug.Log("Effective range");
        }
        else
        {
            isNearPlayer = false;
        }
    }

    protected virtual void FrontCollisionCheck()
    {

    }

    protected virtual void BackCollisionCheck()
    {

    }

    //===========================================================================
    protected virtual void EnemyMoveset()
    {
        //Different for all
    }

    protected virtual void EnemyMovement()
    {
        if (playerDetected == false || canAttack == true)
            return;

        movementDirection = new Vector2(transform.position.x - playerTransform.position.x, 0);
        Debug.Log(movementDirection.x);

        if (movementDirection.x > 0)
        {
            enemyRigidbody.velocity = new Vector2(-speed, enemyRigidbody.velocity.y);
        }
        else if (movementDirection.x < 0)
        {
            enemyRigidbody.velocity = new Vector2(speed, enemyRigidbody.velocity.y);
        }
    }

    protected virtual void AggroPlayer()
    {
        if (visionCollider == true && detectableCollider == true)
        {
            target = visionCollider.transform;
            Debug.Log(target);
            playerDetected = true;
            Debug.Log("Sees player");
        }
        else
        {
            target = null;
            playerDetected = false;
        }
    }

    protected virtual void EnemyState()
    {
        if (playerDetected == false)
        {
            canAttack = false;
        }
        else if (playerDetected == true)
        {
            if (isNearPlayer == true)
            {
                Debug.Log("Near");
                canAttack = true;
            }
            else if (isNearPlayer == false)
            {
                Debug.Log("Not Near");
                canAttack = false;
            }
        }
    }

    protected virtual void GuardUpState()
    {

    }

    //protected virtual void Staggered()
    //{
    //    if (playerCombat.ParriedAttack)
    //    {
    //        isStaggered = true;
    //        staggeredDuration.StartCooldown();
    //    }

    //    if (staggeredDuration.CurrentProgress is Cooldown.Progress.Finished)
    //    {
    //        isStaggered = false;
    //        staggeredDuration.ResetCooldown();
    //    }
    //}

    protected virtual void TakeDamage()
    {
        Debug.Log("Took damage");

        if (currentHealth <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    protected virtual void DealDamage()
    {
        if (playerStats == null)
            return;

        Debug.Log("Deal damage");
        playerStats.PlayerCurrentHealth -= damage;
    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            DealDamage();
            Debug.Log("Collision");
        }
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerWeapon"))
        {
            TakeDamage();
            Debug.Log("Trigger collision");
        }
    }

    protected void OnDrawGizmos()
    {
        if (gizmoToggleOn != true)
            return;
        //Parry box
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(parryAreaTransform.position, parryArea);
        //Vision box
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(visionAreaTransform.position, visionArea);
        //Parry box
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(effectiveRangeTransform.position, effectiveRangeRadius);
        //Detection zone
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(detectionAreaTransform.position, detectableArea);
    }
}

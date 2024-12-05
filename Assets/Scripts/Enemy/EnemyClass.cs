using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.LowLevel;

public abstract class EnemyClass : MonoBehaviour
{
    [Header("---Gizmo Configuration---")]
    [SerializeField] protected bool gizmoToggleOn = true;

    [Header("---Enemy Configuration---")]
    [SerializeField] protected float maxHealth;
    protected float currentHealth;
    [SerializeField] protected float damage;
    [SerializeField] protected float attackSpeed;
    [SerializeField] protected float knockbackForce;
    [SerializeField] protected float stunDuration;
    [SerializeField] protected float movementSpeed;   
    [SerializeField] protected LayerMask detectableLayerMask;
    [SerializeField] protected Vector2 parryBoxSize;
    [SerializeField] protected Vector2 visionBoxSize;
    [SerializeField] protected Vector2 detectionBoxSize;
    [SerializeField] protected Vector2 attackBoxSize;
    protected Collider2D attackBox;

    [Header("---Cooldowns---")]
    [SerializeField] protected Cooldown attackCooldown;
    [SerializeField] protected Cooldown staggeredDuration;

    //Component References
    [Header("---Component References---")]
    [SerializeField] protected Transform attackBoxTransform;
    [SerializeField] protected Transform attackRangeBoxTransform;
    [SerializeField] protected Transform parryBoxTransform;
    [SerializeField] protected Transform playerTransform;
    [SerializeField] protected Transform visionBoxTransform;
    [SerializeField] protected Transform detectionBoxTransform;
    [SerializeField] protected LayerMask playerLayer;
    [SerializeField] protected Collider2D neutralAttackCollider;

    [SerializeField] protected PlayerMovement playerMovement;
    [SerializeField] protected PlayerCombat playerCombat;
    [SerializeField] protected PlayerStats playerStats;

    //Bools
    protected bool playerDetected;
    protected bool canAttack;
    protected bool parriedByPlayer;

    //Others
    protected GameObject target;

    public float Health { get { return currentHealth; } set { currentHealth = value; } }
    public float Damage { get { return damage; } set { damage = value; } }
    public float AttackSpeed { get { return attackSpeed; } set { attackSpeed = value; } }
    public float KnockbackForce { get { return knockbackForce; } set { knockbackForce = value; } }
    public float StunDuration { get { return stunDuration; } set { stunDuration = value; } }
    public float MovementSpeed { get { return movementSpeed; } set { attackSpeed = value; } }

    protected void Start()
    {
        currentHealth = maxHealth;

        playerMovement = FindObjectOfType<PlayerMovement>();
        playerCombat = FindObjectOfType<PlayerCombat>();
        playerStats = FindObjectOfType<PlayerStats>();
    }

    protected virtual void Update()
    {
        EnemyMoveset();
        AggroPlayer();

        PlayerDetection();
    }

    //-------------
    protected virtual void EnemyMoveset()
    {
        if (playerDetected != true)
            return;

        //Move 1: Basic Swing
        if (attackCooldown.CurrentProgress is Cooldown.Progress.Ready)
        {
            attackBox = Physics2D.OverlapBox(attackBoxTransform.position, attackBoxSize, 0, playerLayer);
        }
    }

    protected virtual void AggroPlayer()
    {
        if (target == null)
            return;

        Debug.Log("Moving to player");
    }

    //---Effects---

    //---States---
    protected virtual void GuardUpState()
    {

    }

    protected virtual void Staggered()
    {
        if (playerCombat.ParriedAttack)
        {
            staggeredDuration.StartCooldown();
        }

        if (staggeredDuration.CurrentProgress is Cooldown.Progress.Finished)
        {
            staggeredDuration.ResetCooldown();
        }
    }

    protected void TakeDamage()
    {
        currentHealth -= playerStats.PlayerCurrentDamage;

        if (currentHealth <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    protected void DealDamage()
    {
        if (playerStats == null)
            return;

        playerStats.PlayerCurrentHealth -= damage;
    }

    //---Collision/Trigger Check---
    protected void PlayerDetection()
    {
        //Detect player
        if (Physics2D.OverlapBox(detectionBoxTransform.position, detectionBoxSize, 0, detectableLayerMask) && Physics2D.OverlapBox(visionBoxTransform.position, visionBoxSize, 0, detectableLayerMask))
        {
            target = GameObject.FindWithTag("Player");
            playerDetected = true;
            Debug.Log("Sees player");
        }
        else
        {
            target = null;
            playerDetected = false;
        }
    }


    protected virtual void AnimationHandler()
    {

    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            DealDamage();
        }

        if (collision.gameObject.CompareTag("PlayerWeapon"))
        {
            TakeDamage();
        }
    }

    protected void OnDrawGizmos()
    {
        if (gizmoToggleOn != true)
            return;
        //Attack box
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(attackBoxTransform.position, attackBoxSize);
        //Parry box
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(parryBoxTransform.position, parryBoxSize);
        //Vision box
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(visionBoxTransform.position, visionBoxSize);
        //Detection zone
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(detectionBoxTransform.position, detectionBoxSize);
    }
}

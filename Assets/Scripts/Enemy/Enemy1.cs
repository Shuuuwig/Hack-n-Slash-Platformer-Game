using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy1 : MonoBehaviour
{
    [Header("---Enemy Configuration---")]
    [SerializeField] private Transform visionTransform;
    [SerializeField] private Vector2 visionBoxSize;
    [SerializeField] private Transform mainDetectionTransform;
    [SerializeField] private Vector2 mainDetectionBoxSize;
    [SerializeField] private LayerMask detectableLayerMask;

    [Header("---Gizmo Configuration---")]
    [SerializeField] private bool gizmoToggleOn = true;

    [Header("---Others---")]
    [SerializeField] private Transform playerTransform;

    //Bools
    private bool playerDetected;

    //Private Configuration
    private Collider2D mainDetectionZone;
    private Collider2D enemyVision;
    private EnemyClass enemyClass = new EnemyClass(10f, 5f, 1f, 0.2f);

    private GameObject target;
    private void Start()
    {
        

    }

    private void Update()
    {
        PlayerDetection();
    }

    private void Enemy1Moveset()
    {
        
    }

    private void TakingDamage()
    {
        Debug.Log("Took Damage");
        enemyClass.Health -= 5f;

        if (enemyClass.Health <= 0 ) 
        {
            Destroy(gameObject);
        }
    }

    private void PlayerDetection()
    {
        //Detect for player collider 
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Weapon"))
        {
            TakingDamage();
        }
    }
}

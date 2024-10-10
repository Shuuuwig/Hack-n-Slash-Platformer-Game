using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy1 : EnemyClass
{
    

    private void Start()
    {
        

    }

    private void Update()
    {
        PlayerDetection();
    }

    protected override void EnemyMoveset()
    {      
        if (playerDetected != true)
            return;

        //Move 1: Basic Swing
        if (attackCooldown.CurrentProgress is Cooldown.Progress.Ready)
        {

        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Weapon"))
        {
            TakingDamage();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Collided with player");
            KnockbackEffect();
        }
    }
}

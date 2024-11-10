using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class Scout : EnemyClass
{
    protected override void Update()
    {
        base.Update();
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
        
    }
}

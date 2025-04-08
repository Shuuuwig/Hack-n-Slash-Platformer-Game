using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : Status
{
    [SerializeField] protected Timer InvulnerabilityDuration;

    private bool canTakeDamage = true;

    void Update()
    {
        //OnDeath();
        InvulnerabilityFrames();
    }

    //private void OnDeath()
    //{
    //    if (currentHealth <= 0)
    //    {
    //        Destroy(this.gameObject);
    //    }
    //}

    private void InvulnerabilityFrames()
    {
        if (canTakeDamage == true)
            return;

        if (InvulnerabilityDuration.CurrentProgress is Timer.Progress.Ready)
        {
            InvulnerabilityDuration.StartCooldown();
            Debug.Log("Currently Invulnerable");
        }

        if (InvulnerabilityDuration.CurrentProgress is Timer.Progress.Finished)
        {
            InvulnerabilityDuration.ResetCooldown();
            Debug.Log("No longer Invulnerable");
            canTakeDamage = true;
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Obstacle"))
        {
            if (canTakeDamage == true)
            {
                canTakeDamage = false;
            }
        }
    }
}

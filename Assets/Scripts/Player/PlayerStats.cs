using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float currentHealth;

    [SerializeField] private Cooldown InvulnerabilityDuration;

    private bool tookDamage;

    public float PlayerCurrentHealth
    {
        get { return currentHealth; }
        set 
        { 
            if (tookDamage == true && InvulnerabilityDuration.CurrentProgress is Cooldown.Progress.Ready)
            {
                currentHealth = value;
            }
             
        }
    }
    void Start()
    {
        
    }

    void Update()
    {
        OnDeath();
        InvulnerabilityFrames();
    }

    private void OnDeath()
    {
        if (currentHealth <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    private void InvulnerabilityFrames()
    {
        if (tookDamage == false)
            return;

        if (InvulnerabilityDuration.CurrentProgress is Cooldown.Progress.Ready)
        {
            InvulnerabilityDuration.StartCooldown();
            Debug.Log("Currently Invulnerable");
        }
        else if (InvulnerabilityDuration.CurrentProgress is Cooldown.Progress.Finished)
        {
            InvulnerabilityDuration.ResetCooldown();
            Debug.Log("No longer Invulnerable");
            tookDamage = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Obstacle"))
        {
            tookDamage = true;
        }
    }
}

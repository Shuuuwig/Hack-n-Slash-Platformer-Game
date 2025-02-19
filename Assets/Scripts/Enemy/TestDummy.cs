using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDummy : MonoBehaviour
{
    [SerializeField] private bool toggleSlash;
    [SerializeField] private bool toggleThrust;
    [SerializeField] private bool toggleParry;

    [SerializeField] private Cooldown slashDuration;
    [SerializeField] private Cooldown thrustDuration;
    [SerializeField] private Cooldown attackCooldown;

    [SerializeField] private Animator animator;

    [SerializeField] private Collider2D slashCollider;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        BasicSlash();
    }

    private void BasicSlash()
    {
        if (toggleSlash == false)
        {
            animator.Play("Idle");
            return;
        }

        if (attackCooldown.CurrentProgress is Cooldown.Progress.Ready)
        {
            animator.Play("Slash");
            slashCollider.enabled = true;
            attackCooldown.StartCooldown();
        }
        else if (attackCooldown.CurrentProgress is Cooldown.Progress.Finished)
        {
            slashCollider.enabled = false;
            attackCooldown.ResetCooldown();
        }

    }
}

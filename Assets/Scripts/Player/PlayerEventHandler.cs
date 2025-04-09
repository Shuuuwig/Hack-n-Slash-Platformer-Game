using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEventHandler : MonoBehaviour
{
    [SerializeField] private Collider2D neutralLightCollider;
    [SerializeField] private Collider2D airLightCollider;
    [SerializeField] private Collider2D airLowLightCollider;
    [SerializeField] private Collider2D airHighLightCollider;
    [SerializeField] private Collider2D submergeLightCollider;

    //Player Component Reference
    [Header("---Component Reference---")]
    [SerializeField] private PlayerCombat playerCombat;
    [SerializeField] private PlayerMovement playerMovement;

    void Start()
    {

    }

    void Update()
    {

    }

    private void NeutralAttack()
    {
        neutralLightCollider.enabled = !neutralLightCollider.enabled;
    }

    private void SubmergedAttack()
    {

    }

    private void AirAttack()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEventHandler : MonoBehaviour
{
    [Header("---Component Reference---")]
    [SerializeField] private PlayerCombat combat;
    [SerializeField] private PlayerMovement movement;
    private void Parry()
    {
        combat.ParryCollider.enabled = !combat.ParryCollider.enabled;
    }

    private void NeutralLight()
    {
        combat.NeutralLightCollider.enabled = !combat.NeutralLightCollider.enabled;
    }

    private void AirLight()
    {
        combat.AirLightCollider.enabled = !combat.AirLightCollider.enabled;
    }

    private void AirLightLow()
    {
        combat.AirLightLowCollider.enabled = !combat.AirLightLowCollider.enabled;
    }

    private void AirLightHigh()
    {
        combat.AirLightHighCollider.enabled = !combat.AirLightHighCollider.enabled;
    }

    private void SubmergedLight()
    {
        combat.SubmergedLightCollider.enabled = !combat.SubmergedLightCollider.enabled;
    }
}
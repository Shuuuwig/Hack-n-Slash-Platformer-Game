using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HauntingBladeEventHandler : MonoBehaviour
{
    [SerializeField] private HauntingBladeCombat combat;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void HauntingBladeSlash()
    {
        combat.NeutralSlashCollider.enabled = !combat.NeutralSlashCollider.enabled;
    }
}

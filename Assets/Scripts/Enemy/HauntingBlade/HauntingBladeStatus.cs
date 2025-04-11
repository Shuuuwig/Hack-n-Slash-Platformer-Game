using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class HauntingBladeStatus : EnemyStatus
{
    protected override void Start()
    {
        stats = GetComponent<HauntingBladeStats>();
        movement = GetComponent<HauntingBladeMovement>();
        combat = GetComponent<HauntingBladeCombat>();
    }
}

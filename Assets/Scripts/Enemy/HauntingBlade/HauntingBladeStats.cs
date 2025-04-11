using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HauntingBladeStats : EnemyStats
{
    protected override void Start()
    {
        status = GetComponent<HauntingBladeStatus>();
    }

    protected override void Update()
    {
        
    }
}

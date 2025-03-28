using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour
{
    protected Dictionary<string, string> moveset = new Dictionary<string, string>();

    public Dictionary<string, string> Moveset { get { return moveset; } }

    protected bool neutralAttack;

    protected bool isDirectionLocked;

    public bool IsDirectionLocked { get { return isDirectionLocked; } }

    protected virtual void DirectionLock()
    {
        
    }
}

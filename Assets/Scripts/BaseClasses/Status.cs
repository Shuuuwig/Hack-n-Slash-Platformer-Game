using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour
{
    protected bool isKnockedback;
    protected bool isSlowed;
    protected bool isStunned;
    protected bool isParalyzed;
    protected bool isBurned;
    protected bool isKnockedUp;

    public bool IsKnockedback 
    { 
        get { return isKnockedback; } 
        set { isKnockedback = value; } 
    }
    public bool IsSlowed
    {
        get { return isSlowed; }
        set { isSlowed = value; }
    }
    public bool IsStunned 
    { 
        get { return isStunned; }
        set { isStunned = value; }
    }
    public bool IsParalyzed 
    {  
        get { return isParalyzed; } 
        set { isParalyzed = value; }
    }
    public bool IsBurned 
    { 
        get { return isBurned; } 
        set { isBurned = value; }
    }
    public bool IsKnockedUp 
    { 
        get {  return isKnockedUp; }
        set { isKnockedUp = value;}
    }


}

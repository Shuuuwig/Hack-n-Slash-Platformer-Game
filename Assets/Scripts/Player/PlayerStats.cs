using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Stats Base Template")]
public class PlayerStats : ScriptableObject
{
    public enum BaseStats
    {
        health,
        damage,

    }

    public Dictionary<BaseStats, float> baseStats = new Dictionary<BaseStats, float>();
    public Dictionary<BaseStats, float> localStats = new Dictionary<BaseStats, float>();

    public void InitializeStats()
    {
        localStats = new Dictionary<BaseStats, float>(baseStats);
    }
}

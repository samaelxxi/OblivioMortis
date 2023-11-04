using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [CreateAssetMenu(fileName = "ShooterStats", menuName = "ScriptableObjects/ShooterStats", order = 1)]
public class ShooterStats : EnemyStats
{
    
    [field: SerializeField, Min(0.1f), Header("Shooter Stats")]
    public float MinDistanceFromPlayer { get; private set; } = 7f;
    [field: SerializeField, Min(0.1f)]
    public float MaxDistanceFromPlayer { get; private set; } = 13f;

    [field: SerializeField, Min(0.1f)]
    public float DelayBetweenShots { get; private set; } = 0.5f;
}

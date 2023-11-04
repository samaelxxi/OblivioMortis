using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// [CreateAssetMenu(fileName = "EnemyStats", menuName = "ScriptableObjects/EnemyStats", order = 1)]
[System.Serializable]
public class EnemyStats : ScriptableObject
{
    [field: SerializeField, Min(1)]
    public int Health { get; private set; } = 5;
    [field: SerializeField, Min(0.1f)] 
    public float Speed { get; private set; } = 4f;
    [field: SerializeField, Min(0.1f)] 
    public float RotationSpeed { get; private set; } = 4f;

    [field: SerializeField, Min(0f), Header("Attack Stats")]
    public float AttackPreparationTime { get; private set; } = 0.5f;
    [field: SerializeField, Min(0)]
    public float AttackRestTime { get; private set; } = 0.5f;
    [field: SerializeField, Min(0.1f)]
    public float AttackCooldown { get; private set; } = 1f;

    [field: SerializeField, Min(0), Header("Agro Stats")]
    public float AgroRadius { get; private set; } = 10f;
    [field: SerializeField, Min(0)]
    public float ChaseRadius { get; private set; } = 30f;

    [field: SerializeField, Min(0), Header("Resistance Stats")]
    public float KnockbackResistance { get; private set; } = 1;
    [field: SerializeField, Tooltip("If player melee attack interrupts")]
    public bool MeleeInterruptionResistance { get; private set; } = false;
    [field: SerializeField, Tooltip("If player ranged attack interrupts")]
    public bool RangedInterruptionResistance { get; private set; } = false;
    [field: SerializeField, Min(0)]
    public float StaggerResistance { get; private set; } = 1;
}

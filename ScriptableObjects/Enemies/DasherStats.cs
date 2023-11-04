using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// [CreateAssetMenu(fileName = "DasherStats", menuName = "ScriptableObjects/DasherStats", order = 1)]
[System.Serializable]
public class DasherStats : EnemyStats
{
    [field: SerializeField, Min(0.1f), Header("Dasher Stats")]
    public float StartAttackDistance { get; private set; } = 3f;
    [field: SerializeField, Min(0)]
    public int AttackDamage { get; private set; } = 1;
    [field: SerializeField, Min(0)]
    public int AttackKnockback { get; private set; } = 1;
    [field: SerializeField, Min(0.1f)]
    public float LungeDistance { get; private set; } = 4f;
    [field: SerializeField, Min(0.1f)]
    public float LungeTime { get; private set; } = 1f;
    public float LungeSpeed => LungeDistance / LungeTime;

    [field: SerializeField]
    public AnimationCurve LungeCurve { get; private set; } = AnimationCurve.Linear(0, 0, 1, 1);
}

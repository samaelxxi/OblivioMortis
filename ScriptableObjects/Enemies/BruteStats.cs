using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [CreateAssetMenu(fileName = "BruteStats", menuName = "ScriptableObjects/BruteStats", order = 1)]
public class BruteStats : EnemyStats
{
    [field: SerializeField, Min(0.1f), Header("Brute Stats")]
    public float StartAttackDistance { get; private set; } = 3f;

    [field: SerializeField, Min(0)]
    public int AttackDamage { get; private set; } = 1;
    [field: SerializeField, Min(0.1f)]
    public float AttackKnockback { get; private set; } = 1f;
    [field: SerializeField, Min(0.1f)]
    public float AttackAOESize { get; private set; } = 1f;



    [field: SerializeField, Min(0.1f), Space(4)]
    public float LungeDistance { get; private set; } = 4f;
    [field: SerializeField, Min(0.1f)]
    public float LungeTime { get; private set; } = 1f;
    public float LungeSpeed => LungeDistance / LungeTime;
    [field: SerializeField]
    public AnimationCurve LungeCurve { get; private set; } = AnimationCurve.Linear(0, 0, 1, 1);

    [field: SerializeField, Min(0.1f), Space(4)]
    public float JumpTime { get; private set; } = 0.85f;
    [field: SerializeField, Min(0.1f)]
    public float JumpHeight { get; private set; } = 5f;
    [field: SerializeField, Min(0.1f), Space(4)]
    public float JumpMaxDistance { get; private set; } = 10;
    [field: SerializeField]
    public AnimationCurve JumpCurve { get; private set; } = AnimationCurve.Linear(0, 0, 1, 1);
}

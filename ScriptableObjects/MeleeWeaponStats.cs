using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MeleeWeaponStats", menuName = "ScriptableObjects/Player/MeleeWeaponStats", order = 1)]
public class MeleeWeaponStats : ScriptableObject
{
    [field: SerializeField, Range(0.1f, 10)]
    public int Damage { get; private set; } = 1;
    [field: SerializeField, Range(0.1f, 3)]
    public float DamageAreaWidth { get; private set; } = 1;
    [field: SerializeField, Range(0.1f, 3)]
    public float DamageAreaLength { get; private set; } = 0.5f;
    [field: SerializeField, Range(0, 1)]
    public float AttackPreparationTime { get; private set; } = 0.1f;
    [field: SerializeField, Range(0, 2)]
    public float AttackDuration { get; private set; } = 0.2f;
    [field: SerializeField, Range(0, 1)]
    public float AttackRestTime { get; private set; } = 0.1f;
    public float AttackTotalTime => AttackPreparationTime + AttackDuration + AttackRestTime;
    [field: SerializeField, Range(0, 2)]
    public float AttackCooldown { get; private set; } = 1;
    [field: SerializeField, Range(0.01f, 5)]
    public float LungeDistance { get; private set; } = 1;
    [field: SerializeField, Range(0.01f, 1)]
    public float LungeTime { get; private set; } = 0.2f;
    [field: SerializeField]
    public AnimationCurve LungeCurve { get; private set; } = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float LungeSpeed => LungeDistance / LungeTime;

    [field: SerializeField, Min(0)]
    public float AttackStagger { get; private set; }
    [field: SerializeField, Min(0)]
    public float AttackKnockback { get; private set; }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "RangedWeaponStats", menuName = "ScriptableObjects/Player/RangedWeaponStats", order = 1)]
public class RangedWeaponStats : ScriptableObject
{
    [field: SerializeField, Min(1)]
    public int MaxAmmo { get; private set; } = 6;

    [field: SerializeField, Min(0.1f)]
    public float FireRate { get; private set; } = 3;

    [field: SerializeField, Min(0)]
    public float Stagger { get; private set; } = 0;
    [field: SerializeField, Min(0)]
    public float Knockback { get; private set; } = 0;

    [field:SerializeField, Range(0, 10)]
    public int MaxAimBounces { get; private set; } = 3;
    [field:SerializeField, Min(1)]
    public float MaxAimRayLength { get; private set; } = 50;
}

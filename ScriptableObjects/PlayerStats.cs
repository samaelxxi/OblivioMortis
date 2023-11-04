using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [CreateAssetMenu(fileName = "PlayerStats", menuName = "ScriptableObjects/Player/PlayerStats", order = 1)]
public class PlayerStats : ScriptableObject
{
    [field: SerializeField, Min(1)] public int MaxHealth { get; private set; } = 4;
    [field: SerializeField, Min(0)] public int MaxHealthItems { get; private set; } = 2;
    [field: SerializeField, Min(1)] public int MaxSoulFuel { get; private set; } = 4;
    [field: SerializeField, Min(0)] public int MaxSoulParts { get; private set; } = 8;
    [field: SerializeField, Min(0)] public float GrabSoulRadius { get; private set; } = 10f;
    [field: SerializeField, Min(0)] public float InvulnerabilityTime  { get; private set; } = 1;


    [field: SerializeField, Range(0, 1), Space(4)] public float AnimationDamp { get; private set; } = 0.1f;

    [field: SerializeField, Header("Ability stats"), Range(0.01f, 1)] public float AbilitySlowDownTimeScale { get; private set; } = 0.2f;
    [field: SerializeField, Range(0.01f, 10)] public float SlowDownTime { get; private set; } = 4;
    public float AbilityDurationScaled => SlowDownTime * AbilitySlowDownTimeScale;
    [field: SerializeField, Range(0, 0.5f), Header("Impacts")] public float HitStaggerTime { get; private set; } = 0.1f;
    [field: SerializeField] public float CameraZoomInOnMeleeDistance { get; private set; } = 0.5f;
    [field: SerializeField] public float CameraZoomOutOnRicochet { get; private set; } = 2;

    [field: SerializeField, Space(6)] public PlayerMovementStats Movement { get; private set; }
    [field: SerializeField] public MeleeWeaponStats MeleeWeapon { get; private set; }
    [field: SerializeField] public RangedWeaponStats RangedWeapon { get; private set; }
}

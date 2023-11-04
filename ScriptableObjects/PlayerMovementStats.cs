using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "PlayerMovementStats", menuName = "ScriptableObjects/Player/PlayerMovementStats", order = 1)]
public class PlayerMovementStats : ScriptableObject
{
    [field: SerializeField, Header("Movement")]
    public  float MaxMoveSpeed { get; private set; } = 10f;
    [field: SerializeField]
    public float Acceleration { get; private set; }  = 15f;
    [field: SerializeField]
    public float RotationSpeed { get; private set; }  = 10f;


    [field: SerializeField, Header("Air Movement")]
    public float MaxAirMoveSpeed { get; private set; }  = 15f;
    [field: SerializeField]
    public float AirAccelerationSpeed { get; private set; }  = 15f;
    [field: SerializeField, Tooltip("How much air speed is slowed(kinda) down")]
    public float Drag { get; private set; }  = 0.1f;


    [field: SerializeField, Header("Dashing")]
    public float DashDistance { get; private set; }  = 4f;
    [field: SerializeField]
    public float DashTime { get; private set; } = 0.5f;
    [field: SerializeField]
    public float DashPrepareTime { get; private set; } = 0.1f;
    [field: SerializeField]
    public float DashRecoveryTime { get; private set; } = 0.1f;
    [field: SerializeField]
    public float DashCooldown { get; private set; } = 1f;
    [field: SerializeField]
    public AnimationCurve DashCurve { get; private set; } = AnimationCurve.EaseInOut(0, 0, 1, 1);

    public float DashSpeed => DashDistance / DashTime;
    public float TotalDashTime => DashPrepareTime + DashTime + DashRecoveryTime;

    [field: SerializeField, Foldout("Misc")]
    public List<Collider> IgnoredColliders { get; private set; }  = new();
    [field: SerializeField, Foldout("Misc")]
    public Vector3 Gravity { get; private set; }  = new Vector3(0, -30f, 0);
}

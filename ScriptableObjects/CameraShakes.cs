using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public enum NoiseShakeType
{
    DamageTaken, MeleeAttack, BruteAttack
}

public enum ImpulseShakeType
{
    Shoot, MeleeAttack
}

[CreateAssetMenu(fileName = "CameraShakes", menuName = "OblivioMortis/CameraShakes", order = 1)]
public class CameraNoiseShakes : ScriptableObject
{
    [Serializable]
    public class ShakesDictionary : SerializableDictionary<NoiseShakeType, Shake> {}
    [Serializable]
    public class ImpulseShakesDictionary : SerializableDictionary<ImpulseShakeType, ImpulseShake> {}

    [System.Serializable]
    public struct Shake
    {
        // public NoiseShakeType Type;
        public NoiseSettings NoiseSettings;
        public float Time;
    }

    [System.Serializable]
    public struct ImpulseShake
    {
        public AnimationCurve Curve;
        public float Time;
        public float Strength;
    }

    [SerializeField] ShakesDictionary _shakesDictionary = new();

    public Shake GetShake(NoiseShakeType type)
    {
        return _shakesDictionary[type];
    }

    [SerializeField] ImpulseShakesDictionary _impulseShakesDictionary = new();

    public ImpulseShake GetImpulseShake(ImpulseShakeType type)
    {
        return _impulseShakesDictionary[type];
    }
}

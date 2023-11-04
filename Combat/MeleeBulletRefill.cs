using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MeleeBulletRefill : MonoBehaviour
{
    [SerializeField] BulletEffectType _effect = BulletEffectType.Default;

    public BulletEffectType Effect => _effect;
}

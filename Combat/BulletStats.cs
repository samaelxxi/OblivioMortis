using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;


public enum BulletEffectType { Default, AOE, TwoForOne, Knockback,
    ShooterBullet, TestBullet }


[CreateAssetMenu(fileName = "BulletStats", menuName = "OblivioMortis/BulletStats", order = 1)]
public class BulletStats : ScriptableObject
{
    [field: SerializeField]
    public int Damage { get; private set; } = 1;
    [field: SerializeField]
    public float Radius { get; private set; } = 0.2f;

    [field: SerializeField]
    public float Speed { get; private set; } = 10;

    [field: SerializeField]
    public float Knockback { get; private set; } = 0;

    [field: SerializeField]
    public float Stagger { get; private set; } = 0;


    [field: SerializeField, Space(5)] 
    public bool IsHasExtraBullets { get; private set; } = false;
    [field: SerializeField,  ShowIf("IsHasExtraBullets")]
    public int BonusBulletNumber { get; private set; } = 1;
    [field: SerializeField, ShowIf("IsHasExtraBullets")]
    public float TimeBetweenBullets { get; private set; } = 0.1f;


    [field: SerializeField, Space(5)] 
    public bool IsHasAOE { get; private set; } = false;
    [field: SerializeField, ShowIf("IsHasAOE")]
    public float AreaOfEffect { get; private set; } = 5;

    [field: SerializeField]
    public Color UIColor { get; private set; } = Color.white;
    [SerializeField, ColorUsage(true, true)]
    Color _hitColor = Color.red;
    public Color HitColor => _hitColor;
}

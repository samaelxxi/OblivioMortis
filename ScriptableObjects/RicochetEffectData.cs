using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;


public enum RicochetSpecialEffect
{
    None, DoubleBullet, AOE
}

[CreateAssetMenu(fileName = "RicochetEffectData", menuName = "OblivioMortis/Combat/RicochetEffectData", order = 1)]
public class RicochetEffectData : ScriptableObject
{
    [field: SerializeField]
    public int BonusDamage { get; private set; }
    [field: SerializeField]
    public float BonusRadius { get; private set; }
    [field: SerializeField]
    public float BonusKnockback { get; private set; }
    [field: SerializeField]
    public float BonusStagger { get; private set; }
    [field: SerializeField]
    public float BonusSpeed { get; private set; }

    [field: SerializeField]
    public RicochetSpecialEffect SpecialEffect { get; private set; }
    [field: SerializeField, ShowIf("SpecialEffect", RicochetSpecialEffect.AOE)]
    public float AreaOfEffect { get; private set; }
    [field: SerializeField, ShowIf("SpecialEffect", RicochetSpecialEffect.DoubleBullet)]
    public float DelayBetweenBullets { get; private set; }
}

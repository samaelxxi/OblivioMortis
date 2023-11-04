using UnityEngine;

public class Dasher : Enemy
{
    [SerializeField] KnockbackDamageZone _knockbackDamageZone;
    [SerializeField] ParticleSystem _attackVFX;

    public DasherStats DasherStats => _stats as DasherStats;

    public float StartAttackDistance => DasherStats.StartAttackDistance;
    public float LungeDistance => DasherStats.LungeDistance;
    public float LungeTime => DasherStats.LungeTime;
    public float LungeSpeed => DasherStats.LungeSpeed;
    public AnimationCurve LungeCurve => DasherStats.LungeCurve;

    public override OblivioSounds AttackSound => OblivioSounds.DasherAttack;
    public override OblivioSounds DamagedSound => OblivioSounds.DasherDamaged;
    public override EnemyType EnemyType => EnemyType.Dasher;

    protected override void Awake()
    {
        base.Awake();
        _knockbackDamageZone.SetDamage(DasherStats.AttackDamage);
        _knockbackDamageZone.SetKnockback(DasherStats.AttackKnockback);
        
        #if UNITY_EDITOR
        _animator.ChangeStateDuration("AttackPreparation", AttackPreparationTime);
        _animator.ChangeStateDuration("AttackEnd", AttackRestTime);
        #endif
        OnStaggered += () => _attackVFX.Stop();
    }

    protected override void AttackPreparation()
    {
        base.AttackPreparation();
        JSAM.AudioManager.PlaySound(AttackSound, transform.position);
        _attackVFX.Play();
    }

    protected override void AttackRest()
    {
        base.AttackRest();
        _attackVFX.Stop();
    }
}

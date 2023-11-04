using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

public class Shooter : Enemy
{
    [SerializeField] Transform _projectileSpawnPoint;
    [SerializeField] Light _shootLight;
    [SerializeField] VisualEffect _shootEffect;
    [SerializeField] VisualEffect _attackPrepEffect;

    public ShooterStats ShooterStats => _stats as ShooterStats;

    public float MinDistanceFromPlayer => ShooterStats.MinDistanceFromPlayer;
    public float MaxDistanceFromPlayer => ShooterStats.MaxDistanceFromPlayer;
    public float PreferredDistanceFromPlayer => (MinDistanceFromPlayer + MaxDistanceFromPlayer) / 2;
    public float DelayBetweenShots => ShooterStats.DelayBetweenShots;

    public override OblivioSounds AttackSound => OblivioSounds.ShooterAttack;
    // public override OblivioSounds DamagedSound => OblivioSounds.ShooterDamaged;
    public override EnemyType EnemyType => EnemyType.Shooter;


    protected override void Awake()
    {
        base.Awake();
        #if UNITY_EDITOR
        _animator.ChangeStateDuration("AttackPreparation", AttackPreparationTime);
        _animator.ChangeStateDuration("AttackEnd", AttackRestTime);
        #endif
        OnStaggered += () => _attackPrepEffect.Stop();
    }


    public void Shoot()
    {
        var bullet = ServiceLocator.Get<BulletFactory>().SpawnBullet(BulletType.Shooter, BulletEffectType.ShooterBullet, _projectileSpawnPoint.position, false);
        bullet.Launch(transform.forward);
        _shootLight.DOIntensity(0, 0.1f).From(5);
        _shootEffect.Play();
    }

    // was testing this, but it's not working well
    public Vector3 GetPredictedPos(Vector3 player, Vector3 playerVelocity)
    {
        if (playerVelocity.magnitude < 0.05f)
            return player;

        var playerPos = player;
        var playerSpeed = playerVelocity.magnitude;
        var playerDirection = playerVelocity.normalized;
        var playerDistance = Vector3.Distance(transform.position, playerPos);
        var timeToReachPlayer = playerDistance / playerSpeed;
        var predictedPos = playerPos + playerDirection * timeToReachPlayer;
        return predictedPos;
    }

    protected override void AttackPreparation()
    {
        base.AttackPreparation();
        _attackPrepEffect.Play();
    }

    protected override void Attack()
    {
        base.Attack();
        JSAM.AudioManager.PlaySound(AttackSound, transform.position);
        _attackPrepEffect.Stop();
        Shoot();
    }

    protected override void AttackEnd()
    {
        base.AttackEnd();
        _attackPrepEffect.Stop();
    }
}

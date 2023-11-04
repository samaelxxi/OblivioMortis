using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiAimBullet : Bullet
{
    List<Vector3> _targets = new();
    List<Transform> _enemies = new();


    int _currentTarget;

    protected override void CleanOnRelease()
    {
        base.CleanOnRelease();
        _targets.Clear();
        _enemies.Clear();
    }

    public override void Setup(Bullet bullet)
    {
        base.Setup(bullet);
        var multiAimBullet = bullet as MultiAimBullet;
        _targets = multiAimBullet._targets;
        _enemies = multiAimBullet._enemies;
        _currentTarget = multiAimBullet._currentTarget;
    }

    public override Bullet Copy()
    {
        var bullet = ServiceLocator.Get<BulletFactory>().GetBullet(BulletType.MultiAim);
        bullet.Setup(this);
        return bullet;
    }

    public void SetTargets(List<Vector3> targets, List<Transform> enemies)
    {
        _targets.Clear();
        _enemies.Clear();
        foreach (var target in targets)
            _targets.Add(target);
        foreach (var enemy in enemies)
            _enemies.Add(enemy);
        _currentTarget = 0;
    }

    bool ShouldBounce()
    {
        if (_currentTarget >= _targets.Count - 1)  // last target or no targets
            return false;

        var targetPos = _enemies[_currentTarget] == null ? _targets[_currentTarget] : _enemies[_currentTarget].position;
        bool _hitTargetEnemy = _enemies[_currentTarget] != null && _hitInfo.collider.transform == _enemies[_currentTarget];
        if (!_hitTargetEnemy && (_hitInfo.point - targetPos).magnitude > Radius*2)
            return false;  // didn't hit target

        return true;
    }

    protected void ActivateSpecialRicochetEffect(RicochetEffectData effect)
    {
        switch (effect.SpecialEffect)
        {
            case RicochetSpecialEffect.None:
                break;
            case RicochetSpecialEffect.DoubleBullet:
                var bullet = Copy();
                bullet.gameObject.SetActive(false);
                bullet.transform.position = transform.position;
                StaticCoroutine.StartInSec(() => bullet.gameObject.SetActive(true), effect.DelayBetweenBullets / _currentTarget);
                break;
        }
    }


    protected override void Update()
    {
        _liveTime += Time.deltaTime;
        if (_liveTime > Globals.BULLET_LIFE_TIME)
        {
            Release();
            return;
        }

        float distThisFrame = Speed * Time.deltaTime;
        if (CheckHit())
        {
            HitSomething();
            if (ShouldBounce())
            {
                Vector3 newMovementDirection;

                if (_enemies[_currentTarget+1] != null)
                    newMovementDirection = (_enemies[_currentTarget+1].position - transform.position).normalized;
                else
                    newMovementDirection = (_targets[_currentTarget+1] - transform.position).normalized;

                Vector3 midPos = _hitInfo.point + _hitInfo.normal * Radius;
                float distBeforeHit = Vector3.Distance(transform.position, midPos);
                transform.position = midPos;
                transform.position += newMovementDirection * (distThisFrame - distBeforeHit);
                _movementDirection = newMovementDirection;
                transform.rotation = Quaternion.LookRotation(_movementDirection);
                _currentTarget++;

                if (_hitInfo.collider.gameObject.TryGetComponent<RicochetEffect>(out var effect))
                {
                    MergeEffect(effect.Effect);
                    ActivateSpecialRicochetEffect(effect.Effect);
                }

                JSAM.AudioManager.PlaySound(OblivioSounds.Ricochet, transform.position);
            }
            else
            {
                GetDestroyed();
            }
        }
        else
            transform.position += Speed * Time.deltaTime * _movementDirection;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RicochetBullet : Bullet
{
    int _bounces = 0;

    int _maxBounces = 3;


    public override void Setup(Bullet bullet)
    {
        base.Setup(bullet);
        var ricochetBullet = bullet as RicochetBullet;
        _bounces = ricochetBullet._bounces;
        _maxBounces = ricochetBullet._maxBounces;
    }

    public override Bullet Copy()
    {
        var bullet = ServiceLocator.Get<BulletFactory>().GetBullet(BulletType.Ricochet);
        bullet.Setup(this);
        return bullet;
    }



    public void SetMaxBounces(int bounces)
    {
        _maxBounces = bounces;
    }

    protected override void CleanOnRelease()
    {
        base.CleanOnRelease();
        _bounces = 0;
    }

    bool ShouldBounce()
    {
        return ((1 << _hitInfo.collider.gameObject.layer) & PhysicsMasks.BouncableMask) != 0 &&
                _bounces < _maxBounces;
    }

    void ActivateSpecialRicochetEffect(RicochetEffectData effect)
    {
        switch (effect.SpecialEffect)
        {
            case RicochetSpecialEffect.None:
                break;
            case RicochetSpecialEffect.DoubleBullet:
                var bullet = Copy();
                bullet.gameObject.SetActive(false);
                bullet.transform.position = transform.position;
                StaticCoroutine.StartInSec(() => bullet.gameObject.SetActive(true), effect.DelayBetweenBullets / _bounces);
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

                _movementDirection = Vector3.Reflect(_movementDirection, _hitInfo.normal);
                // hit.point is point on surface, not center of sphere so need to find real mid-pos
                var newPos = _hitInfo.point + _hitInfo.normal * Radius;
                var distBeforeHit = Vector3.Distance(transform.position, newPos);
                transform.position = newPos;
                transform.position += _movementDirection * (distThisFrame - distBeforeHit);
                transform.rotation = Quaternion.LookRotation(_movementDirection);
                _bounces++;

                if (_hitInfo.collider.gameObject.TryGetComponent<RicochetEffect>(out var effect))
                {
                    MergeEffect(effect.Effect);
                    ActivateSpecialRicochetEffect(effect.Effect);
                }

                JSAM.AudioManager.PlaySound(OblivioSounds.Ricochet, transform.position);
            }
            else
                GetDestroyed();
        }
        else
            transform.position += Speed * Time.deltaTime * _movementDirection;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using NaughtyAttributes;


public class Bullet : DesignPatterns.Poolable<Bullet>
{
    protected struct HitInfo
    {
        public Vector3 point;
        public Vector3 normal;
        public Collider collider;
    }


    [SerializeField] bool _isHasVFX;
    [SerializeField, ShowIf("_isHasVFX")] VisualEffect _trailVFX;
    [SerializeField] bool _isHasTrail;  // prob should move somewhere dedicated
    [SerializeField, ShowIf("_isHasTrail")] TrailRenderer _trailRenderer;

    public int Damage { get; private set; }
    public float Radius { get; private set; }
    public float Speed { get; private set; }
    public float Knockback { get; private set; }
    public float Stagger { get; private set; }
    public bool IsHasAOE { get; private set; }
    public float AreaOfEffect { get; private set; }

    public int CollisionMask => _collisionMask;


    Collider[] _colliders = new Collider[1];
    protected Vector3 _movementDirection;
    protected int _collisionMask;
    protected float _liveTime = 0;
    protected bool _isEnemyHit;
    float _defaultTrailWidth;
    Color _hitColor = Color.red;

    protected HitInfo _hitInfo = new();


    protected void Awake()
    {
        if (_isHasTrail)
            _defaultTrailWidth = _trailRenderer.widthMultiplier;
    }

    public virtual void Setup(BulletStats effect, bool isPlayerBullet)
    {
        _liveTime = 0;
        Damage = effect.Damage;
        Radius = effect.Radius;
        Speed = effect.Speed;
        Knockback = effect.Knockback;
        Stagger = effect.Stagger;
        IsHasAOE = effect.IsHasAOE;
        AreaOfEffect = effect.AreaOfEffect;
        _hitColor = effect.HitColor;

        transform.localScale = 2 * Radius * Vector3.one;
        _collisionMask = isPlayerBullet ? PhysicsMasks.PlayerBulletTargetMask : PhysicsMasks.EnemyBulletTargetMask;
    
        if (_isHasTrail)
            _trailRenderer.widthMultiplier = _defaultTrailWidth * Radius * 2;
    }

    public virtual void Setup(Bullet bullet)
    {
        _liveTime = 0;
        Damage = bullet.Damage;
        Radius = bullet.Radius;
        transform.localScale = 2 * Radius * Vector3.one;
        Speed = bullet.Speed;
        Knockback = bullet.Knockback;
        Stagger = bullet.Stagger;
        IsHasAOE = bullet.IsHasAOE;
        AreaOfEffect = bullet.AreaOfEffect;
        _movementDirection = bullet._movementDirection;

        _collisionMask = bullet._collisionMask;

        if (_isHasTrail)
            _trailRenderer.widthMultiplier = _defaultTrailWidth * Radius * 2;
    }

    public virtual Bullet Copy()
    {
        var bullet = ServiceLocator.Get<BulletFactory>().GetBullet(BulletType.Default);
        bullet.Setup(this);
        return bullet;
    }

    protected void MergeEffect(RicochetEffectData effect)  // TODO move to ricochet bullet?
    {
        // Debug.Log("Merging effect " + effect.name);
        Damage = Mathf.Max(0, Damage + effect.BonusDamage);
        Speed = Mathf.Max(0.1f, Speed + effect.BonusSpeed);
        Radius = Mathf.Max(0.01f, Radius + effect.BonusRadius);
        if (_isHasTrail)
            _trailRenderer.widthMultiplier = _defaultTrailWidth * Radius * 2;
        transform.localScale = 2 * Radius * Vector3.one;
        Knockback = Mathf.Max(0, Knockback + effect.BonusKnockback);
        Stagger = Mathf.Max(0, Stagger + effect.BonusStagger);

        if (effect.SpecialEffect == RicochetSpecialEffect.AOE && !IsHasAOE)
        {
            IsHasAOE = true;
            AreaOfEffect = effect.AreaOfEffect;
        }
    }

    protected bool CheckHit()
    {
        Vector3 startPos = transform.position;

        // well, spherecast doesn't detect collision if start pos is inside collider
        // so we move it back a bit in hope that it won't move back too much
        // and then spherecast
        // this happens when target collider is too close to bullet spawn point
        // and sometimes happens that bullet flies through target because of some fixed update stuff
        // should've used trigger collider but it's too late now(or not?)
        int movedBackDist = 0;
        while (Physics.OverlapSphereNonAlloc(startPos, Radius, _colliders, _collisionMask) > 0)
        {
            movedBackDist++;
            startPos -= _movementDirection;
        }

        if (Physics.SphereCast(startPos, Radius, _movementDirection, 
                    out var hit, Speed * Time.deltaTime + movedBackDist, _collisionMask))
        {
            _hitInfo.collider = hit.collider;
            _hitInfo.point = hit.point;
            _hitInfo.normal = hit.normal;
            return true;
        }

        return false;
    }

    // protected void ActivateSpecialRicochetEffect(RicochetEffectData effect)
    // {
    //     switch (effect.SpecialEffect)
    //     {
    //         case RicochetSpecialEffect.None:
    //             break;
    //         case RicochetSpecialEffect.DoubleBullet:
    //             var bullet = Copy();
    //             bullet.transform.position = transform.position + _movementDirection * 0.1f;
    //             bullet.gameObject.SetActive(false);
    //             StaticCoroutine.StartInSec(() => bullet.gameObject.SetActive(true), effect.DelayBetweenBullets);
    //             break;
    //     }
    // }

    protected override void CleanOnRelease()
    {
        IsHasAOE = false;
        AreaOfEffect = 0;
        _movementDirection = Vector3.zero;
        _isEnemyHit = false;
    }

    public void AddStagger(float stagger)
    {
        Stagger += stagger;
    }

    public void AddKnockback(float knockback)
    {
        Knockback += knockback;
    }

    public void Launch(Vector3 direction)
    {
        _movementDirection = direction;
        if (_isHasVFX)
        {
            _trailVFX.Reinit();
            _trailVFX.Play();
            _trailVFX.gameObject.SetActive(true);
        }
        if (_isHasTrail)
        {
            _trailRenderer.transform.SetParent(transform);
            _trailRenderer.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
            _trailRenderer.Clear();
        }
        transform.rotation = Quaternion.LookRotation(direction);
    }

    protected virtual void Update()
    {
        _liveTime += Time.deltaTime;
        if (_liveTime > Globals.BULLET_LIFE_TIME)
        {
            Release();
            return;
        }

        if (CheckHit())
        {
            HitSomething();
            GetDestroyed();
        }
        else
            transform.position += Speed * Time.deltaTime * _movementDirection;
    }

    protected virtual void HitSomething()
    {
        _isEnemyHit = false;
        if (_hitInfo.collider.gameObject.TryGetComponent<ColliderObserver>(out var lever))
        {
            lever.UseBullet(this);  // TODO use actor-reactor?
        }
        else
        {
            if (!IsHasAOE)  // explosion will deal with damage
            {
                if (_hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    _isEnemyHit = true;
                    _hitInfo.collider.GetComponent<Enemy>().GetBulletHit(this, -_hitInfo.normal);
                }
                else if (_hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
                    _hitInfo.collider.GetComponent<Player>().TakeDamageWithKnockback(Damage, -_hitInfo.normal, Knockback);
            }
            else
                Explode();
        }
        DrawHitVFX();
    }

    protected void Explode()
    {
        var explosion = ServiceLocator.Get<PrefabsHolder>().GetNewInstance<Explosion>();
        explosion.gameObject.transform.position = transform.position;
        explosion.Setup(this); // might want pooling
        explosion.Explode();
        JSAM.AudioManager.PlaySound(OblivioSounds.Explosion, transform.position);
    }

    protected void GetDestroyed()
    {
        if (!_isEnemyHit)
            JSAM.AudioManager.PlaySound(OblivioSounds.BulletHitSurface, transform);
        if (_isHasTrail)
            _trailRenderer.transform.SetParent(null);  // let trail render till the end
        Release();
    }

    protected void DrawHitVFX()
    {
        var vfx = ServiceLocator.Get<VFXManager>().GetVFX(VFXType.BulletHit, 200);
        vfx.SetVector4("Color", _hitColor);
        ServiceLocator.Get<VFXManager>().SpawnDecal(DecalType.BulletHit, 
                    _hitInfo.point, Quaternion.LookRotation(-_hitInfo.normal));
        vfx.transform.position = _hitInfo.point + _hitInfo.normal * 0.3f;
        vfx.SetVector3("HitNormal", _hitInfo.normal);
        vfx.Play();
    }
}

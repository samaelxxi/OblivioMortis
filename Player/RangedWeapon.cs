using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.VFX;

public class RangedWeapon : MonoBehaviour
{
    [SerializeField] Transform _shootPoint;
    [SerializeField] LineRenderer _aimRay;
    [SerializeField] VisualEffect _shootEffect;
    [SerializeField] Light _shootLight;

    public Vector3 ShootPoint => _shootPoint.position;
    public Vector3 ShootPointDirection => _shootPoint.forward;

    public event Action OnShoot;
    public event Action<int> OnAmmoChanged;
    public event Action<int> OnMaxAmmoChanged;
    public event Action<BulletEffectType> OnBulletEffectChanged;

    public int Ammo { get; private set; } = 0;
    public int MaxAmmo { get; private set; } = 0;


    RangedWeaponStats _stats;
    BulletEffectType _bulletEffect;

    float _timeSinceLastShot = Mathf.Infinity;
    bool _isShooting = false;
    bool _isAbilityOn = false;
    float _aimWidth;
    BulletStats _bulletStats;
    BulletFactory _bulletFactory;
    PlayerInputs _inputs;

    readonly Vector3[] _shootRayPositions = new Vector3[10];
    List<Vector3> _multiAimTargets = new();
    List<Transform> _multiAimEnemies = new();


    int _abilityType = 0; // 0 - ricochet, 1 - multi aim


    void Awake()
    {
        _aimWidth = _aimRay.widthMultiplier;
        _aimRay.widthMultiplier = 0;
    }

    void Start()
    {
        _bulletFactory = ServiceLocator.Get<BulletFactory>();
        _inputs = ServiceLocator.Get<PlayerInputs>();
        SetEffect(BulletEffectType.Default);
        OnBulletEffectChanged?.Invoke(_bulletEffect);  // setup ui
    }

    public void RefillAmmo(BulletEffectType effect)
    {
        if (effect != _bulletEffect)
        {
            SetEffect(effect);
            OnBulletEffectChanged?.Invoke(effect);
        }
        Ammo = MaxAmmo;
        OnAmmoChanged?.Invoke(Ammo);
    }

    public void GetUpgrade(PlayerUpgradeType type)
    {
        switch (type)
        {
            case PlayerUpgradeType.Ammo:
                MaxAmmo++;
                OnMaxAmmoChanged?.Invoke(MaxAmmo);
                RefillAmmo(_bulletEffect);
                break;
        }
    }

    public void SetStats(RangedWeaponStats stats)
    {
        _stats = stats;
        MaxAmmo = stats.MaxAmmo;
        Ammo = MaxAmmo;
    }

    public void SetEffect(BulletEffectType effect)
    {
        _bulletEffect = effect;
        _bulletStats = _bulletFactory.GetStatsByType(effect);
    }

    void Update()
    {
        _timeSinceLastShot += Time.deltaTime;
        if (_isAbilityOn)
            DrawAimRay();
    }

    public bool CanShoot()
    {
        return ReadyToShoot() && Ammo > 0;
    }

    public bool ReadyToShoot()
    {
        return _timeSinceLastShot >= 1 / _stats.FireRate && !_isShooting;
    }

    public void Use()
    {
        if (_isAbilityOn && _abilityType == 1)
        {
            ConfirmMultiAim();
            return;
        }

        if (!CanShoot())
        {
            JSAM.AudioManager.PlaySound(OblivioSounds.PlayerShotNoAmmo);
            return;
        }

        if (_bulletStats.IsHasExtraBullets)  // can transform into strategy pattern
            StartCoroutine(MultipleShot(_bulletStats.BonusBulletNumber, _bulletStats.TimeBetweenBullets));
        else
            ShootInner();

        if (!Cheats.InfiniteAmmo)
        {
            Ammo--;
            OnAmmoChanged?.Invoke(Ammo);
        }
    }

    bool _shouldShootSpecialBullet = false;
    void ShootInner()
    {
        Bullet bullet;
        if (!_shouldShootSpecialBullet)
        {
            bullet = _bulletFactory.SpawnBullet(BulletType.Default, _bulletEffect, _shootPoint.position);
            bullet.Launch(GetBulletDirection());
        }
        else
        {
            if (_abilityType == 1)  // TODO refactor when ability choosen
            {
                bullet = ShootMultiAimBullet();
                _shouldShootSpecialBullet = false;
            }
            else
            {
                bullet = ShootRicochetBullet();
            }
        }
        bullet.AddKnockback(_stats.Knockback);
        bullet.AddStagger(_stats.Stagger);
        OnShoot?.Invoke();
        _shootEffect.Play();
        _shootLight.DOIntensity(0, 0.25f).From(5);
        _timeSinceLastShot = 0;
        CreateBulletShell();
    }

    void CreateBulletShell()
    {
        var shell = ServiceLocator.Get<VFXManager>().GetStuff(StuffType.BulletShell, 30);
        shell.transform.position = ShootPoint - ShootPointDirection * 0.1f;
        var shellBody = shell.GetComponent<Rigidbody>();
        shellBody.velocity = Vector3.zero;
        shellBody.angularVelocity = Vector3.zero;
        var vect = UnityEngine.Random.onUnitSphere;
        shellBody.AddTorque(vect * 0.2f, ForceMode.Impulse);
        vect = new Vector3(vect.x / 3, Mathf.Abs(vect.y), -Mathf.Abs(vect.z));
        vect *= UnityEngine.Random.Range(0.5f, 1.5f);
        shellBody.AddForce(transform.TransformDirection(vect), ForceMode.Impulse);
    }

    MultiAimBullet ShootMultiAimBullet()
    {
        MultiAimBullet multiBullet = _bulletFactory.SpawnBullet(BulletType.MultiAim, _bulletEffect, _shootPoint.position) as MultiAimBullet;
        multiBullet.SetTargets(_multiAimTargets, _multiAimEnemies);
        if (_multiAimTargets.Count > 0)
        {
            multiBullet.Launch((_multiAimTargets[0] - _shootPoint.position).normalized);
        }
        else
            multiBullet.Launch(GetBulletDirection());

        return multiBullet;
    }

    RicochetBullet ShootRicochetBullet()
    {
        RicochetBullet ricochetBullet = _bulletFactory.SpawnBullet(BulletType.Ricochet, _bulletEffect, _shootPoint.position) as RicochetBullet;
        ricochetBullet.SetMaxBounces(_stats.MaxAimBounces);
        ricochetBullet.Launch(GetBulletDirection());
        return ricochetBullet;
    }

    Vector3 GetBulletDirection()
    {
        return (CalculateWorldMousePoint() - _shootPoint.position).normalized;
    }

    IEnumerator MultipleShot(int bullets, float delay)
    {
        _isShooting = true;
        for (int i = 0; i < bullets + 1; i++)
        {
            ShootInner();
            yield return new WaitForSeconds(delay);
        }
        _isShooting = false;
    }

    public void ActivateAbility(int abilityType)
    {
        _abilityType = abilityType;
        if (abilityType == 1)
        {            
            _multiAimTargets.Clear();
            _multiAimEnemies.Clear();
        }
        else
            _shouldShootSpecialBullet = true;

        _isAbilityOn = true;
        DOTween.To(() => _aimRay.widthMultiplier, x => _aimRay.widthMultiplier = x, _aimWidth, 0.2f);
        _aimRay.enabled = true;
    }

    public void DeactivateAbility()
    {
        if (!_isAbilityOn)
            return;

        _isAbilityOn = false;
        if (_abilityType == 1)
        {
            _shouldShootSpecialBullet = true;
            if (CanShoot())
                Use();
        }
        else
            _shouldShootSpecialBullet = false;

        var tweener = DOTween.To(() => _aimRay.widthMultiplier, x => _aimRay.widthMultiplier = x, 0, 0.2f);
        tweener.onComplete += () => _aimRay.enabled = false;
    }

    void DrawAimRay()
    {
        if (_abilityType == 0)
            DrawRicochetRay();
        else
            DrawMultiAimRay();
    }

    Vector3 CalculateWorldMousePoint()  // TODO duplication...
    {
        var ray = Camera.main.ScreenPointToRay(_inputs.MouseLook);
        var shootPlane = new Plane(transform.up, ShootPoint);
        shootPlane.Raycast(ray, out var distance);
        return ray.GetPoint(distance);
    }

    void ConfirmMultiAim()
    {
        Vector3 currentPoint = _shootPoint.position;
        if (_multiAimTargets.Count > 0)
            currentPoint = _multiAimTargets[^1];
        Vector3 rayDirection = CalculateWorldMousePoint() - currentPoint;
        Vector3 nextPoint;
        RaycastHit hit = CalculateNextRayHit(currentPoint, rayDirection, _stats.MaxAimRayLength);
        if (hit.collider != null)
            nextPoint = hit.point + hit.normal * _bulletStats.Radius;
        else
            nextPoint = currentPoint + rayDirection * _stats.MaxAimRayLength;


        Collider[] colliders = new Collider[1];
        if (Physics.OverlapSphereNonAlloc(nextPoint, 0.1f, colliders, PhysicsMasks.EnemyMask) > 0)
            _multiAimEnemies.Add(colliders[0].transform);
        else
            _multiAimEnemies.Add(null);

        _multiAimTargets.Add(nextPoint);
    }

    void DrawRicochetRay()
    {
        _shootRayPositions[0] = _shootPoint.position;
        float remainingLength = _stats.MaxAimRayLength;
        Vector3 rayDirection = GetBulletDirection();
        Vector3 currentPoint = _shootPoint.position;
        int rayIndex = 1;

        while (remainingLength > 0 && rayIndex < _shootRayPositions.Length && rayIndex <= _stats.MaxAimBounces)
        {
            var hit = Physics.SphereCast(currentPoint, _bulletStats.Radius, rayDirection, 
                                out var hitInfo, remainingLength, PhysicsMasks.PlayerBulletTargetMask);
            if (hit)
            {
                 // use sphere center as start to keep accuracy but use hit point to show ray not ending in air
                _shootRayPositions[rayIndex] = hitInfo.point;
                currentPoint = hitInfo.point + hitInfo.normal * _bulletStats.Radius;
                remainingLength -= hitInfo.distance;
                rayDirection = Vector3.Reflect(rayDirection, hitInfo.normal);
            }
            else
            {
                currentPoint += rayDirection * remainingLength;
                remainingLength = -1;
                _shootRayPositions[rayIndex] = currentPoint;
            }
            rayIndex++;
        }
        _aimRay.positionCount = rayIndex;
        _aimRay.SetPositions(_shootRayPositions);
    }

    void DrawMultiAimRay()
    {
        Vector3 currentPoint = _shootPoint.position;
        if (_multiAimTargets.Count > 0)
            currentPoint = _multiAimTargets[^1];
        Vector3 rayDirection = CalculateWorldMousePoint() - currentPoint;

        Vector3 nextPoint = CalculateNextRayPoint(currentPoint, rayDirection, _stats.MaxAimRayLength);

        _shootRayPositions[0] = _shootPoint.position;
        int i;
        for (i = 0; i < _multiAimTargets.Count; i++)
        {
            if (_multiAimEnemies[i] != null)
                _shootRayPositions[i+1] = _multiAimEnemies[i].position;
            else
                _shootRayPositions[i+1] = _multiAimTargets[i];
        }
        _shootRayPositions[i+1] = nextPoint;
        _shootRayPositions[i+2] = _shootRayPositions[i];  // to make ray don't dissapear
        _aimRay.positionCount = _multiAimTargets.Count + 3;
        _aimRay.SetPositions(_shootRayPositions);
    }

    Vector3 CalculateNextRayPoint(Vector3 currentPoint, Vector3 rayDirection, float remainingLength)
    {
        var hit = Physics.SphereCast(currentPoint, _bulletStats.Radius, rayDirection, out var hitInfo, 
                                        remainingLength, PhysicsMasks.PlayerBulletTargetMask);
        if (hit)
            return hitInfo.point;
        else
            return currentPoint + rayDirection * remainingLength;
    }

    RaycastHit CalculateNextRayHit(Vector3 currentPoint, Vector3 rayDirection, float remainingLength)
    {
        var hit = Physics.SphereCast(currentPoint, _bulletStats.Radius, rayDirection, out var hitInfo, 
                                        remainingLength, PhysicsMasks.PlayerBulletTargetMask);
        if (hit)
            return hitInfo;
        else
            return default;
    }
}

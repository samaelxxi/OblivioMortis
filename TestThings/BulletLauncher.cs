using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletLauncher : MonoBehaviour
{
    [SerializeField] Bullet _bulletPrefab;
    [SerializeField] PlayerStats _playerStats;
    [SerializeField] RangedWeapon _rangedWeapon;
    float _time = 1000;


    void Awake()
    {
        _rangedWeapon.ActivateAbility(0); 
        _rangedWeapon.SetStats(_playerStats.RangedWeapon);
    }

    void Update()
    {
        if (_time > 0.5f)
            SpawnBullet();
        _time += Time.deltaTime;
    } 

    void SpawnBullet()
    {
        _time = 0;

        _rangedWeapon.SetEffect(BulletEffectType.TestBullet);
        _rangedWeapon.Use();
    }
}

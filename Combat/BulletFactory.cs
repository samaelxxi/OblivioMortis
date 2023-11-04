using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class BulletDictionary : SerializableDictionary<BulletType, Bullet> {}

[System.Serializable]
public class BulletEffectStats : SerializableDictionary<BulletEffectType, BulletStats> {}

public enum BulletType { Default, Ricochet, MultiAim, Shooter }


// [CreateAssetMenu(fileName = "BulletFactory", menuName = "ScriptableObjects/Combat/BulletFactory", order = 1)]
public class BulletFactory : DesignPatterns.Factory<Bullet>, Services.IRegistrable, Services.IInitializable
{
    [SerializeField] BulletDictionary _bulletPrefabs = null;
    [SerializeField] BulletEffectStats _bulletStats = null;


    public void Initialize()
    { // TODO  sort since this works only because order in dict is right
        base.Initialize(_bulletPrefabs.Values.ToList());
    }

    public Color GetBulletUIColor(BulletEffectType effectType)
    {
        return GetStatsByType(effectType).UIColor;
    }

    public BulletStats GetStatsByType(BulletEffectType effectType)
    {
        return _bulletStats[effectType];
    }

    public Bullet SpawnBullet(BulletType bulletType, BulletEffectType effectType, Vector3 position, bool isPlayerBullet = true)
    {
        var bullet = _pools[(int)bulletType].Get();
        bullet.Setup(GetStatsByType(effectType), isPlayerBullet);
        bullet.transform.SetPositionAndRotation(position, Quaternion.identity);
        return bullet;
    }

    public Bullet GetBullet(BulletType bulletType)
    {
        return _pools[(int)bulletType].Get();
    }
}

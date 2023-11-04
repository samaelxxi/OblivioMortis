using UnityEngine;
using DesignPatterns;
using DG.Tweening;
using System.Linq;
using System;
using UnityEngine.Rendering.Universal;

[Serializable]
public class DecalDictionary : SerializableDictionary<DecalType, DecalPoolable> {}

public enum DecalType { Blood, Oil, BulletHit, BruteHit }



public class DecalFactory : Factory<DecalPoolable>
{
    [SerializeField] DecalDictionary _decals = null;

    public void Init()
    {
        base.Initialize(_decals.Values.ToList(), "VFX");
    }

    public DecalProjector GetDecal(DecalType type)
    {
        var decal = _pools[(int)type].Get();
        return decal.DecalProjector;
    }

    public void SpawnDecal(DecalType type, Vector3 position, Quaternion rotation, float duration = 5)
    {
        var decal = _pools[(int)type].Get();
        SetupDecal(decal, duration);
        decal.transform.SetPositionAndRotation(position, rotation);
    }

    public void SpawnDecal(DecalType type, Vector3 position, float duration = 5)
    {
        var decal = _pools[(int)type].Get();
        SetupDecal(decal, duration);
        decal.transform.position = position;
    }

    public void SpawnBloodDecal(Vector3 position, Quaternion rotation, float duration = 5)
    {
        var decal = _pools[0].Get();
        decal.transform.SetPositionAndRotation(position, rotation);
        SetupBloodDecal(decal, duration);
    }

    public void SpawnOilDecal(Vector3 position, Quaternion rotation, float duration = 5)
    {
        var decal = _pools[1].Get();
        decal.transform.SetPositionAndRotation(position, rotation);
        SetupBloodDecal(decal, duration);
    }

    void SetupDecal(DecalPoolable decal, float duration)
    {
        var projector = decal.DecalProjector;
        projector.fadeFactor = 1;
        decal.InSeconds(duration, () => DOVirtual.Float(1, 0, 1, (float value) => projector.fadeFactor = value));
        decal.InSeconds(duration + 2, () => decal.Pool.Release(decal));
    }

    void SetupBloodDecal(DecalPoolable decal, float duration)
    {
        SetupDecal(decal, duration);
        decal.DecalProjector.size = new Vector3(UnityEngine.Random.Range(0.5f, 1.5f),
                                UnityEngine.Random.Range(0.5f, 1.5f), decal.DecalProjector.size.z);
    }
}

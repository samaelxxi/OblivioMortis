using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DesignPatterns;
using UnityEngine;
using UnityEngine.VFX;

[Serializable]
public class VFXDictionary : SerializableDictionary<VFXType, VFXPoolable> {}

public enum VFXType { BulletHit, MeleeSlash, BruteAttack }


// [CreateAssetMenu(fileName = "VFXFactory", menuName = "Factories/VFXFactory")]
public class VFXFactory : Factory<VFXPoolable>
{
    [SerializeField] VFXDictionary _vfx = null;


    public void Init()
    { // TODO  sort since this works only because order in dict is right
        base.Initialize(_vfx.Values.ToList(), "VFX");
    }

    public void PlayVFX(VFXType type, Vector3 position, Quaternion rotation, float duration = 3)
    {
        var obj = _pools[(int)type].Get();
        obj.transform.SetPositionAndRotation(position, rotation);
        // obj.VFX.Reinit();
        obj.VFX.Play();
        if (duration > 0)
            obj.InSeconds(duration, () => obj.Pool.Release(obj));
    }

    public VisualEffect GetVFX(VFXType type, float duration = 3)
    {
        var obj = _pools[(int)type].Get();
        obj.SetDuration(duration);
        return obj.VFX;
    }
}

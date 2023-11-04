using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DesignPatterns;
using UnityEngine;


[System.Serializable]
public class ParticleDictionary : SerializableDictionary<ParticleType, ParticlesPoolable> {}

public enum ParticleType { Blood, Oil, BulletExplosion, RobotExplosion, RobotSparkles }


// [CreateAssetMenu(fileName = "ParticlesFactory", menuName = "Factories/ParticlesFactory")]
public class ParticlesFactory : Factory<ParticlesPoolable>
{
    [SerializeField] ParticleDictionary _particles = null;

    public void Init()
    {
        base.Initialize(_particles.Values.ToList(), "VFX");
    }

    protected override ParticlesPoolable CreateObject(int idx)
    {
        var obj = base.CreateObject(idx);
        var m = obj.Particles.main;
        m.stopAction = ParticleSystemStopAction.Callback;
        return obj;
    }

    public ParticleSystem GetParticle(ParticleType type)
    {
        var obj = _pools[(int)type].Get();
        return obj.Particles;
    }
}

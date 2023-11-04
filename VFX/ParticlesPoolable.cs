using System.Collections;
using System.Collections.Generic;
using DesignPatterns;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ParticlesPoolable : Poolable<ParticlesPoolable>
{
    public ParticleSystem Particles;

    void OnParticleSystemStopped()
    {
        this.Pool.Release(this);
    }
}

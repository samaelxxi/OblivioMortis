using System.Collections;
using System.Collections.Generic;
using DesignPatterns;
using UnityEngine;
using UnityEngine.VFX;


public class GenericPoolable : Poolable<GenericPoolable>
{
    public Transform Transform;


    void Awake()
    {
        Transform = transform;
    }

    public void SetDuration(float duration)
    {
        if (duration > 0)
            this.InSeconds(duration, () => this.Pool.Release(this));
    }
}

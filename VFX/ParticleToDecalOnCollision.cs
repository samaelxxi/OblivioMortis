using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class ParticleToDecalOnCollision : MonoBehaviour
{
    [SerializeField] DecalType _decalType;

    ParticleSystem _particles;


    private List<ParticleCollisionEvent> collisionEvents = new();

    void Awake()
    {
        _particles = GetComponent<ParticleSystem>();
    }

    void OnParticleCollision(GameObject other)
    {
        _particles.GetCollisionEvents(other, collisionEvents);

        foreach(ParticleCollisionEvent e in collisionEvents)
        {
            if (_decalType == DecalType.Blood)
                ServiceLocator.Get<VFXManager>().SpawnBloodDecal(e.intersection, Quaternion.LookRotation(-e.normal, Vector3.up));
            else if (_decalType == DecalType.Oil)
                ServiceLocator.Get<VFXManager>().SpawnOilDecal(e.intersection, Quaternion.LookRotation(-e.normal, Vector3.up));
            // // might want to stretch decal based on velocity
            // // Debug.Log($"{Vector3.Dot(e.normal, e.velocity.normalized)}");  // the closer to -1, the less streched decal is
        }
    }
}

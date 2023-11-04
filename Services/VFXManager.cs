using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.VFX;


public class VFXManager : Services.SORegistrable, Services.IInitializable
{
    [SerializeField] DecalFactory _decalFactory;
    [SerializeField] ParticlesFactory _particlesFactory;
    [SerializeField] VFXFactory _vfxFactory;
    [SerializeField] StuffFactory _stuffFactory;


    public void Initialize()
    {
        _decalFactory.Init();
        _particlesFactory.Init();
        _vfxFactory.Init();
        _stuffFactory.Init();
    }

    public void PlayVFX(VFXType type, Vector3 position, Quaternion rotation, float duration = 3)
    {
        _vfxFactory.PlayVFX(type, position, rotation, duration);
    }

    public VisualEffect GetVFX(VFXType type, float duration = 3)
    {
        return _vfxFactory.GetVFX(type, duration);
    }

    public void PlayParticle(ParticleType type, Vector3 position, Quaternion rotation)
    {
        var particle = _particlesFactory.GetParticle(type);
        particle.transform.SetPositionAndRotation(position, rotation);
        particle.Play();
    }

    public ParticleSystem GetParticle(ParticleType type)
    {
        return _particlesFactory.GetParticle(type);
    }

    public void SpawnBloodDecal(Vector3 position, Quaternion rotation)
    {
        _decalFactory.SpawnBloodDecal(position, rotation);
    }

    public void SpawnOilDecal(Vector3 position, Quaternion rotation)
    {
        _decalFactory.SpawnOilDecal(position, rotation);
    }

    public void SpawnDecal(DecalType type, Vector3 position, Quaternion rotation, float duration = 5)
    {
        _decalFactory.SpawnDecal(type, position, rotation, duration);
    }

    public void SpawnDecal(DecalType type, Vector3 position, float duration = 5)
    {
        _decalFactory.SpawnDecal(type, position, duration);
    }

    public GameObject GetStuff(StuffType type, float duration = 30)
    {
        return _stuffFactory.GetStuff(type, duration);
    }
}

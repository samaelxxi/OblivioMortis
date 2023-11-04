using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;


public class CameraController : Services.MonoRegistrable
{
    [SerializeField] CameraNoiseShakes _cameraShakes;

    public Camera MainCamera { get; private set; }
    public CinemachineVirtualCamera VirtualCamera;

    public Vector2 ScreenSpaceCenter => new(MainCamera.pixelWidth / 2, MainCamera.pixelHeight / 2);
    public float Pitch => MainCamera.transform.rotation.eulerAngles.y;
    public Quaternion Rotation => MainCamera.transform.rotation;


    CinemachineBasicMultiChannelPerlin _noise;
    CinemachineImpulseSource _impulseSource;
    float _defaultZoom;
    

    void Awake()
    {
        MainCamera = GetComponentInChildren<Camera>();
        _impulseSource = GetComponentInChildren<CinemachineImpulseSource>();
        _impulseSource.m_ImpulseDefinition.m_ImpulseShape = CinemachineImpulseDefinition.ImpulseShapes.Custom;
        _noise = VirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _defaultZoom = VirtualCamera.m_Lens.FieldOfView;
        ServiceLocator.Register(this);
    }

    public void NoiseShake(NoiseShakeType type)
    {
        var shake = _cameraShakes.GetShake(type);
        _noise.m_NoiseProfile = shake.NoiseSettings;
        StartCoroutine(ShakeCoroutine(shake));
    }

    IEnumerator ShakeCoroutine(CameraNoiseShakes.Shake shake)
    {
        _noise.m_NoiseProfile = shake.NoiseSettings;
        _noise.m_AmplitudeGain = 1;
        _noise.m_FrequencyGain = 1;
        yield return new WaitForSeconds(shake.Time);
        DOVirtual.Float(1, 0, 0.2f, (x) => _noise.m_AmplitudeGain = x)
            .SetEase(Ease.InOutCubic).SetUpdate(true);
        DOVirtual.Float(1, 0, 0.2f, (x) => _noise.m_FrequencyGain = x).
            SetEase(Ease.InOutCubic).SetUpdate(true);
    }

    public void ImpulseShake(ImpulseShakeType type, Vector2 dir)
    {
        var shakeData = _cameraShakes.GetImpulseShake(type);
        _impulseSource.m_ImpulseDefinition.m_CustomImpulseShape = shakeData.Curve;
        _impulseSource.m_ImpulseDefinition.m_ImpulseDuration = shakeData.Time;
        _impulseSource.GenerateImpulseWithVelocity(new Vector3(dir.x, 0, dir.y).normalized * shakeData.Strength);
    }

    public void TempZoomIn(float zoom, float time)
    {
        StartCoroutine(TempZoomInCoroutine(zoom, time));
    }

    IEnumerator TempZoomInCoroutine(float zoom, float time)
    {
        float oldZoom = VirtualCamera.m_Lens.FieldOfView;
        float targetZoom = oldZoom - zoom;

        float t = 0;
        while (t < time/2)
        {
            t += Time.deltaTime;
            VirtualCamera.m_Lens.FieldOfView = Mathf.Lerp(_defaultZoom, targetZoom, t / (time/2));
            yield return null;
        }
        t = 0;
        while (t < time/2)
        {
            t += Time.deltaTime;
            VirtualCamera.m_Lens.FieldOfView = Mathf.Lerp(targetZoom, _defaultZoom, t / (time/2));
            yield return null;
        }
        VirtualCamera.m_Lens.FieldOfView = oldZoom;
    }

    public void AddZoom(float zoom)
    {
        DOVirtual.Float(_defaultZoom, _defaultZoom + zoom, 0.5f, (x) => VirtualCamera.m_Lens.FieldOfView = x)
            .SetEase(Ease.InOutQuad).SetUpdate(true);
    }

    public void ZoomToDefault()
    {
        DOVirtual.Float(VirtualCamera.m_Lens.FieldOfView, _defaultZoom, 0.5f, (x) => VirtualCamera.m_Lens.FieldOfView = x)
            .SetEase(Ease.InOutQuad).SetUpdate(true);
    }
}

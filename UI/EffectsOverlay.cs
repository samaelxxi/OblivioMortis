using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class EffectsOverlay : MonoBehaviour
{
    [SerializeField] Image _leftDamage;
    [SerializeField] Image _rightDamage;
    [SerializeField] Image _leftSouls;
    [SerializeField] Image _rightSouls;


    [SerializeField] Material _damageMaterial;
    [SerializeField] Material _soulsMaterial;


    bool _showingDamage = false;
    Coroutine _damageCoroutine;
    bool _showingSouls = false;
    Coroutine _soulsCoroutine;


    public void GetDamaged()
    {
        if (_showingDamage)
        {
            StopCoroutine(_damageCoroutine);
            _damageCoroutine = StartCoroutine(DamageCoroutine());
        }
        else
            _damageCoroutine = StartCoroutine(DamageCoroutine());
    }

    IEnumerator DamageCoroutine()
    {
        _showingDamage = true;
        _damageMaterial.EnableKeyword("_ENABLED");
        _damageMaterial.SetFloat("_Strength", 0.5f);
        yield return _damageMaterial.DOFloat(1, "_Strength", 0.1f).WaitForCompletion();
        yield return _damageMaterial.DOFloat(0.5f, "_Strength", 4).WaitForCompletion();
        _damageMaterial.DisableKeyword("_ENABLED");
        _showingDamage = false;
    }

    public void RestoreSouls()
    {
        if (_showingSouls)
        {
            StopCoroutine(_soulsCoroutine);
            _soulsCoroutine = StartCoroutine(SoulsCoroutine());
        }
        else
            _soulsCoroutine = StartCoroutine(SoulsCoroutine());
    }

    IEnumerator SoulsCoroutine()
    {
        _showingSouls = true;
        _soulsMaterial.EnableKeyword("_ENABLED");
        _soulsMaterial.SetFloat("_Strength", 0.5f);
        yield return _soulsMaterial.DOFloat(1, "_Strength", 0.1f).WaitForCompletion();
        yield return _soulsMaterial.DOFloat(0.5f, "_Strength", 4).WaitForCompletion();
        _soulsMaterial.DisableKeyword("_ENABLED");
        _showingSouls = false;
    }

    void OnDisable()
    {
        _damageMaterial.DisableKeyword("_ENABLED");
        _soulsMaterial.DisableKeyword("_ENABLED");
        _soulsMaterial.SetFloat("_Strength", 0.5f);
        _damageMaterial.SetFloat("_Strength", 0.5f);
    }
}

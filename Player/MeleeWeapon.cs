using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public enum MeleeWeaponState
{
    Idle, Preparing, Attacking, Resting
}

public class MeleeWeapon : MonoBehaviour
{
    [SerializeField] BoxCollider _attackCollider;
    [SerializeField] GameObject _scythe;

    public int Damage => _stats.Damage;
    public MeleeWeaponState State => _state;
    public event System.Action<Collider> OnHit;

    MeleeWeaponStats _stats;
    MeleeWeaponState _state = MeleeWeaponState.Idle;

    float _timeSinceLastAttack = Mathf.Infinity;
    float _attackTimer = 0;

    readonly List<Collider> _hitColliders = new();


    void Awake()
    {
        _attackCollider.gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (_hitColliders.Contains(other))  // trigger any collider only once per attack
            return;
        _hitColliders.Add(other);
        OnHit?.Invoke(other);
    }

    public void SetStats(MeleeWeaponStats stats)
    {
        _stats = stats;
    }

    void Update()
    {
        switch (_state)
        {
            case MeleeWeaponState.Idle:
                _timeSinceLastAttack += Time.deltaTime;
                break;
            case MeleeWeaponState.Preparing:
                _attackTimer += Time.deltaTime;
                if (_attackTimer >= _stats.AttackPreparationTime)
                    ActivateAttack();
                break;
            case MeleeWeaponState.Attacking:
                _attackTimer += Time.deltaTime;
                if (_attackTimer >= _stats.AttackDuration)
                    DeactivateAttack();
                break;
            case MeleeWeaponState.Resting:
                _attackTimer += Time.deltaTime;
                if (_attackTimer >= _stats.AttackRestTime)
                    FinishAttack();
                break;
        }
    }

    public void StartAttack()
    {
        _hitColliders.Clear();
        _scythe.SetActive(true);
        _attackTimer = 0;
        _state = MeleeWeaponState.Preparing;
        ServiceLocator.Get<VFXManager>().PlayVFX(VFXType.MeleeSlash, transform.position + transform.forward*3, transform.rotation);
    }

    public void CancelAttack()
    {
        _timeSinceLastAttack = 0;
        _state = MeleeWeaponState.Idle;
        _scythe.SetActive(false);
    }

    void ActivateAttack()
    {
        _attackTimer = 0;
        _attackCollider.gameObject.SetActive(true);
        #if UNITY_EDITOR
        _attackCollider.gameObject.transform.localPosition = new Vector3(0, 0, 0.5f + _stats.DamageAreaLength / 2);
        _attackCollider.gameObject.transform.localScale = new Vector3(_stats.DamageAreaWidth, 0.1f, _stats.DamageAreaLength);
        #endif
        _state = MeleeWeaponState.Attacking;
    }

    void DeactivateAttack()
    {
        _attackTimer = 0;
        _attackCollider.gameObject.SetActive(false);
        _state = MeleeWeaponState.Resting;
    }

    void FinishAttack()
    {
        _timeSinceLastAttack = 0;
        _state = MeleeWeaponState.Idle;
        _scythe.SetActive(false);
    }

    public bool CanCancelAttack()
    {
        return _state == MeleeWeaponState.Preparing || _state == MeleeWeaponState.Resting;
    }

    public bool CanAttack()
    {
        return _timeSinceLastAttack >= _stats.AttackCooldown;
    }
}

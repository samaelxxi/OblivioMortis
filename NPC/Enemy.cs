using System.Collections;
using System;
using UnityEngine;
using UnityEngine.AI;
using NodeCanvas.BehaviourTrees;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;
using NaughtyAttributes;
using DesignPatterns;

[SelectionBase, RequireComponent(typeof(NavMeshAgent)), 
    RequireComponent(typeof(BehaviourTreeOwner))]
public partial class Enemy : Poolable<Enemy>
{
    [SerializeField] protected EnemyStats _stats;
    [SerializeField] protected Animator _animator;
    [SerializeField] ParticleSystem _damagedSteam;  // Temp

    [SerializeField] bool _shouldPatrol = false;
    [SerializeField, ShowIf("_shouldPatrol")] List<GameObject> _patrolPoints = new();

    public EnemyStats Stats => _stats;
    public NavMeshAgent NavMeshAgent => _navMeshAgent;

    // need this for the behavior tree mostly
    public float Speed => Stats.Speed;
    public float RotationSpeed => Stats.RotationSpeed;
    public float AttackPreparationTime => Stats.AttackPreparationTime;
    public float AttackRestTime => Stats.AttackRestTime;
    public float AttackCooldown => Stats.AttackCooldown;
    public float AgroRadius => Stats.AgroRadius;
    public float ChaseRadius => Stats.ChaseRadius;
    public int Health => _health;
    public bool ShouldPatrol => _shouldPatrol;
    public List<GameObject> PatrolPoints => _patrolPoints;
    public bool DoesSensePlayer { get => _doesSensePlayer; 
        set 
        {
            if (_doesSensePlayer == value)
                return;
            Globals.AwareEnemiesCount += value ? 1 : -1;
            _doesSensePlayer = value;
            if (value)
                SensePlayer();
        }
    }

    public float TimeSinceLastAttack { get; protected set; } = 0;

    public bool IsAttacking { get; set; } = false;
    public bool IsDead => _isDead;

    public virtual int LowHealth => Stats.Health / 2;
    public virtual OblivioSounds AttackSound => OblivioSounds.RobotHit;
    public virtual OblivioSounds DamagedSound => OblivioSounds.RobotHit;
    public virtual EnemyType EnemyType { get; }

    int _health;
    NavMeshAgent _navMeshAgent;
    BehaviourTreeOwner  _behaviorTree;
    SkinnedMeshRenderer _meshRenderer;
    
    bool _isDead = false;
    bool _doesSensePlayer = false;


    public event Action OnDamageTaken;
    public event Action OnAttackInterrupted;
    public event Action OnStaggered;
    public event Action OnKnockbacked;
    public event Action<Enemy> OnDeath;


    // BT methods
    public virtual void DamageTakenEvent() { OnDamageTaken?.Invoke(); }
    public virtual void AttackPreparationEvent() { AttackPreparation(); }
    public virtual void AttackEvent() { Attack(); }
    public virtual void AttackRestEvent() { AttackRest(); }
    public virtual void AttackEndEvent() { AttackEnd(); }
    public virtual void AttackInterruptedEvent() { OnAttackInterrupted?.Invoke(); }



    protected virtual void Awake()
    {
        if (_shouldPatrol && _patrolPoints.Count < 2)
            Debug.LogError($"{name} on {transform.position} has not enough patrol points.");
        _health = _stats.Health;
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.speed = Stats.Speed;
        _navMeshAgent.angularSpeed = Stats.RotationSpeed * Mathf.Rad2Deg;
        _behaviorTree = GetComponent<BehaviourTreeOwner>();
        _meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        OnDamageTaken += () => JSAM.AudioManager.PlaySound(DamagedSound, transform.position);
    }

    public virtual void Update()
    {
        TimeSinceLastAttack += Time.deltaTime;

        _animator.SetBool(AnimationHashes.Moving, _navMeshAgent.velocity.magnitude > 0.1f);
        _animator.SetBool(AnimationHashes.Aware, DoesSensePlayer);
    }

    public virtual void SensePlayer()
    {
        DoesSensePlayer = true;
        TimeSinceLastAttack = AttackCooldown - 1;  // grace time 1 sec
    }

    protected virtual void AttackPreparation()
    {
        _animator.ResetTrigger(AnimationHashes.Attack);
        _animator.ResetTrigger(AnimationHashes.AttackRest);
        _animator.ResetTrigger(AnimationHashes.AttackEnd);
        _animator.SetTrigger(AnimationHashes.AttackPrep);
    }

    protected virtual void Attack()
    {
        _animator.SetTrigger(AnimationHashes.Attack);
    }

    protected virtual void AttackRest()
    {
        _animator.SetTrigger(AnimationHashes.AttackRest);
    }

    protected virtual void AttackEnd()
    {
        _animator.SetTrigger(AnimationHashes.AttackEnd);
        ResetTimeSinceLastAttack();
    }

    void ResetData()
    {   // TODO should refactor enemies into EnemyData class maybe?
        _animator.ResetTrigger(AnimationHashes.Attack);
        _animator.ResetTrigger(AnimationHashes.AttackRest);
        _animator.ResetTrigger(AnimationHashes.AttackEnd);
        _animator.ResetTrigger(AnimationHashes.AttackPrep);
        _animator.Play("Idle");
        DoesSensePlayer = false;
        IsAttacking = false;
        _health = _stats.Health;
        _damagedSteam.Stop();
    }

    public void Respawn()
    {
        _isDead = false;
        ResetData();
        gameObject.SetActive(true);
        _behaviorTree.RestartBehaviour();
    }

    public void Die()
    {
        OnDeath?.Invoke(this);
        _isDead = true;
        _behaviorTree.StopBehaviour();
        DoesSensePlayer = false;
        gameObject.SetActive(false);
        ServiceLocator.Get<VFXManager>().PlayParticle(ParticleType.RobotExplosion, transform.position + Vector3.up*1.5f, Quaternion.identity);
    }

    public void ResetTimeSinceLastAttack()
    {
        TimeSinceLastAttack = 0;
    }

    public void TakeDamage(int damage)
    {
        _health -= damage;
        OnDamageTaken?.Invoke();
        GlobalEvents.OnEnemyDamaged.Publish(this, damage);
        if (_health <= 0)
        {
            Die();
            return;
        }
        StartCoroutine(DamageFlash());

        if (_health < LowHealth)
            _damagedSteam.Play();
        // Debug.Log($"{name} took {damage} damage.");
    }

    IEnumerator DamageFlash()
    {
        var oldMat = _meshRenderer.material;
        _meshRenderer.material = Globals.DamageFlashMaterial;
        yield return CoroutineWaiters.WaitForEndOfFrame;
        _meshRenderer.material = oldMat;
    }

    public void GetMeleeHit(MeleeWeaponStats meleeWeapon, Vector3 dir)
    {
        ProcessHit((int)meleeWeapon.Damage, 
                    meleeWeapon.AttackKnockback, 
                    meleeWeapon.AttackStagger, dir);
        if (!_stats.MeleeInterruptionResistance && IsAttacking)
            OnAttackInterrupted?.Invoke();
    }

    public void GetBulletHit(Bullet bullet, Vector3 dir)
    {
        ProcessHit((int)bullet.Damage, bullet.Knockback, bullet.Stagger, dir);
        if (!_stats.RangedInterruptionResistance && IsAttacking)
            OnAttackInterrupted?.Invoke();
    }

    void ProcessHit(int damage, float knockback, float stagger, Vector3 dir)
    {
        TakeDamage(damage);
        ServiceLocator.Get<VFXManager>().PlayParticle(ParticleType.Oil, transform.position + Vector3.up, 
                                    Quaternion.LookRotation(dir.SetY(0)));
        ServiceLocator.Get<VFXManager>().PlayParticle(ParticleType.RobotSparkles, transform.position + Vector3.up, 
                            Quaternion.LookRotation(dir.SetY(0)));
        if (_isDead)
            return;
        var totalStagger = stagger - _stats.StaggerResistance;
        if (totalStagger > 0)
            GetStaggered(totalStagger);
        var totalKnockback = knockback - _stats.KnockbackResistance;
        if (totalKnockback > 0)
            GetKnockbacked(totalKnockback, dir);
    }

    void GetStaggered(float staggerValue)
    {
        OnStaggered?.Invoke();
        _behaviorTree.PauseBehaviour();
        StartCoroutine(PauseBehavior(staggerValue));
    }

    IEnumerator PauseBehavior(float time)
    {
        _behaviorTree.PauseBehaviour();
        yield return new WaitForSeconds(time);
        _behaviorTree.StartBehaviour();
    }

    void GetKnockbacked(float knockbackValue, Vector3 dir)
    {
        OnKnockbacked?.Invoke();
        StartCoroutine(Knockback(knockbackValue, dir));
    }

    IEnumerator Knockback(float knockbackValue, Vector3 dir)
    {
        _navMeshAgent.velocity = Vector3.zero;
        dir = dir.SetY(0).normalized;  // ?
        float time = 0;
        while (time < Globals.KnockbackTime)
        {
            float speed = MathUtils.TimeToVelocityOnCurve(Globals.KnockbackCurve, time,
                                        Time.deltaTime, Globals.KnockbackTime, knockbackValue);
            time += Time.deltaTime;
            if (_navMeshAgent.isOnNavMesh) // there crashes sometimes without this
                _navMeshAgent.Move(speed * Time.deltaTime * dir);
            yield return null;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (Stats == null)
            return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Stats.AgroRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, Stats.ChaseRadius);

        if (_shouldPatrol && _patrolPoints.Count > 1)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < _patrolPoints.Count; i++)
            {
                Gizmos.DrawWireSphere(_patrolPoints[i].transform.position, 0.5f);
                if (i < _patrolPoints.Count - 1)
                    Gizmos.DrawLine(_patrolPoints[i].transform.position, _patrolPoints[i + 1].transform.position);
            }
            Gizmos.DrawLine(_patrolPoints[^1].transform.position, _patrolPoints[0].transform.position);
        }
    }
}

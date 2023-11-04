using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.VFX;

public class Brute : Enemy
{
    [SerializeField] CapsuleCollider _attackCollider;

    public BruteStats BruteStats => _stats as BruteStats;

    public float StartAttackDistance => BruteStats.StartAttackDistance;
    public float LungeDistance => BruteStats.LungeDistance;
    public float LungeTime => BruteStats.LungeTime;
    public float LungeSpeed => BruteStats.LungeSpeed;
    public AnimationCurve LungeCurve => BruteStats.LungeCurve;
    public float AttackAOESize => BruteStats.AttackAOESize;
    public float JumpTime => BruteStats.JumpTime;
    public float JumpHeight => BruteStats.JumpHeight;
    public float JumpMaxDistance => BruteStats.JumpMaxDistance;
    public AnimationCurve JumpCurve => BruteStats.JumpCurve;
    public bool NextAttackIsJump => _nextAttackIsJump;
    public Transform Player { get; set; }

    public override OblivioSounds AttackSound => OblivioSounds.BruteAttack;
    // public override OblivioSounds DamagedSound => OblivioSounds.BruteDamaged;
    public override EnemyType EnemyType => EnemyType.Brute;

    KnockbackDamageZone _damageZone;

    bool _nextAttackIsJump = false;
    bool _nextAttackChoosen = true;


    protected override void Awake()
    {
        base.Awake();
        _damageZone = _attackCollider.GetComponent<KnockbackDamageZone>();
        _damageZone.SetDamage(BruteStats.AttackDamage);
        _damageZone.SetKnockback(BruteStats.AttackKnockback);
    }

    public override void Update()
    {
        base.Update();
        if (!_nextAttackChoosen && TimeSinceLastAttack > AttackCooldown / 2)
            ChooseNextAttack();

        // replan attack type if needed
        if (TimeSinceLastAttack > AttackCooldown + 1 && !_nextAttackIsJump && 
                Vector3.Distance(transform.position, Player.transform.position) > JumpMaxDistance)
            _nextAttackIsJump = true;  // seeking player for some time coz he's far - jump
        else if (TimeSinceLastAttack > (AttackCooldown - 0.2f) && _nextAttackIsJump &&
                Vector3.Distance(transform.position, Player.transform.position) < StartAttackDistance)
            _nextAttackIsJump = false;  // always lunge on close range
    }

    public override void SensePlayer()
    {
        base.SensePlayer();
        ChooseNextAttack();
    }

    protected override void AttackPreparation()
    {
        base.AttackPreparation();
        #if UNITY_EDITOR
        _attackCollider.gameObject.transform.localScale = new Vector3(AttackAOESize, 0.3f, AttackAOESize);
        _damageZone.SetDamage(BruteStats.AttackDamage);
        _damageZone.SetKnockback(BruteStats.AttackKnockback);
        #endif
    }

    protected override void Attack()
    {
        base.Attack();
    }

    protected override void AttackEnd()
    {
        base.AttackEnd();
        _nextAttackChoosen = false;
    }

    public void ActivateAttack()
    {
        _damageZone.gameObject.SetActive(true);  // could(?) die same frame so disable in global coroutine
        StaticCoroutine.StartInSec(() => _damageZone.gameObject.SetActive(false), 0.1f);
        ServiceLocator.Get<CameraController>().NoiseShake(NoiseShakeType.BruteAttack);
        ServiceLocator.Get<VFXManager>().SpawnDecal(DecalType.BruteHit, _damageZone.transform.position, 2);
        ServiceLocator.Get<VFXManager>().PlayVFX(VFXType.BruteAttack, _damageZone.transform.position, Quaternion.identity, 2);
    }

    public void MakeJump(Vector3 target)
    {
        NavMeshAgent.enabled = false;
        transform.DOJump(target, 4, 1, 0.9f).SetEase(Ease.InOutQuint).OnComplete(() => NavMeshAgent.enabled = true);
    }

    public void ChooseNextAttack()
    {
        if (Player == null)
            return;

        float dist = Vector3.Distance(transform.position, Player.transform.position);
        float jumpProb = AI.ResponseCurves.ComputeValue(dist, AI.CurveType.Logistic, 1, 1.25f, LungeDistance, JumpMaxDistance);
        float val = Random.value;
        // Debug.Log($"{dist} => {jumpProb} > {val} = {val < jumpProb}");
        _nextAttackIsJump = val < jumpProb;
        _nextAttackChoosen = true;
    }

    public bool ShouldSeekPlayer()
    {
        var dist = Vector3.Distance(transform.position, Player.transform.position);
        return ((_nextAttackIsJump || !_nextAttackChoosen) && dist > JumpMaxDistance) ||
                (!_nextAttackIsJump && dist > StartAttackDistance);
    }
}

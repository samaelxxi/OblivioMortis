// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Rendering.Universal;

// public partial class Enemy : DesignPatterns.Poolable<Enemy>
// {
//     [SerializeField] protected DecalProjector _decal;  // this is debug only


//     void RegisterDebug()
//     {
//         OnMove += DebugSeek;
//         OnStopMoving += DebugIdle;
//         OnAttackPreparation += DebugAttackPreparation;
//         OnAttackRest += DebugAttackRest;
//         OnAttack += DebugAttack;
//         OnAttackInterrupted += DebugAttackInterrupted;
//         OnAttackEnd += DebugAttackEnd;
//         OnStaggered += DebugStaggered;
//         OnKnockbacked += DebugKnockbacked;
//         OnDamageTaken += DebugDamaged;
//         _decal.enabled = true;
//     }

//     public void DebugIdle()
//     {
//         if (_animator != null)
//         _animator.SetBool("Moving", false);
//         // Debug.Log("idle");
//         _decal.material = ServiceLocator.Get<DebugDecals>().Default;
//     }

//     public void DebugSeek()
//     {
//         if (_animator != null)
//         _animator.SetBool("Moving", true);
//         // Debug.Log("seek");
//         // _decal.material = ServiceLocator.Get<DebugDecals>().Seek;
//     }

//     public void DebugAttackPreparation()
//     {
//         if (_animator != null)
//             _animator.SetTrigger("AttackPrep");
//         // Debug.Log("attack prep");
//         _decal.material = ServiceLocator.Get<DebugDecals>().AttackPrep;
//     }

//     public void DebugAttackRest()
//     {
//         // Debug.Log("attack rest");
//         if (_animator != null)
//             _animator.SetTrigger("AttackRest");
//         _decal.material = ServiceLocator.Get<DebugDecals>().AttackRest;
//     }

//     public void DebugAttack()
//     {
//         // Debug.Log("attack");
//         if (_animator != null)
//             _animator.SetTrigger("Attack");
        
//         _decal.material = ServiceLocator.Get<DebugDecals>().Attack;
//     }

//     public void DebugAttackInterrupted()
//     {
//         // Debug.Log("attack interrupted");
//         _decal.material = ServiceLocator.Get<DebugDecals>().Default;
//     }

//     public void DebugAttackEnd()
//     {
//         if (_animator != null)
//             _animator.SetTrigger("AttackEnd");
//         // Debug.Log("attack end");
//         _decal.material = ServiceLocator.Get<DebugDecals>().Default;
//     }

//     public void DebugStaggered()
//     {
//         // Debug.Log("staggered");
//         StartCoroutine(DebugDecalTemp(ServiceLocator.Get<DebugDecals>().Stagger, 0.5f));
//     }

//     public void DebugKnockbacked()
//     {
//         // Debug.Log("knockbacked");
//         StartCoroutine(DebugDecalTemp(ServiceLocator.Get<DebugDecals>().Knockback, 0.5f));
//     }

//     public void DebugDamaged()
//     {
//         // Debug.Log("damaged");
//         StartCoroutine(DebugDecalTemp(ServiceLocator.Get<DebugDecals>().Damaged, 0.5f));
//     }

//     IEnumerator DebugDecalTemp(Material material, float time)
//     {
//         _decal.material = material;
//         yield return new WaitForSeconds(time);
//         _decal.material = ServiceLocator.Get<DebugDecals>().Default;
//     }
// }

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class KnockbackDamageZone : DamageZone
{
    enum KnockbackDirType
    {
        FromCenter,
        FromObject
    }

    [SerializeField] float _knockback = 1;

    [SerializeField] KnockbackDirType _knockbackDirType = KnockbackDirType.FromCenter;
    [SerializeField, ShowIf("_knockbackDirType", KnockbackDirType.FromObject)] 
    Transform _knockbackDirObject;

    public void SetKnockback(float knockback)
    {
        _knockback = knockback;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Player>(out var player))
        {
            var dir = Vector3.zero;
            switch (_knockbackDirType)
            {
                case KnockbackDirType.FromCenter:
                    dir = (player.transform.position - transform.position).normalized;
                    break;
                case KnockbackDirType.FromObject:
                    dir = (player.transform.position - _knockbackDirObject.position).normalized;
                    break;
            }
            player.TakeDamageWithKnockback(_damageAmount, dir, _knockback);
        }
    }
}

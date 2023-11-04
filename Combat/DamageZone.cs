using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    [SerializeField] protected int _damageAmount = 1;


    public void SetDamage(int damage)
    {
        _damageAmount = damage;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Player>(out var player))
        {
            player.TakeDamage(_damageAmount);
        }
    }
}

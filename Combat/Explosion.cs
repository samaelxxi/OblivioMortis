using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Explosion : MonoBehaviour
{
    Bullet _bullet;

    public void Setup(Bullet bullet)
    {
        _bullet = bullet;
        // change damage sphere size(visual only)
        gameObject.transform.localScale = 2 * bullet.AreaOfEffect * Vector3.one;
    }

    public void Explode()
    {
        var hits = Physics.OverlapSphere(transform.position, 
                _bullet.AreaOfEffect, PhysicsMasks.DamageableMask);
        foreach (var hit in hits)
        {
            var explosionDir = (hit.transform.position - transform.position).normalized;
            if (hit.gameObject.TryGetComponent<Enemy>(out var enemy))
                enemy.GetBulletHit(_bullet, explosionDir);
            else if (hit.gameObject.TryGetComponent<Player>(out var player))
                player.TakeDamageWithKnockback(_bullet.Damage, explosionDir, _bullet.Knockback);
            // should affect levers?
        }
        ServiceLocator.Get<VFXManager>().PlayParticle(ParticleType.BulletExplosion, transform.position, Quaternion.identity);
        this.InSeconds(0.2f, () => Destroy(gameObject));
    }
}

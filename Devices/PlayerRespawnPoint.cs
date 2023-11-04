using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[SelectionBase]
public class PlayerRespawnPoint : InteractZone
{
    [SerializeField] Animator _animator;


    public void Deactivate()
    {
        _animator.Play("Deactivate");
    }

    public override void OnTriggerEnter(Collider other)
    {
        _player = other.GetComponent<Player>();
        if (_player.Health < _player.MaxHealth || 
            ServiceLocator.Get<PlayerSpawnService>().GetRespawnObject() != this)
        {
            SetActive(true);
            base.OnTriggerEnter(other);
        }
    }

    public void EnableRespawnPoint()
    {
        var oldRespawn = ServiceLocator.Get<PlayerSpawnService>().GetRespawnObject();
        if (oldRespawn != this)
        {
            _animator.Play("Activate");
            if (oldRespawn != null)
                oldRespawn.Deactivate();
        }

        ServiceLocator.Get<PlayerSpawnService>().SetRespawnObject(this);
        GlobalEvents.OnRespawnActivated.Publish();
        SetActive(false);
}

    public override void OnInteraction()
    {
        EnableRespawnPoint();
    }
}

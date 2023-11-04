

using UnityEngine;

public enum PlayerUpgradeType { Health, Souls, Ammo, Ability } 



public class PlayerUpgradeZone : InteractZone<PlayerUpgradeType>, ISaveable
{
    [SerializeField] GameEvent<PlayerUpgradeType> _onInteractionEvent;
    [SerializeField] Animator _animator;

    bool _isPicked = false;


    public override void OnTriggerEnter(Collider other)
    {
        if (_isPicked)
            return;
        base.OnTriggerEnter(other);
        _animator.SetTrigger("Aware");
    }

    public override void OnTriggerExit(Collider other)
    {
        if (_isPicked)
            return;
        base.OnTriggerExit(other);
        _animator.SetTrigger("Idle");
    }

    public override void OnInteraction()
    {
        _onInteractionEvent.Raise(_value);
        _isPicked = true;
        SetActive(false);
        _animator.Play("TakeUpgrade");
    }

    public string Save()
    {
        return _isPicked.ToString();
    }

    public void Load(string data)
    {
        _isPicked = bool.Parse(data);
        if (_isPicked)
        {
            SetActive(false);
            _animator.Play("TakeUpgrade", layer: -1, normalizedTime: 1);
        }
    }
}

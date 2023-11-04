using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class InteractZone : MonoBehaviour
{
    bool _isActive = true;

    bool _isSubscribed = false;

    protected Player _player;

    public virtual void OnTriggerEnter(Collider other)
    {
        _player = other.GetComponent<Player>();  // only player can trigger this
        if (!_isActive)
            return;

        ServiceLocator.Get<PlayerInputs>().OnInteractClicked += OnInteraction;
        ServiceLocator.Get<UIView>().EnableInteractBillboard(true);
        _isSubscribed = true;
    }

    public virtual void OnTriggerExit(Collider other)
    {
        _player = null;
        if (!_isActive)
            return;

        ServiceLocator.Get<PlayerInputs>().OnInteractClicked -= OnInteraction;
        ServiceLocator.Get<UIView>().EnableInteractBillboard(false);
        _isSubscribed = false;
    }

    public void SetActive(bool isActive)
    {
        _isActive = isActive;
        if (!_isActive && _isSubscribed)
        {
            ServiceLocator.Get<PlayerInputs>().OnInteractClicked -= OnInteraction;
            ServiceLocator.Get<UIView>().EnableInteractBillboard(false);
        }
    }

    public abstract void OnInteraction();
}

[RequireComponent(typeof(Collider))]
public abstract class InteractZone<T> : InteractZone
{
    [SerializeField] protected T _value;
}
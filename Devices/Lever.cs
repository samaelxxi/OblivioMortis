using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;


[SelectionBase]
public class Lever : InteractZone, ISaveable
{
    enum LeverType { Event, Callback }

    [SerializeField] ColliderObserver _attackCollider;
    [SerializeField] Animator _animator;

    [SerializeField] LeverType _leverType = LeverType.Event;
    [SerializeField] bool _twoWay = false;
    [SerializeField] bool _canBeAttacked = true;
    
    [SerializeField] GameEvent _onEnableEvent;
    [SerializeField] GameEvent _onDisableEvent;
    [SerializeField] UnityEvent _onEnableCallback;
    [SerializeField] UnityEvent _onDisableCallback;


    bool _isActivated = false;
    bool _changingState = false;


    void Awake()
    {
        if (_canBeAttacked)
        {
            _attackCollider.gameObject.SetActive(true);
            _attackCollider.OnTriggerEnterEvent += (c) => OnInteraction();  // TODO maybe add some check for player
            _attackCollider.OnBulletTriggerEnterEvent += (b) => OnInteraction();
        }
        else
            _attackCollider.gameObject.SetActive(false);
    }

    public override void OnInteraction()
    {
        if (_changingState)
            return;

        if (_isActivated && _twoWay)
            Deactivate();
        else
            Activate();
    }

    void Activate()
    {
        _isActivated = true;
        if (_leverType == LeverType.Event)
            _onEnableEvent.Raise();
        else
            _onEnableCallback.Invoke();
        this.InSeconds(0.5f, () => _changingState = false);
        _animator.Play("Activate");
        _changingState = true;
        if (!_twoWay)
            SetActive(false);
    }

    void Deactivate()
    {
        _isActivated = false;
        if (_leverType == LeverType.Event)
            _onDisableEvent.Raise();
        else
            _onDisableCallback.Invoke();
        this.InSeconds(0.5f, () => _changingState = false);
        _animator.Play("Activate");
        _changingState = true;
    }

    public string Save()
    {
        return _isActivated.ToString();
    }

    public void Load(string data)
    {
        _isActivated = bool.Parse(data);
    }
}

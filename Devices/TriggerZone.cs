using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;

[RequireComponent(typeof(Collider))]
public class TriggerZone : MonoBehaviour
{
    // enum TriggerType { Event, Callback }

    [SerializeField] TriggerType _triggerType;
    [SerializeField] GameEvent _onTriggerEvent;
    [SerializeField] UnityEvent _onTriggerCallback;


    void OnTriggerEnter(Collider other)
    {
        // if (other)
        // {
        switch (_triggerType)
        {
            case TriggerType.Event:
                _onTriggerEvent.Raise();
                break;
            case TriggerType.Callback:
                _onTriggerCallback?.Invoke();
                break;
        }
        // }
    }

    // void OnTriggerExit(Collider other)
    // {
        // if (other.CompareTag("Player"))
        // {
        //     // switch
        //     // ServiceLocator.Get<UIView>().EnableInteractBillboard(false);
        // }
    // }/
}
public enum TriggerType { Event, Callback }
public class TriggerZone<T> : MonoBehaviour
{
    [SerializeField] TriggerType _triggerType;

    [SerializeField, ShowIf("_triggerType", TriggerType.Event)]
    GameEvent<T> _onTriggerEvent;
    [SerializeField, ShowIf("_triggerType", TriggerType.Callback)]
    UnityEvent<T> _onTriggerCallback;

    [SerializeField] T _value;

    void OnTriggerEnter(Collider other)
    {
        switch (_triggerType)
        {
            case TriggerType.Event:
                _onTriggerEvent.Raise(_value);
                break;
            case TriggerType.Callback:
                _onTriggerCallback?.Invoke(_value);
                break;
        }
    }
}

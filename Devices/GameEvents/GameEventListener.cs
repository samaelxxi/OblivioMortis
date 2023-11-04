

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

enum ConditionType { AND, OR }
public class GameEventListener : MonoBehaviour
{
    [SerializeField] bool _isComplexCondition;

    [SerializeField] GameEvent Event;

    [SerializeField] ConditionType Condition;
    [SerializeField] List<GameEvent> Events;

    [SerializeField] UnityEvent Response;

    [SerializeField] bool _hasDelay;
    [SerializeField] float _delayTime;

    [SerializeField] bool _onlyOnce;


    Dictionary<GameEvent, bool> _eventStates;
    bool _isActivated;

    void OnEnable()
    {
        if (_isComplexCondition)
        {
            if (Events.Count == 0)
            {
                Debug.LogWarning($"No events assigned to {name} GameEventListener");
                return;
            }
            foreach (var ev in Events)
                ev.RegisterListener(this);
        }
        else
        {
            if (Event == null)
            {
                Debug.LogWarning($"No event assigned to {name} GameEventListener");
                return;
            }
            Event.RegisterListener(this);
        }

        if (_isComplexCondition && Condition == ConditionType.AND)
        {
            if (_eventStates != null && _eventStates.All(x => _eventStates.ContainsKey(x.Key)))
                return;

            _eventStates = new Dictionary<GameEvent, bool>();
            foreach (var ev in Events)
                _eventStates.Add(ev, false);
        }
    }

    void OnDisable()
    {
        if (_isComplexCondition)
            foreach (var ev in Events)
                ev.UnregisterListener(this);
        else if (Event != null)
            Event.UnregisterListener(this);
    }

    public void OnEventRaised()
    {
        if (_onlyOnce && _isActivated)
            return;

        if (!_isComplexCondition || Condition == ConditionType.OR)
            Respond();
        else
        {
            _eventStates[Event] = true;
            if (_eventStates.ContainsValue(false))
                return;
            Respond();
        }
    }

    void Respond()
    {
        if (_hasDelay)
            this.InSeconds(_delayTime, RespondInner);
        else
            RespondInner();
    }

    void RespondInner()
    {
        _isActivated = true;
        Response.Invoke();
    }
}

public class GameEventListener<T> : MonoBehaviour
{
    [SerializeField] GameEvent<T> Event;
    [SerializeField] UnityEvent<T> Response;

    [SerializeField] bool _onlyOnce;

    bool _isActivated;

    void OnEnable()
    {
        if (Event == null)
        {
            Debug.LogWarning($"No event assigned to {name} GameEventListener");
            return;
        }
        Event.RegisterListener(this);
    }

    void OnDisable()
    {
        if (Event != null)
            Event.UnregisterListener(this);
    }

    public void OnEventRaised(T value)
    {
        if (_onlyOnce && _isActivated)
            return;

        _isActivated = true;
        Response.Invoke(value);
    }
}
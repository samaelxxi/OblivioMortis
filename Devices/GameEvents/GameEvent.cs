using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using System;


[CreateAssetMenu(menuName = "Game Events/GameEvent")]
public class GameEvent : ScriptableObject
{
	public List<GameEventListener> Listeners = new();
    public List<Action> Actions = new();

    public void Raise()
    {
        for(int i = Listeners.Count - 1; i >= 0; i--)
            Listeners[i].OnEventRaised();
        for (int i = Actions.Count - 1; i >= 0; i--)
            Actions[i].Invoke();
    }

    public void RegisterListener(GameEventListener listener)
    { 
        Listeners.Add(listener);
    }
    public void UnregisterListener(GameEventListener listener)
    {
        Listeners.Remove(listener);
    }
    public void RegisterListener(Action action)
    {
        Actions.Add(action);
    }
    public void UnregisterListener(Action action)
    {
        Actions.Remove(action);
    }
}


public class GameEvent<T> : ScriptableObject
{
    public List<GameEventListener<T>> Listeners = new();
    public List<Action<T>> Actions = new();

    public void Raise(T value)
    {
        for (int i = Listeners.Count - 1; i >= 0; i--)
            Listeners[i].OnEventRaised(value);
        for (int i = Actions.Count - 1; i >= 0; i--)
            Actions[i].Invoke(value);
    }

    public void RegisterListener(GameEventListener<T> listener)
    {
        Listeners.Add(listener);
    }
    public void UnregisterListener(GameEventListener<T> listener)
    {
        Listeners.Remove(listener);
    }
    public void RegisterListener(Action<T> action)
    {
        Actions.Add(action);
    }
    public void UnregisterListener(Action<T> action)
    {
        Actions.Remove(action);
    }
}

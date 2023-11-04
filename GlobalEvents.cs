using System;


public static class GlobalEvents
{
    public static GlobalEvent OnRespawnActivated = new();
    public static GlobalEvent<Enemy, int> OnEnemyDamaged = new();

    public static GlobalEvent OnNoEnemiesAware = new();
    public static GlobalEvent OnAnyEnemyAware = new();
}

public class GlobalEvent
{
    event Action Action = delegate { };

    public void Publish()
    {
        Action?.Invoke();
    }

    public void Add(Action subscriber)
    {
        Action += subscriber;
    }

    public void Remove(Action subscriber)
    {
        Action -= subscriber;
    }
}

public class GlobalEvent<T>
{
    event Action<T> Action;

    public void Publish(T param)
    {
        Action?.Invoke(param);
    }

    public void Add(Action<T> subscriber)
    {
        Action += subscriber;
    }

    public void Remove(Action<T> subscriber)
    {
        Action -= subscriber;
    }
}

public class GlobalEvent<S, T>
{
    event Action<S, T> Action;

    public void Publish(S param1, T param2)
    {
        Action?.Invoke(param1, param2);
    }

    public void Add(Action<S, T> subscriber)
    {
        Action += subscriber;
    }

    public void Remove(Action<S, T> subscriber)
    {
        Action -= subscriber;
    }
}
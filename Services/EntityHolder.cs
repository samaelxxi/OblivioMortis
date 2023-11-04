using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Attributes.AutoRegisteredService]
public class EntityHolder : Services.IRegistrable
{
    Dictionary<string, object> _entities = new();


    public void Register<T>(T entity)
    {
        _entities.Add(typeof(T).Name, entity);
    }

    public void Register<T>(string name, T entity)
    {
        _entities.Add(name, entity);
    }

    public T GetEntity<T>()
    {
        return (T)_entities[typeof(T).Name];
    }
}

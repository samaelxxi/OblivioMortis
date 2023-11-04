using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DesignPatterns;
using UnityEngine;



[System.Serializable]
public class StuffDictionary : SerializableDictionary<StuffType, GenericPoolable> { }

public enum StuffType { BulletShell }


[CreateAssetMenu(fileName = "StuffFactory", menuName = "Factories/StuffFactory")]
public class StuffFactory : Factory<GenericPoolable>
{
    [SerializeField] StuffDictionary _stuff = null;


    public void Init()
    {
        base.Initialize(_stuff.Values.ToList(), "VFX");
    }

    public GameObject GetStuff(StuffType type, float duration = 3)
    {
        var obj = _pools[(int)type].Get();
        obj.SetDuration(duration);
        return obj.gameObject;
    }

    public void SpawnStuff(StuffType type, Vector3 position, Quaternion rotation, float duration = 3)
    {
        var obj = _pools[(int)type].Get();
        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetDuration(duration);
    }
}

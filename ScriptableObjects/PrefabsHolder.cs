using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [CreateAssetMenu(fileName = "PrefabsHolder", menuName = "ScriptableObjects/PrefabsHolder", order = 1)]
public class PrefabsHolder : Services.SORegistrable
{
    [SerializeField] List<GameObject> _prefabs = new();

    public GameObject GetPrefabByName(string name)
    {
        return _prefabs.Find(p => p.name == name);
    }

    public T GetNewInstance<T>() where T : MonoBehaviour
    {
        return Instantiate(GetPrefabByName(typeof(T).Name)).GetComponent<T>();
    }
}

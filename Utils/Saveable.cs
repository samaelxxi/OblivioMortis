using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEditor;

public interface ISaveable
{
    string Save();
    void Load(string data);
}


// limited to saving only this object with no more than one saveable component of each type
public class Saveable : MonoBehaviour
{
    [SerializeField, HideInInspector] string _id;


#if UNITY_EDITOR
    void OnValidate()
    {
        if (string.IsNullOrEmpty(_id) && UnityEditor.PrefabUtility.IsPartOfPrefabInstance(this.gameObject))
        {
            UpdateId();
        }
    }

    public void ResetID()
    {
        _id = null;
        OnValidate();
    }

    public void UpdateId()
    {
        UnityEditor.Undo.RecordObject(this, "Updated Object Scene ID");
        _id = System.Guid.NewGuid().ToString();
    }
#endif

    public string GetID()
    {
        return _id;
    }

    public string Save()
    {
        var saveables = GetComponents<ISaveable>();
        Dictionary<string, string> nameToData = new();

        foreach (var saveable in saveables)
        {
            nameToData.Add(saveable.GetType().Name, saveable.Save());
        }

        var serializedData = JsonConvert.SerializeObject(nameToData);

        return serializedData;
    }

    public void Load(string data)
    {
        var saveables = GetComponents<ISaveable>();
        var nameToData = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);

        int restoredCount = 0;
        foreach (var saveable in saveables)
        {
            if (nameToData.ContainsKey(saveable.GetType().Name))
            {
                saveable.Load(nameToData[saveable.GetType().Name]);
                restoredCount++;
            }
            else
                Debug.LogError($"Saveable component {saveable.GetType().Name} not found in save data");
        }

        if (restoredCount != nameToData.Count)
            Debug.LogError($"Saveable components count mismatch: {restoredCount} restored, {nameToData.Count} in save data");
    }
}

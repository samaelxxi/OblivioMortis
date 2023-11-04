using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;


[Attributes.AutoRegisteredService]
public class SceneSaver : Services.IRegistrable
{
    Dictionary<string, Dictionary<string, string>> _sceneData = new();

    public void SaveScene(Scene scene)
    {
        List<Saveable> saveables = GetSaveablesInScene(scene);

        Dictionary<string, string> saveableData = new();
        foreach (var saveable in saveables)
            saveableData.Add(saveable.GetID(), saveable.Save());

        Debug.Log("Saved " + saveableData.Count + " saveables");
        _sceneData[scene.name] = saveableData;
    }

    public void LoadScene(Scene scene)
    {
        if (!_sceneData.ContainsKey(scene.name))
            return;

        Dictionary<string, Saveable> idToSaveable = GetSaveablesInSceneAsDict(scene);

        Dictionary<string, string> saveableData = _sceneData[scene.name];
        foreach (var saveable in saveableData)
        {
            // Debug.Log($"Loading {saveable.Key} {saveable.Value} {idToSaveable[saveable.Key].name}");
            if (idToSaveable.ContainsKey(saveable.Key))
                idToSaveable[saveable.Key].Load(saveable.Value);
        }

        // Debug.Log($"Loaded {saveableData.Count} saveables of {idToSaveable.Count} in scene {scene.name}");
    }

    public void ClearSaveData()
    {
        _sceneData.Clear();
    }

    List<Saveable> GetSaveablesInScene(Scene scene)
    {
        var rootObjects = scene.GetRootGameObjects();
        List<Saveable> saveables = new();

        foreach (var rootObject in rootObjects)
            saveables.AddRange(rootObject.GetComponentsInChildren<Saveable>());

        return saveables;
    }

    Dictionary<string, Saveable> GetSaveablesInSceneAsDict(Scene scene)
    {
        Dictionary<string, Saveable> idToSaveable = new();

        var rootObjects = scene.GetRootGameObjects();
        foreach (var rootObject in rootObjects)
        {
            var saveables = rootObject.GetComponentsInChildren<Saveable>();
            foreach (var saveable in saveables)
                idToSaveable.Add(saveable.GetID(), saveable);
        }

        return idToSaveable;
    }
}

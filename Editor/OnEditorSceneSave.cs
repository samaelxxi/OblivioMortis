using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.EditorCoroutines.Editor;

[UnityEditor.InitializeOnLoad]
static class OnEditorSceneSave
{
    static OnEditorSceneSave()
    {
        UnityEditor.SceneManagement.EditorSceneManager.sceneSaving += OnSceneSaved;
    }

    static void OnSceneSaved(Scene scene, string path)
    {
        var rootObjects = scene.GetRootGameObjects();
        List<Saveable> saveables = new();


        foreach (var rootObject in rootObjects)
            saveables.AddRange(rootObject.GetComponentsInChildren<Saveable>());

        HashSet<string> ids = new();
        bool _anyDuplicates = false;
        foreach (var saveable in saveables)
        {
            if (ids.Contains(saveable.GetID()))
                _anyDuplicates = true;

            while (ids.Contains(saveable.GetID()))
                saveable.UpdateId();

            ids.Add(saveable.GetID());
        }
        if (_anyDuplicates)
        {
            Debug.LogWarning("Duplicate IDs found in scene " + scene.name + "! Rewriting...");
            EditorCoroutineUtility.StartCoroutineOwnerless(Resave(scene));
            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);  // doesn't work...
        }
    }

    static IEnumerator Resave(Scene scene)
    {
        yield return new EditorWaitForSeconds(0.1f);
        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);
    }
}

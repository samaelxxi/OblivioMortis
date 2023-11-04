using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public static class ShowFallZones
{
    static bool _showInEditor = false;
    static public bool ShowInEditor => _showInEditor;

    [MenuItem("OblivioMortis/ShowFallZones", false, 1)]
    static void CreateNewAsset()
    {
        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        var rootObjects = scene.GetRootGameObjects();
        List<FallZone> fallZones = new();

        foreach (var rootObject in rootObjects)
            fallZones.AddRange(rootObject.GetComponentsInChildren<FallZone>());

        _showInEditor = !_showInEditor;
        foreach (var fallZone in fallZones)
            fallZone.SetShowInEditor(_showInEditor);
    }
}
#endif
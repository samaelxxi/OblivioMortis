using UnityEditor;
using UnityEngine.SceneManagement;


public class ResetSaveables
{

    [MenuItem("Tools/Reset Saveables")]
    static void Reset()
    {
        var rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (var rootObject in rootObjects)
        {
            var saveables = rootObject.GetComponentsInChildren<Saveable>();
            foreach (var saveable in saveables)
                saveable.ResetID();
        }
    }
}

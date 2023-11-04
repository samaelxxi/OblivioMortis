using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public static class GameInitializer
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {
        ServiceLocator.RegisterSO<BulletFactory>("Services/BulletFactory");
        // ServiceLocator.RegisterSO<DebugDecals>("Services/DebugDecals");
        ServiceLocator.RegisterSO<PrefabsHolder>("Services/PrefabsHolder");
        ServiceLocator.RegisterSO<VFXManager>("Services/VFXManager");
        ServiceLocator.RegisterSO<GlobalSO>("Services/GlobalSO");
        ServiceLocator.RegisterSO<EditorUserSettings>("Services/EditorUserSettings");
        ServiceLocator.RegisterSO<EnemyManager>("Services/EnemyManager");


        var objs = UnityEngine.Object.FindObjectsByType(typeof(Camera), FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (objs.Length > 0) return;
        Game.Instantiate(Resources.Load<Game>("Game"));
        Globals.IsGameInitialized = true;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void InitializeAfterSceneLoad()
    {
        Game.Instantiate(Resources.Load<GameObject>("AudioManager"));
    }
}

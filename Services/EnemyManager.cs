using System.Collections;
using System.Collections.Generic;
using DesignPatterns;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;


public enum EnemyType { Dasher, Shooter, Brute }


public class EnemyManager : Services.SORegistrable, Services.IInitializable
{
    [SerializeField] EnemyFactory _enemyFactory;

    struct EnemySaveableData
    {
        public Enemy Enemy;
        public Vector3 Position;
        public Quaternion Rotation;
    }

    List<EnemySaveableData> _sceneEnemies = new();


    public void Initialize()
    {
        SceneManager.sceneLoaded += (scene, mode) => OnSceneLoaded();
        _enemyFactory.Init();
    }

    public void OnSceneLoaded()
    {
        _sceneEnemies.Clear();
        var enemies = GameObject.FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach (var enemy in enemies)
        {
            _sceneEnemies.Add(new EnemySaveableData
            {
                Enemy = enemy,
                Position = enemy.transform.position,
                Rotation = enemy.transform.rotation
            });
        }
    }

    public void RespawnEnemies()
    {
        foreach (var enemyData in _sceneEnemies)
        {
            var enemy = enemyData.Enemy;
            enemy.transform.SetPositionAndRotation(enemyData.Position, enemyData.Rotation);
            enemy.Respawn();
        }
    }

    public Enemy SpawnEnemy(EnemyType enemyType, Vector3 position, Quaternion rotation)
    {
        return _enemyFactory.SpawnEnemy(enemyType, position, rotation);
    }
}

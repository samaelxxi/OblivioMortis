using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DesignPatterns;
using UnityEngine;


[System.Serializable]
public class EnemyDictionary : SerializableDictionary<EnemyType, Enemy> {}


public class EnemyFactory : Factory<Enemy>
{
    [SerializeField] EnemyDictionary _enemyPrefabs = new();


    public void Init()
    { // TODO  sort since this works only because order in dict is right
        base.Initialize(_enemyPrefabs.Values.ToList());
    }

    public Enemy SpawnEnemy(EnemyType enemyType, Vector3 position, Quaternion rotation)
    {
        var enemy = _pools[(int)enemyType].Get();
        if (enemy.IsDead)  // reset stats 
            enemy.Respawn();
        else  // poolable enemies should be released
            enemy.OnDeath += (enemy) => enemy.Release();
        enemy.NavMeshAgent.Warp(position);
        enemy.transform.rotation = rotation;
        return enemy;
    }
}

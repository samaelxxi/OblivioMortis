using System.Collections;
using System.Collections.Generic;
using ParadoxNotion;
using UnityEngine;
using UnityEngine.Events;

public class Arena : MonoBehaviour, ISaveable
{
    [SerializeField] List<Door> _doors = new();
    [SerializeField] List<Transform> _spawnPoints = new();
    [SerializeField] List<EnemyType> _enemyList = new();
    [SerializeField, Min(1)] int _maxEnemies = 3;
    [SerializeField] float _spawnDelay = 1f;
    [SerializeField, Min(1)] int _firstSpawnCount = 1;

    [SerializeField, Space(5)] UnityEvent _onArenaStarted = new();
    [SerializeField] UnityEvent _onArenaFailed = new();
    [SerializeField] UnityEvent _onArenaCompleted = new();


    bool _isCompleted = false;
    bool _isFailed = false;
    bool _isActive = false;

    List<Enemy> _aliveEnemies = new();
    int _enemiesSpawned = 0;
    int _enemiesAlive = 0;
    float _spawnTimer = 0f;
    Coroutine _spawnCoroutine = null;

    List<int> _spawnIndexes = new();
    int _nextSpawnIndex = 0;

    public void StartArena()
    {
        // Debug.Log($"StartArena {_isCompleted} {_isActive}");
        if (_isCompleted || _isActive)
            return;

        Globals.Player.OnDeath += FailArena;

        _isActive = true;
        _isFailed = false;
        _onArenaStarted.Invoke();

        _spawnCoroutine = StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        _doors.ForEach(d => d.CloseDoor());
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < _firstSpawnCount; i++)
            SpawnNextEnemy();
        
        while (_enemiesSpawned < _enemyList.Count)
        {
            _spawnTimer -= Time.deltaTime;
            if (_spawnTimer <= 0 && _enemiesAlive < _maxEnemies)
                SpawnNextEnemy();
            yield return null;
        }
    }

    void SpawnNextEnemy()
    {
        if (_enemiesSpawned >= _enemyList.Count || _enemiesAlive >= _maxEnemies)
            return;

        var enemyType = _enemyList[_enemiesSpawned];
        var spawnPoint = _spawnPoints[GetNextSpawnIndex()];
        var enemy = ServiceLocator.Get<EnemyManager>().SpawnEnemy(enemyType, 
                                        spawnPoint.position, spawnPoint.rotation);
        enemy.OnDeath += OnEnemyDeath;
        enemy.DoesSensePlayer = true;
        _aliveEnemies.Add(enemy);
        _enemiesSpawned++;
        _enemiesAlive++;
        _spawnTimer = _spawnDelay;
    }

    int GetNextSpawnIndex()
    {
        if (_spawnIndexes.Count == 0)
        {
            for (int i = 0; i < _spawnPoints.Count; i++)
                _spawnIndexes.Add(i);
            _spawnIndexes.Shuffle();
        }
        int index = _spawnIndexes[_nextSpawnIndex];
        _nextSpawnIndex++;
        if (_nextSpawnIndex >= _spawnIndexes.Count)
        {
            _nextSpawnIndex = 0;
            _spawnIndexes.Shuffle();
        }
        return index;
    }

    void OnEnemyDeath(Enemy enemy)
    {
        enemy.OnDeath -= OnEnemyDeath;
        _enemiesAlive--;
        _aliveEnemies.Remove(enemy);
        if (_enemiesAlive == 0 && _enemiesSpawned >= _enemyList.Count && !_isFailed)
            CompleteArena();
    }

    void FailArena()
    {
        Globals.Player.OnDeath -= FailArena;
        _isFailed = true;
        StopCoroutine(_spawnCoroutine);
        this.InSeconds(2, delegate 
        {
            Debug.Log("FailArena");
            while (_aliveEnemies.Count > 0)
                _aliveEnemies[0].Die();
            _aliveEnemies.Clear();
            _enemiesSpawned = 0;
            _enemiesAlive = 0;
            _isActive = false;
            _onArenaFailed.Invoke();
            _doors.ForEach(d => d.OpenDoor());
        });
    }

    void CompleteArena()
    {
        Debug.Log("CompleteArena");
        Globals.Player.OnDeath -= FailArena;
        _isCompleted = true;
        _isActive = false;
        _onArenaCompleted.Invoke();
        _doors.ForEach(d => d.OpenDoor());
    }

    public void Load(string data)
    {
        _isCompleted = bool.Parse(data);
    }

    public string Save()
    {
        return _isCompleted.ToString();
    }
}

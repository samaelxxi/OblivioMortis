using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawner : MonoBehaviour
{
    [SerializeField] GameObject _prefab;
    [SerializeField] GameObject _currentTarget;
    [SerializeField] float _respawnTime = 0;

    float _timer = 0;

    // Update is called once per frame
    void Update()
    {
        if (_currentTarget.GetComponent<Enemy>().IsDead)
        {
            _timer += Time.deltaTime;
            if (_timer >= _respawnTime)
            {
                _timer = 0;
                var enemy = _currentTarget.GetComponent<Enemy>();
                enemy.Respawn();
                enemy.gameObject.transform.SetPositionAndRotation(transform.position, transform.rotation);
                // _currentTarget = Instantiate(_prefab, transform.position, transform.rotation);
            }
        }
    }
}

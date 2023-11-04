using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;


namespace DesignPatterns
{
    public abstract class Factory<T> : ScriptableObject where T : Poolable<T>
    {
        protected Scene _poolScene;

        List<T> _prefabs = new();
        protected List<IObjectPool<T>> _pools;


        protected void Initialize(List<T> prefabs, string sceneName = "")
        {
            if (_pools != null)
                return;
            if (string.IsNullOrEmpty(sceneName))
                sceneName = name;

            _prefabs = prefabs;
            _pools = new();

            for (int i = 0; i < prefabs.Count; i++)
            {
                int j = i; // closure
                var pool = new ObjectPool<T>(() => CreateObject(j), GetObject, 
                                            ReleaseObject, DestroyObject);
                _pools.Add(pool);
            }

            _poolScene = SceneManager.GetSceneByName(sceneName);
            if (!_poolScene.IsValid())
                _poolScene = SceneManager.CreateScene(sceneName);
        }

        protected virtual T CreateObject(int idx)
        {
            var obj = Instantiate(_prefabs[idx]);
            SceneManager.MoveGameObjectToScene(obj.gameObject, _poolScene);
            obj.gameObject.SetActive(false);
            obj.Pool = _pools[idx];
            return obj;
        }

        protected virtual void GetObject(T obj)
        {
            obj.gameObject.SetActive(true);
        }

        protected virtual void ReleaseObject(T obj)
        {
            obj.gameObject.SetActive(false);
        }

        protected virtual void DestroyObject(T obj)
        {
            Destroy(obj.gameObject);
        }
    }
}

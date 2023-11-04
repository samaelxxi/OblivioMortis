using UnityEngine;
using UnityEngine.Pool;


namespace DesignPatterns
{
    public abstract class Poolable<T> : MonoBehaviour where T : Component
    {
        public IObjectPool<T> Pool { get; set; }

        protected virtual void CleanOnRelease() {}

        public virtual void Release()
        {
            CleanOnRelease();
            Pool.Release(this as T);
        }
    }
}
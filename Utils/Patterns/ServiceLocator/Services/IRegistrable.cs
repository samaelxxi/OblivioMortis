using UnityEngine;
using System;

namespace Services
{
    public interface IRegistrable
    {
    }

    public interface IInitializable
    {
        void Initialize();
    }

    public abstract class MonoRegistrable : MonoBehaviour, IRegistrable
    {
        protected void Reset()
        {
            name = GetType().Name;
        }
    }

    public abstract class SORegistrable : ScriptableObject, IRegistrable
    {
    }
}

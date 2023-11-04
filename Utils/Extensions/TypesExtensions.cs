using System;
using UnityEngine;

namespace Extensions
{
    public static class TypesExtensions
    {
        public static bool IsMonoBehaviour(this Type t)
        {
            return typeof(MonoBehaviour).IsAssignableFrom(t);
        }

        public static bool IsScriptableObject(this Type t)
        {
            return typeof(ScriptableObject).IsAssignableFrom(t);
        }
    }
}

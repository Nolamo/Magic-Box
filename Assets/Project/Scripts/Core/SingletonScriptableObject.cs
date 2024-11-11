using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// generic singleton scriptable object
namespace NolanCore
{
    public abstract class SingletonScriptableObject<T> : ScriptableObject where T : SingletonScriptableObject<T>
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    T[] assets = Resources.LoadAll<T>("");
                    if (assets == null || assets.Length < 1)
                    {
                        throw new System.Exception("Could not find any singleton object instances in resources.");
                    }
                    else if (assets.Length > 1)
                    {
                        Debug.LogWarning("Multiple instances of singleton scriptable object found in resources.");
                    }
                    instance = assets[0];
                }
                return instance;
            }
        }
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignPatterns.Singleton
{
    public abstract class Singleton<T> : MonoBehaviour
    {
        [SerializeField] protected bool dontDestroyOnLoad = false;
        public static T Instance;

        public virtual void Awake ()
        {
            if (Instance == null)
            {
                Instance = GetInstance();
                if (dontDestroyOnLoad)
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public virtual void OnDestroy()
        {
            if (Instance as Singleton<T> == this)
            {
                Instance = default;
            }
        }

        public abstract T GetInstance();
    }
}

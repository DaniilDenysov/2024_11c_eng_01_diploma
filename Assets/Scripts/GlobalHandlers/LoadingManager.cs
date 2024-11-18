using DesignPatterns.Singleton;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Managers
{
    public class LoadingManager : Singleton<LoadingManager>
    {
        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private UnityEvent onLoadEnd,onLoadStart; 

        public override LoadingManager GetInstance()
        {
            return this;
        }


        //change to animation handler
        public void StartLoading ()
        {
            onLoadStart?.Invoke();
        }

        public void EndLoading ()
        {
            onLoadEnd?.Invoke();
        }

        public bool IsLoading()
        {
            return loadingScreen.activeInHierarchy;
        }
    }
}

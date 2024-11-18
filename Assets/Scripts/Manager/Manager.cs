using Zenject;
using UnityEngine;
using CustomTools;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Managers
{
    public abstract class Manager : MonoInstaller
    {
        public abstract override void InstallBindings();
    }
}

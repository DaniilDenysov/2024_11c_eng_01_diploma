using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace ShootingSystem.Server
{
    public class ServerShootingSystemInstaller : MonoInstaller
    {
        [SerializeField] private ServerShootingHandler shootingHandler;


        public override void InstallBindings()
        {
            Container.Bind<ServerShootingHandler>().To<ServerShootingHandler>().FromInstance(shootingHandler).AsSingle();
        }
    }
}

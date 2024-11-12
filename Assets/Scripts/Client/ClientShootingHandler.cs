using Mirror;
using ShootingSystem.Server;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace ShootingSystem.Client
{
    //controller
    public class ClientShootingHandler : NetworkBehaviour
    {
        [SerializeField] private Transform shootingPoint;
        [SerializeField] private ServerShootingHandler serverShootingHandler;

        [Inject]
        public void Construct (ServerShootingHandler serverShootingHandler)
        {
            this.serverShootingHandler = serverShootingHandler;
        }
        #region Commands
        #endregion

        #region Callbacks

        #endregion

        #region InputHandling
        public void OnShoot ()
        {
            //add handling for pressings
            Debug.Log("Client shooted");
            serverShootingHandler.CmdShoot(shootingPoint.position,transform.forward);
        }
        #endregion
    }
}

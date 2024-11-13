using Mirror;
using ShootingSystem.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace ShootingSystem.Client
{
    // Controller
    public class ClientShootingHandler : NetworkBehaviour
    {
        [SerializeField] private Transform shootingPoint;
        private DefaultInput inputActions;
        [SerializeField] private Camera mainCamera;

        private void Start()
        {
            Construct();
        }

        public void Construct()
        {
            inputActions = new DefaultInput();
            inputActions.Enable();
            inputActions.Player.Shoot.performed += OnShoot;
        }

        #region Commands
        [Command]
        public void CmdShoot(Vector3 position, Vector3 direction)
        {
            ServerShootingHandler.Instance.CmdShoot(position, direction);
        }
        #endregion

        #region InputHandling
        private void OnShoot(InputAction.CallbackContext obj)
        {
            if (mainCamera == null)
            {
                Debug.LogWarning("Main camera not found. Unable to determine shooting direction.");
                return;
            }
            Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCamera.nearClipPlane));
            Vector3 shootingDirection = (mouseWorldPosition - shootingPoint.position).normalized;
            Debug.Log(shootingDirection);
            CmdShoot(shootingPoint.position, shootingDirection);
        }
        #endregion
    }
}

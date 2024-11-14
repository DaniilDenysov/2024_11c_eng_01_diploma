using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace ShootingSystem.Client
{
    public class ClientShootingHandler : NetworkBehaviour
    {
        [SerializeField] private WeaponSO weapon;
        [SerializeField] private Transform shootingPoint;
        private DefaultInput inputActions;
        [SerializeField] private Camera mainCamera;

        [SerializeField] private TMP_Text magDisplay;

        private float lastTimeFired;
        [SyncVar(hook = nameof(UpdateMagDisplay))] private float currentBullets;
        [SyncVar] private bool isFiring;

        private void Start()
        {
            Construct();
            if (isOwned)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        [Server]
        private void Update()
        {
            if (!isFiring) return;
            if (lastTimeFired + weapon.FireRate >= Time.time) return;
            if (currentBullets <= 0) return;
            Shoot();
        }

        public void Construct()
        {
            inputActions = new DefaultInput();
            inputActions.Enable();
            inputActions.Player.Shoot.started += OnShootInput;
            inputActions.Player.Shoot.performed += OnShootInput;
            inputActions.Player.Shoot.canceled += OnShootInput;
            lastTimeFired = -weapon.FireRate;
            currentBullets = weapon.Mag.GetMaxBullets();
        }

        #region Callbacks
        private void UpdateMagDisplay(float oldCurrentBullets, float newCurrentBullets)
        {
            magDisplay.text = $"{newCurrentBullets}/{weapon.Mag.GetMaxBullets()}";
        }
        #endregion

        #region Commands
        [Command]
        public void CmdShoot(Vector3 position, Vector3 direction)
        {
            currentBullets--;
            lastTimeFired = Time.time;
           // ServerShootingHandler.Instance.CmdShoot(position, direction);
        }
        #endregion

        #region InputHandling
        private void OnShootInput(InputAction.CallbackContext context)
        {
            Debug.Log("Pressed");
            if (mainCamera == null)
            {
                Debug.LogWarning("Main camera not found. Unable to determine shooting direction.");
                return;
            }

            if (context.phase == InputActionPhase.Started && InputTypeHandled(context))
            {
                if (weapon.Mode == WeaponSO.ShootingMode.Press)
                {
                    Shoot();
                }
                else if (weapon.Mode == WeaponSO.ShootingMode.Hold)
                {
                    isFiring = true;
                }
            }
            else if (context.phase == InputActionPhase.Canceled)
            {
                isFiring = false;
            }
        }

        private bool InputTypeHandled(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed && context.duration > 0.2f)
            {
                return weapon.Mode == WeaponSO.ShootingMode.Hold;
            }

            return context.phase == InputActionPhase.Started &&
                   (weapon.Mode == WeaponSO.ShootingMode.Press);
        }
        #endregion

        #region Local
        private void Shoot()
        {
            Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y, mainCamera.nearClipPlane));
            Vector3 shootingDirection = (mouseWorldPosition - shootingPoint.position).normalized;
            CmdShoot(shootingPoint.position, shootingDirection);
        }
        #endregion
    }
}

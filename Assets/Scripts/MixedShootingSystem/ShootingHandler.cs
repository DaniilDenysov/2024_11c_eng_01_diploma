using Cinemachine;
using Mirror;
using ShootingSystem.Local;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace ShootingSystem.Client
{
    public class ShootingHandler : NetworkBehaviour
    {
        [SerializeField] private LayerMask hitLayers;
        [SerializeField] private WeaponSO weapon;
        [SerializeField] private Transform shootingPoint;
        private DefaultInput inputActions;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private TMP_Text magDisplay;

        [SyncVar] private float lastTimeFired;
        [SyncVar(hook = nameof(UpdateMagDisplay))] private int currentBullets;
        [SyncVar] private bool isFiring;

        private void Start()
        {
            if (!isClient) return;
            Construct();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            if (!isClient) return;
            if (inputActions.Player.Shoot.ReadValue<float>() == 0)
            {
                return;
            }
            Shoot();
        }

        public void Construct()
        {
            inputActions = new DefaultInput();
            inputActions.Enable();
            inputActions.Player.Shoot.started += OnShootInputStarted;
            inputActions.Player.Shoot.performed += OnShootInputStarted;
            inputActions.Player.Shoot.canceled += OnShootInputCanceled;
            lastTimeFired = -weapon.GetFireRate();
            currentBullets = weapon.GetMag().GetMaxBullets();
        }

        #region Callbacks
        private void UpdateMagDisplay(int oldCurrentBullets, int newCurrentBullets)
        {
            magDisplay.text = $"{newCurrentBullets}/{weapon.GetMag().GetMaxBullets()}";
        }
        #endregion

        #region Commands
        [Command]
        public void CmdShoot(Vector3 position, Vector3 direction)
        {
            int bulletsShooted = Mathf.Min(currentBullets, weapon.GetBulletsPerShot());
            currentBullets-=bulletsShooted;
            lastTimeFired = Time.time;
            for (int i = 0; i<bulletsShooted;i++)
            {
                ShootRaycast(position, direction);
            }
        }
        #endregion

        #region RPCs
        [ClientRpc]
        private void RpcOnShoot(Vector3 from, Vector3 hitPoint)
        {
            Debug.DrawLine(from, hitPoint, Color.red, 5.0f);
        }
        #endregion

        #region InputHandling
        private void OnShootInputStarted(InputAction.CallbackContext context)
        {
            if (weapon.GetFireMode() == WeaponSO.ShootingMode.Press)
            {
                Shoot();
            }
            else
            {
                isFiring = true;
            }
        }

        private void OnShootInputCanceled(InputAction.CallbackContext context)
        {
            isFiring = false;
        }

        #endregion

        #region Server
        [Server]
        public void ShootRaycast(Vector3 from, Vector3 direction, NetworkConnectionToClient conn = null)
        {
            direction = direction.normalized;
            if (TryShoot(from, direction, out RaycastHit hit))
            {
                if (hit.collider.TryGetComponent(out IDamagable damagable))
                {
                    damagable.DoDamage(weapon.GetDamage());
                }
                RpcOnShoot(from, hit.point);
            }
            else
            {
                RpcOnShoot(from, direction.normalized * weapon.GetDistance());
            }
        }
        #endregion



        #region Local
        private bool CanShoot()
        {
            return (lastTimeFired + weapon.GetFireRate() <= Time.time) && (currentBullets > 0);
        }


        public bool TryShoot(Vector3 from, Vector3 direction, out RaycastHit raycastHit)
        {
            return Physics.Raycast(from, direction, out raycastHit, weapon.GetDistance(), hitLayers);
        }

        private void Shoot()
        {
            if (mainCamera == null)
            {
                Debug.LogWarning("Main camera not found. Unable to determine shooting direction.");
                return;
            }
            if (!CanShoot()) return;

            Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y, mainCamera.nearClipPlane));
            Vector3 shootingDirection = (mouseWorldPosition - shootingPoint.position).normalized;
            CmdShoot(shootingPoint.position, shootingDirection);
        }

        private void OnDestroy()
        {
            inputActions.Player.Shoot.started -= OnShootInputStarted;
            inputActions.Player.Shoot.canceled -= OnShootInputCanceled;
        }
        #endregion
    }
}

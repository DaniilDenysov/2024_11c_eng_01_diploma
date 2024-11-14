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
        private float lastTimeFired;
        private int currentBullets;
        private bool isFiring;

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
            lastTimeFired = -weapon.FireRate;
            currentBullets = weapon.Mag.GetMaxBullets();
        }

        #region Callbacks
        private void UpdateMagDisplay()
        {
            magDisplay.text = $"{currentBullets}/{weapon.Mag.GetMaxBullets()}";
        }
        #endregion

        #region Commands
        [Command]
        public void CmdShootRaycast(Vector3 from, Vector3 direction, NetworkConnectionToClient conn = null)
        {
            direction = direction.normalized;
            if (TryShoot(from, direction, out RaycastHit hit))
            {
                if (hit.collider.TryGetComponent(out IDamagable damagable))
                {
                    damagable.DoDamage(weapon.Damage);
                }
                SpawnBulletHole(hit.collider.gameObject,hit.point,hit.normal);
                RpcOnShoot(from, hit.point);
            }
            else
            {
                RpcOnShoot(from, direction.normalized * weapon.Distance);
            }
        }
        #endregion

        #region RPCs
        [ClientRpc]
        private void RpcOnShoot(Vector3 from, Vector3 hitPoint)
        {
            Debug.DrawLine(from, hitPoint, Color.red, 5.0f);
        }

        [ClientRpc]
        private void SpawnBulletHole(GameObject parent, Vector3 holePosition, Vector3 normal)
        {
            if (weapon.BulletHole == null)
            {
                Debug.LogWarning("No bullet hole prefab assigned in the weapon data.");
                return;
            }
            Quaternion holeRotation = Quaternion.LookRotation(normal);
            Vector3 offsetPosition = holePosition + normal * 0.01f; 

            GameObject hole;
            if (parent != null)
            {
                hole = Instantiate(weapon.BulletHole, offsetPosition, holeRotation);
                hole.transform.SetParent(parent.transform, worldPositionStays: true);
                hole.transform.localScale = Vector3.one;
            }
            else
            {
                hole = Instantiate(weapon.BulletHole, offsetPosition, holeRotation);
            }
        }

        #endregion

        #region InputHandling
        private void OnShootInputStarted(InputAction.CallbackContext context)
        {
            if (weapon.Mode == WeaponSO.ShootingMode.Press)
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
        #endregion



        #region Local
        private bool CanShoot()
        {
            return (lastTimeFired + weapon.FireRate <= Time.time) && (currentBullets > 0);
        }


        public bool TryShoot(Vector3 from, Vector3 direction, out RaycastHit raycastHit)
        {
            return Physics.Raycast(from, direction, out raycastHit, weapon.Distance, hitLayers);
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
            mouseWorldPosition += new Vector3(Random.Range(-weapon.RangeX,weapon.RangeX), Random.Range(-weapon.RangeY, weapon.RangeY));
            Vector3 shootingDirection = (mouseWorldPosition - shootingPoint.position).normalized;
            int bulletsShooted = Mathf.Min(currentBullets, weapon.BulletsPerShot);
            currentBullets -= bulletsShooted;
            lastTimeFired = Time.time;
            UpdateMagDisplay();
            for (int i = 0; i < bulletsShooted; i++)
            {
                CmdShootRaycast(mouseWorldPosition, shootingDirection);
            }
        }

        private void OnDestroy()
        {
            inputActions.Player.Shoot.started -= OnShootInputStarted;
            inputActions.Player.Shoot.canceled -= OnShootInputCanceled;
        }
        #endregion
    }
}

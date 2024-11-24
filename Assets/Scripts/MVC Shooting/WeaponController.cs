using Mirror;
using ShootingSystem.Local;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ShootingSystem
{ 
    public class WeaponController : NetworkBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private WeaponView view;
        [SerializeField] private WeaponModel model;
        private DefaultInput inputActions;
        private bool isFiring;
        private float lastTimeFired;

        private void Start()
        {
            if (!isLocalPlayer) return;
            Construct();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            if (!isLocalPlayer) return;
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
            inputActions.Player.MainWeapon.performed += OnMainWeaponSelected;
            inputActions.Player.SecondaryWeapon.performed += OnSecondaryWeaponSelected;
            lastTimeFired = -model.GetWeaponSO().FireRate;
            Debug.Log(model.GetWeaponSO());
            Debug.Log(model.GetWeaponSO().Mag);
            model.CurrentBullets = model.GetWeaponSO().Mag.GetMaxBullets();
        }

        #region Commands
        [Command]
        public void CmdShootRaycast(Vector3 from, Vector3 direction, NetworkConnectionToClient conn = null)
        {
            direction = direction.normalized;
            if (TryShoot(from, direction, out RaycastHit hit))
            {
                if (hit.collider.TryGetComponent(out IDamagable damagable))
                {
                    damagable.DoDamage(model.GetWeaponSO().Damage, conn);
                }
              //  SpawnBulletHole(hit.collider.gameObject, hit.point, hit.normal);
             //   RpcOnShoot(from, hit.point);
            }
            else
            {
                RpcOnShoot(from, direction.normalized * model.GetWeaponSO().Distance);
            }
        }
        #endregion

        #region RPCs
        [ClientRpc]
        private void RpcOnShoot(Vector3 from, Vector3 hitPoint)
        {
            Debug.DrawLine(from, hitPoint, Color.red, 5.0f);
        }

 
        private void SpawnBulletHole(GameObject parent, Vector3 holePosition, Vector3 normal)
        {
            if (model.GetWeaponSO().BulletHole == null)
            {
                Debug.LogWarning("No bullet hole prefab assigned in the weapon data.");
                return;
            }
            Quaternion holeRotation = Quaternion.LookRotation(normal);
            Vector3 offsetPosition = holePosition + normal * 0.01f;

            GameObject hole;
            if (parent != null)
            {
                hole = Instantiate(model.GetWeaponSO().BulletHole, offsetPosition, holeRotation);
                hole.transform.SetParent(parent.transform, worldPositionStays: true);
                hole.transform.localScale = Vector3.one;
            }
            else
            {
                hole = Instantiate(model.GetWeaponSO().BulletHole, offsetPosition, holeRotation);
            }
            NetworkServer.Spawn(hole);
        }

        #endregion

        #region InputHandling
        private void OnShootInputStarted(InputAction.CallbackContext context)
        {
            if (!isLocalPlayer) return;
            if (model.GetWeaponSO().Mode == WeaponSO.ShootingMode.Press)
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
            if (!isLocalPlayer) return;
            isFiring = false;
        }

        private void OnMainWeaponSelected(InputAction.CallbackContext context)
        {
            if (!isLocalPlayer) return;
            if (!model.IsEquiped(0))
            {
                model.SetWeaponSO(0);
                WeaponSO weaponSO = model.GetWeaponSO();
                view.CmdChangeWeapon(weaponSO);
            }
        }
        private void OnSecondaryWeaponSelected(InputAction.CallbackContext context)
        {
            if (!isLocalPlayer) return;
            if (!model.IsEquiped(1))
            {
                model.SetWeaponSO(1);
                WeaponSO weaponSO = model.GetWeaponSO();
                view.CmdChangeWeapon(weaponSO);
            }
        }
        #endregion

        #region Server
        #endregion



        #region Local
        private bool CanShoot()
        {
            return (lastTimeFired + model.GetWeaponSO().FireRate <= Time.time) && (model.CurrentBullets > 0);
        }


        public bool TryShoot(Vector3 from, Vector3 direction, out RaycastHit raycastHit)
        {
            return Physics.Raycast(from, direction, out raycastHit, model.GetWeaponSO().Distance, model.GetHitLayerMask());
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
            mouseWorldPosition += new Vector3(Random.Range(-model.GetWeaponSO().RangeX, model.GetWeaponSO().RangeX), Random.Range(-model.GetWeaponSO().RangeY, model.GetWeaponSO().RangeY));
            Vector3 shootingDirection = (mouseWorldPosition - mainCamera.transform.position).normalized;
            int bulletsShooted = Mathf.Min(model.CurrentBullets, model.GetWeaponSO().BulletsPerShot);
            model.CurrentBullets -= bulletsShooted;
            lastTimeFired = Time.time;
            view.SetCurrentBullets(model.CurrentBullets, model.GetWeaponSO().Mag.GetMaxBullets());
            for (int i = 0; i < bulletsShooted; i++)
            {
                CmdShootRaycast(mainCamera.transform.position, shootingDirection);
            }
        }
        #endregion

    }
}

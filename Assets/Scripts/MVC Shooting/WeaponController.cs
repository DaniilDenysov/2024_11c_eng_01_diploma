using Mirror;
using Shooting;
using ShootingSystem.Local;
using System;
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
            inputActions.Player.Reload.performed += OnReloaded;
            inputActions.Player.MainWeapon.performed += OnMainWeaponSelected;
            inputActions.Player.SecondaryWeapon.performed += OnSecondaryWeaponSelected;
            lastTimeFired = -model.GetWeaponSO().FireRate;
            Debug.Log(model.GetWeaponSO());
            Debug.Log(model.GetWeaponSO().Mag);
            model.CurrentBullets = model.GetWeaponSO().Mag.GetMaxBullets();
            view.SetCurrentBullets(model.CurrentBullets, model.GetWeaponSO().Mag.GetMaxBullets());
        }

        public void SpawnProjectile(Vector3 from, Vector3 to, NetworkConnectionToClient conn = null)
        {
            var projectile = Instantiate(model.GetWeaponSO().Projectile).gameObject;
            NetworkServer.Spawn(projectile, conn.identity.gameObject);
            if (projectile.TryGetComponent(out RaycastProjectile projObj))
            {
                projObj.Fire(from, to);
            }
        }

        #region Commands
        [Command]
        public void CmdShootRaycast(Vector3 from, Vector3 direction, NetworkConnectionToClient conn = null)
        {
            

            if (TryShoot(from, direction.normalized, out RaycastHit hit))
            {
                if (hit.collider.TryGetComponent(out IDamagable damagable))
                {
                    damagable.DoDamage(model.GetWeaponSO().Damage, conn);
                }
                SpawnProjectile(model.GetShootingPoint().position, hit.point, conn);
                Debug.Log(hit.point);
                RpcSpawnBulletHole(hit.point, hit.normal, hit.collider.GetComponent<NetworkIdentity>());
                //  SpawnBulletHole(hit.collider.gameObject, hit.point, hit.normal);
                RpcOnShoot(from, hit.point);
            }
            else
            {
                SpawnProjectile(model.GetShootingPoint().position, direction * model.GetWeaponSO().Distance, conn);
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
        private void RpcSpawnBulletHole(Vector3 hitPosition, Vector3 hitNormal, NetworkIdentity hitObject)
        {
            GameObject bulletHole = Instantiate(model.GetWeaponSO().BulletHole, hitPosition, Quaternion.LookRotation(hitNormal));
            if (hitObject != null)
            {
                bulletHole.transform.SetParent(hitObject.transform);
            }
            Destroy(bulletHole, 3f);
        }

        #endregion

        #region InputHandling
        private void OnReloaded(InputAction.CallbackContext obj)
        {
            if (!isLocalPlayer) return;
            model.CurrentBullets = model.GetWeaponSO().Mag.GetMaxBullets();
            view.SetCurrentBullets(model.CurrentBullets, model.GetWeaponSO().Mag.GetMaxBullets());
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
            mouseWorldPosition += new Vector3(UnityEngine.Random.Range(-model.GetWeaponSO().RangeX, model.GetWeaponSO().RangeX), UnityEngine.Random.Range(-model.GetWeaponSO().RangeY, model.GetWeaponSO().RangeY));
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

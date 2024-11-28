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
        [SerializeField] private PlayerCameraController mainCamera;
        [SerializeField] private WeaponView view;
        [SerializeField] private WeaponModel model;


        //move to model
        [SerializeField] private float recoilIntensity = 1.5f;
        [SerializeField] private float recoilRecoverySpeed = 5f; 

        private Vector3 targetRecoilOffset = Vector3.zero;
        private Vector3 currentRecoilOffset = Vector3.zero;
        private float recoilResetSpeed = 5f;

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
          //  RecoverRecoil();
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
           // inputActions.Player.MainWeapon.performed += OnMainWeaponSelected;
          //  inputActions.Player.SecondaryWeapon.performed += OnSecondaryWeaponSelected;
            lastTimeFired = -model.GetWeaponSO().FireRate;
            Debug.Log(model.GetWeaponSO());
            Debug.Log(model.GetWeaponSO().Mag);
            model.CurrentBullets = model.GetWeaponSO().Mag.GetMaxBullets();
            view.SetCurrentBullets(model.CurrentBullets, model.GetWeaponSO().Mag.GetMaxBullets());
        }


        #region Commands

        [Command]
        private void OnShoted (NetworkIdentity target, Vector3 from, Vector3 hitpoint)
        {
            if (!target.TryGetComponent(out LagCompensator compensator)) return;

            if (!compensator.RaycastCheck(connectionToClient,from, hitpoint,0f,model.GetHitLayerMask(),out RaycastHit hit)) return;

            if (hit.collider.TryGetComponent(out IDamagable damagable))
            {
                damagable.DoDamage(model.GetWeaponSO().Damage, connectionToClient);
            }
        }

        [Command]
        private void CmdSpawnBulletHole(Vector3 hitPosition, Vector3 hitNormal, NetworkIdentity hitObject)
        {
            RpcSpawnBulletHole(hitPosition,hitNormal, hitObject);
        }

        [Command]
        private void CmdSpawnProjectile(Vector3 from, Vector3 to)
        {
            RpcSpawnProjectile(from, to);
        }

        [Command]
        private void CmdSpawnShootingVFX()
        {
            RpcSpawnShootingVFX();
        }

        #endregion


        #region Client

        [Client]
        public void ShootRaycast(Vector3 from, Vector3 direction, NetworkConnectionToClient conn = null)
        {
            CmdSpawnShootingVFX();
            if (TryShoot(from, direction.normalized, out RaycastHit hit))
            {
                if (hit.collider.TryGetComponent(out NetworkIdentity identity)) OnShoted(identity,from, hit.point);
                CmdSpawnProjectile(model.GetShootingPoint().position, hit.point);
                CmdSpawnBulletHole(hit.point, hit.normal, hit.collider.GetComponent<NetworkIdentity>());
            }
            else
            {
                CmdSpawnProjectile(model.GetShootingPoint().position, direction * model.GetWeaponSO().Distance);
            }
        }


   
        public void CmdShootRaycast(Vector3 from, Vector3 direction, NetworkConnectionToClient conn = null)
        {
            CmdSpawnShootingVFX();
            if (TryShoot(from, direction.normalized, out RaycastHit hit))
            {
                if (hit.collider.TryGetComponent(out IDamagable damagable))
                {
                    damagable.DoDamage(model.GetWeaponSO().Damage, conn);
                }
                CmdSpawnProjectile(model.GetShootingPoint().position, hit.point);
                CmdSpawnBulletHole(hit.point, hit.normal, hit.collider.GetComponent<NetworkIdentity>());
            }
            else
            {
                CmdSpawnProjectile(model.GetShootingPoint().position, direction * model.GetWeaponSO().Distance);
            }
        }

        #endregion

        #region RPCs

        [ClientRpc]
        public void RpcSpawnProjectile(Vector3 from, Vector3 to)
        {
            var projectile = Instantiate(model.GetWeaponSO().Projectile).gameObject;
            if (projectile.TryGetComponent(out RaycastProjectile projObj))
            {
                projObj.Fire(from, to);
            }
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

        [ClientRpc]
        private void RpcSpawnShootingVFX()
        {
            model.GetBulletCase().Play();
            model.GetMuzzleLight().Play();
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
            var weapon = model.GetWeaponSO();
            int bulletsShooted = Mathf.Min(model.CurrentBullets, weapon.BulletsPerShot);
            model.CurrentBullets -= bulletsShooted;
            lastTimeFired = Time.time;
            view.SetCurrentBullets(model.CurrentBullets, weapon.Mag.GetMaxBullets());
            mainCamera.ApplyRecoil(new Vector2(weapon.HorizontalRecoil,weapon.VerticalRecoil));
            for (int i = 0; i < bulletsShooted; i++)
            {
                bool applyAccuracy = UnityEngine.Random.Range(0, 100f) >= weapon.Accuracy;
                Vector3 spread = applyAccuracy ? new Vector3(
                    UnityEngine.Random.Range(-weapon.RangeX, weapon.RangeX),
                    UnityEngine.Random.Range(-weapon.RangeY, weapon.RangeY),
                    0
                ) : Vector3.zero;
                spread = mainCamera.transform.TransformDirection(spread);
                Vector3 finalDirection =  mainCamera.transform.forward + spread;
 
                CmdShootRaycast(mainCamera.transform.position, finalDirection);
            }
        }

        #endregion

    }
}

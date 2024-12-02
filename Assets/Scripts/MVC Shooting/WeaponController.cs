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

        private DefaultInput inputActions;
        private bool isFiring;
        private float lastTimeFired;

        private float initialClickTime;
        private float StopShootTime;
        private bool LastFrameWantedToShoot;


        private void Start()
        {
            if (!isLocalPlayer) return;
            Construct();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Tick(bool wantsToShoot)
        {
            if (wantsToShoot)
            {
                LastFrameWantedToShoot = true;
                //shoot
            }
            else if (LastFrameWantedToShoot)
            {
                StopShootTime = Time.time;
                LastFrameWantedToShoot = false;
            }
        }

        private void Update()
        {
            if (!isLocalPlayer) return;
            //  RecoverRecoil();

            if (inputActions.Player.Shoot.ReadValue<float>() == 0)
            {
                Tick(false);
                return;
            }
            else
            {
                if (!LastFrameWantedToShoot)
                {
                    initialClickTime = Time.time;
                }
            }
            Tick(true);
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
        private void CmdSpawnHitVFX(Vector3 shootingPoint, Vector3 hitPosition, Vector3 hitNormal, NetworkIdentity hitObject)
        {
            SpawnHitVFX(shootingPoint, hitPosition, hitNormal, hitObject);
        }

        [Command]
        private void CmdSpawnVFX(Vector3 shootingPoint, Vector3 hitPosition)
        {
            SpawnVFX(shootingPoint, hitPosition);
        }
        #endregion


        #region Client

        /*  [Client]
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
          }*/


        public void CmdShootRaycast(Vector3 from, Vector3 direction, NetworkConnectionToClient conn = null)
        {
            var weapon = model.GetWeaponSO();
            var shootingPoint = model.GetShootingPoint().position;
           // CmdSpawnShootingVFX();
            if (TryShoot(from, direction.normalized, out RaycastHit hit))
            {
                if (hit.collider.TryGetComponent(out IDamagable damagable))
                {
                    damagable.DoDamage(weapon.Damage, conn);
                }
                //   CmdSpawnProjectile(model.GetShootingPoint().position, hit.point);
                //  CmdSpawnBulletHole(hit.point, hit.normal, hit.collider.GetComponent<NetworkIdentity>());
                SpawnProjectile(shootingPoint, hit.point);
                SpawnBulletHole(hit.point, hit.normal, hit.collider.GetComponent<NetworkIdentity>());
                CmdSpawnHitVFX(shootingPoint, hit.point, hit.normal, hit.collider.GetComponent<NetworkIdentity>());
            }   
            else
            {
                SpawnProjectile(shootingPoint, direction * weapon.Distance);
                CmdSpawnVFX(shootingPoint, direction * weapon.Distance);
            }
            PlayShootingVFX();
        }

        #endregion

        #region RPCs
        [ClientRpc(includeOwner = false)]
        private void SpawnVFX(Vector3 shootingPoint, Vector3 hitPosition)
        {
            SpawnProjectile(shootingPoint, hitPosition);
            PlayShootingVFX();
        }

        [ClientRpc(includeOwner = false)]
        private void SpawnHitVFX(Vector3 shootingPoint, Vector3 hitPosition, Vector3 hitNormal, NetworkIdentity hitObject)
        {
            SpawnProjectile(shootingPoint, hitPosition);
            SpawnBulletHole(hitPosition, hitNormal, hitObject);
            PlayShootingVFX();
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

        public void SpawnProjectile(Vector3 from, Vector3 to)
        {
            var projectile = Instantiate(model.GetWeaponSO().Projectile).gameObject;
            if (projectile.TryGetComponent(out RaycastProjectile projObj))
            {
                projObj.Fire(from, to);
            }
        }

        private void SpawnBulletHole(Vector3 hitPosition, Vector3 hitNormal, NetworkIdentity hitObject)
        {
            GameObject bulletHole = Instantiate(model.GetWeaponSO().BulletHole, hitPosition, Quaternion.LookRotation(hitNormal));
            if (hitObject != null)
            {
                bulletHole.transform.SetParent(hitObject.transform);
            }
            Destroy(bulletHole, 3f);
        }


        private void PlayShootingVFX()
        {
            model.GetBulletCase().Play();
            model.GetMuzzleLight().Play();
        }

        public bool TryShoot(Vector3 from, Vector3 direction, out RaycastHit raycastHit)
        {
            return Physics.Raycast(from, direction, out raycastHit, model.GetWeaponSO().Distance, model.GetHitLayerMask());
        }

        private void Shoot()
        {
            if (mainCamera == null)
            {
                Debug.LogWarning("Main camera not found. Unable to determine shooting direction!");
                return;
            }

            var weapon = model.GetWeaponSO();

            if (weapon == null)
            {
                Debug.LogError("Weapon not found!");
                return;
            }

            if (!CanShoot()) return;

            float lastDuration = Mathf.Clamp(0, (StopShootTime - initialClickTime), weapon.MaxSpreadTime);
            float lerpTime = (weapon.RecoilRecoverySpeed - (Time.time - StopShootTime)/ weapon.RecoilRecoverySpeed);
            initialClickTime = Time.time - Mathf.Lerp(0,lastDuration, Mathf.Clamp01(lerpTime));

            int bulletsShooted = Mathf.Min(model.CurrentBullets, weapon.BulletsPerShot);
            model.CurrentBullets -= bulletsShooted;
            lastTimeFired = Time.time;
            view.SetCurrentBullets(model.CurrentBullets, weapon.Mag.GetMaxBullets());
          //  mainCamera.ApplyRecoil(new Vector2(weapon.HorizontalRecoil, weapon.VerticalRecoil));

            for (int i = 0; i < bulletsShooted; i++)
            {
                Vector3 additionalSpread = weapon.GetTextureDirection(Time.time - initialClickTime);
                Vector3 spreadDirection = mainCamera.transform.forward + additionalSpread;
                Vector3 spreadDirectionUnit = additionalSpread.normalized;
                float horizontalRecoilApplied = (spreadDirectionUnit.x >= 0 ? 1 : -1) * weapon.HorizontalRecoil;
                float verticalRecoilApplied = (spreadDirectionUnit.y >= 0 ? 1 : -1) * weapon.VerticalRecoil;
                Vector2 appliedRecoil = new Vector2(horizontalRecoilApplied, verticalRecoilApplied);

                mainCamera.ApplyRecoil(appliedRecoil);
                Debug.Log($"Applied recoil:{appliedRecoil}");
                CmdShootRaycast(mainCamera.transform.position, mainCamera.transform.forward);
            }
        }


        #endregion

    }
}

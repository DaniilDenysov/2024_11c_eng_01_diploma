using DesignPatterns.Singleton;
using Mirror;
using ShootingSystem.Local;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShootingSystem.Server
{
    public class ServerShootingHandler : NetworkSingleton<ServerShootingHandler>
    {
        [SerializeField] private LayerMask hitLayers;
        [SerializeField, Range(0, 1000f)] private float maxShootDistance = 100f;
        [SerializeField,Range(0, 100f)] private int damageAmount = 10;

        public override ServerShootingHandler GetInstance()
        {
            return this;
        }

        #region Commands

        [Server]
        public void CmdShoot(Vector3 from, Vector3 direction, NetworkConnectionToClient conn = null)
        {
            Debug.Log("Command shoot");
            direction = direction.normalized;
            if (CanShoot(conn))
            {
                if (TryShoot(from, direction, out RaycastHit hit))
                {
                    Debug.Log("Not");
                    ApplyDamage(hit);
                    RpcOnShoot(from, hit.point);
                }
                else
                {
                    Debug.Log("Not hit");
                    RpcOnShoot(from, direction.normalized*maxShootDistance);
                }
            }
        }
        #endregion

        #region Local
        public void Shoot(Vector3 from, Vector3 to, NetworkConnectionToClient conn = null)
        {
            CmdShoot(from,to,conn);
        }

        public bool TryShoot(Vector3 from, Vector3 direction, out RaycastHit raycastHit)
        {
            return Physics.Raycast(from, direction, out raycastHit, maxShootDistance, hitLayers);
        }

        private bool CanShoot (NetworkConnectionToClient conn)
        {
            return true;
        }

        private void ApplyDamage(RaycastHit hit)
        {
            var target = hit.collider.GetComponent<IDamagable>();
            if (hit.collider.TryGetComponent(out IDamagable damagable))
            {
                target.DoDamage(damageAmount);
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
    }
}

using Mirror;
using ShootingSystem.Local;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShootingSystem.Server
{
    public class ServerShootingHandler : NetworkBehaviour
    {
        [SerializeField] private LayerMask hitLayers;
        [SerializeField, Range(0, 1000f)] private float maxShootDistance = 100f;
        [SerializeField,Range(0, 100f)] private int damageAmount = 10;

        #region Commands
        [Command]
        public void CmdShoot(Vector3 from, Vector3 to, NetworkConnectionToClient conn = null)
        {
            if (CanShoot(conn))
            {
                if (TryShoot(from, to, out RaycastHit hit))
                {
                    ApplyDamage(hit);
                    RpcOnShoot(from, to, hit.point);
                }
                else
                {
                    RpcOnShoot(from, to, to);
                }
            }
        }
        #endregion

        #region Local
        public bool TryShoot(Vector3 from, Vector3 to, out RaycastHit raycastHit)
        {
            Vector3 direction = (to - from).normalized;
            float distance = Vector3.Distance(from, to);
            return Physics.Raycast(from, direction, out raycastHit, Mathf.Min(distance, maxShootDistance), hitLayers);
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
        private void RpcOnShoot(Vector3 from, Vector3 to, Vector3 hitPoint)
        {
            Debug.DrawLine(from, hitPoint, Color.red, 1.0f);
        }
        #endregion
    }
}

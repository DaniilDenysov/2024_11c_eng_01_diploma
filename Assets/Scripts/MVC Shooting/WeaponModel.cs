using Mirror;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace ShootingSystem
{
    public class WeaponModel : NetworkBehaviour
    {
        [SerializeField] private Transform shootingPoint;
        [SerializeField] private LayerMask hitLayers;
        [SerializeField] private WeaponSO currentWeaponSO;
        [SerializeField] private ParticleSystem bulletCase;
        [SerializeField] private ParticleSystem muzzleLight;

        public int CurrentBullets;

        private int currentSpreadIndex = 0;

        private void Start()
        {
            CurrentBullets = currentWeaponSO.Mag.GetMaxBullets();
        }

        public void AdvanceSpreadPoint()
        {
          /*  if (currentWeaponSO.SpreadPoints.Count > 0 && currentSpreadIndex < currentWeaponSO.SpreadPoints.Count - 1)
            {
                currentSpreadIndex++;
            }*/
        }

        public void RetreatSpreadPoint()
        {
            if (currentSpreadIndex > 0)
            {
                currentSpreadIndex--;
            }
        }

        public void ResetSpreadIndex()
        {
            currentSpreadIndex = 0;
        }

        public ParticleSystem GetMuzzleLight() => muzzleLight;
        public ParticleSystem GetBulletCase() => bulletCase;    
        public WeaponSO GetWeaponSO() => currentWeaponSO;
        public LayerMask GetHitLayerMask() => hitLayers;
        public Transform GetShootingPoint() => shootingPoint;
    }
}

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
        [SerializeField] private List<WeaponSO> lodout = new List<WeaponSO>();
        [SerializeField] private WeaponSO currentWeaponSO;

        public int CurrentBullets;


        public void SetWeaponSO (int i)
        {
            if (i < 0 || i >= lodout.Count) return;
            currentWeaponSO = lodout[i];
        }

        public bool IsEquiped(int i)
        {
            return lodout.IndexOf(currentWeaponSO) == i;
        }

        public WeaponSO GetWeaponSO() => currentWeaponSO;
        public LayerMask GetHitLayerMask() => hitLayers;
        public Transform GetShootingPoint() => shootingPoint;
    }
}

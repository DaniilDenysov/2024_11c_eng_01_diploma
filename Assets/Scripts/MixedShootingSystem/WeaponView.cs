using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ShootingSystem
{
    public class WeaponView : NetworkBehaviour
    {
        [SerializeField] private Transform  leftGrip, rightGrip;
        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private TMP_Text magDisplay;

        [Server, ClientRpc]
        public void ChangeWeapon(WeaponSO weaponSO)
        {
            meshFilter.mesh = weaponSO.GetMesh();
            Debug.Log(weaponSO.Burst);
            leftGrip.localPosition = weaponSO.GetLeftGrip();
            rightGrip.localPosition = weaponSO.GetRightGrip();
        }

        [Client]
        public void SetCurrentBullets (int currrentBullets, int maxBullets)
        {
            magDisplay.text = $"{currrentBullets}/{maxBullets}";
        }

        [Command]
        public void CmdChangeWeapon (WeaponSO weaponSO)
        {
            ChangeWeapon(weaponSO);
        }
    }
}

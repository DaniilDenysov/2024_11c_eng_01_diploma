using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ShootingSystem
{
    public class WeaponView : NetworkBehaviour
    {
        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private TMP_Text magDisplay;

        [Client]
        public void SetCurrentBullets (int currrentBullets, int maxBullets)
        {
            magDisplay.text = $"{currrentBullets}/{maxBullets}";
        }
    }
}

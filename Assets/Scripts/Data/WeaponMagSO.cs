using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Mag", menuName = "create new mag")]
public class WeaponMagSO : ScriptableObject
{
    [SerializeField, Range(0, 1000)] private int maxBullets;
    public int GetMaxBullets() => maxBullets;
    [SerializeField, Range(0, 1000)] private float reloadTime;
    public float GetReloadTime() => reloadTime;
}

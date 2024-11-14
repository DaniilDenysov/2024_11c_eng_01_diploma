using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "create new weapon")]
public class WeaponSO : ScriptableObject
{
    [SerializeField, Range(0,1000)] private float fireRate;
    [SerializeField, Range(0,100)] private float damage = 10f;
    [SerializeField, Range(0,10000)] private float distance = 1000f;
    [SerializeField, Range(0,1000)] private int burst = 1, bulletsPerShot = 1;
    [SerializeField] private ShootingMode mode;
    public enum ShootingMode
    {
        Press,
        Hold
    }
    [SerializeField, Range(0,1000)] private float scopeTime;
    [SerializeField] private WeaponMagSO mag;


    public ShootingMode GetFireMode() => mode;
    public float GetFireRate() => fireRate;
    public float GetDamage() => damage;
    public float GetDistance() => distance;
    public float GetScopeTime() => scopeTime;
    public int GetBurst() => burst;
    public int GetBulletsPerShot() => bulletsPerShot;
    public WeaponMagSO GetMag () => mag;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "create new weapon")]
public class WeaponSO : ScriptableObject
{

    [SerializeField] private GameObject bulletHole;
    public GameObject BulletHole { get => bulletHole; private set { } }

    [SerializeField, Range(0, 1000)] private float fireRate;
    public float FireRate { get => fireRate; private set { } }

    [SerializeField, Range(0, 100)] private float damage = 10f;
    public float Damage { get => damage; private set { } }

    [SerializeField, Range(0, 10000)] private float distance = 1000f;
    public float Distance { get => distance; private set { } }

    [SerializeField, Range(0, 1000)] private int burst = 1;
    public float Burst { get => burst; private set { } }

    [SerializeField, Range(0, 1000)] private int bulletsPerShot = 1;
    public int BulletsPerShot { get => bulletsPerShot; private set { } }

    [SerializeField] private ShootingMode mode;
    public ShootingMode Mode { get => mode; private set {} }

    public enum ShootingMode
    {
        Press,
        Hold
    }

    [Header("Recoil")]
    [SerializeField, Range(0,1000f)] private float rangeX;
    [SerializeField, Range(0,1000f)] private float rangeY;
    public float RangeX { get => rangeX; private set { } }
    public float RangeY { get => rangeY; private set { } }

    [SerializeField, Range(0,1000)] private float scopeTime;
    public float ScopeTime { get => scopeTime; private set { } }
    [SerializeField] private WeaponMagSO mag;
    public WeaponMagSO Mag { get => mag; set { } }
}

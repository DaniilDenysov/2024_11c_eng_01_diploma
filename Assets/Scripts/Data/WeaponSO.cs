using CustomTools;
using Shooting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "create new weapon")]
public class WeaponSO : ScriptableObject
{
    [Header("View")]
    [SerializeField] private GameObject bulletHole;
    public GameObject BulletHole { get => bulletHole; private set { } }

    [SerializeField] private RaycastProjectile projectile;
    public RaycastProjectile Projectile { get => projectile; private set { } }

    [SerializeField] private Vector3 leftGrip;
    public Vector3 GetLeftGrip() => leftGrip;
    [SerializeField] private Vector3 rightGrip;
    public Vector3 GetRightGrip() => rightGrip;
    [SerializeField] private Mesh mesh;
    public Mesh GetMesh() => mesh;

    [Header("Stats")]
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
    [SerializeField, Range(0,100)] private float accuracy;
    public float Accuracy { get => accuracy; private set { } }
    [SerializeField, Range(0, 100)] private float verticalRecoil;
    public float VerticalRecoil { get => verticalRecoil; private set { } }
    [SerializeField, Range(0, 100)] private float horizontalRecoil;
    public float HorizontalRecoil { get => horizontalRecoil; private set { } }

    /*   [SerializeField] private Sprite spreadPattern;
       public List<Vector3> SpreadPoints = new List<Vector3>();

       [Range(0, 2f)] public float spreadMultiplier = 1f;

       [ContextMenu("Create pattern")]
       public void ScanAndCreatePattern()
       {
           if (spreadPattern == null)
           {
               Debug.LogError("Spread pattern is null!");
               return;
           }

           Texture2D texture = spreadPattern.texture;
           SpreadPoints.Clear();

           if (texture == null)
           {
               Debug.LogError("Spread pattern texture is null!");
               return;
           }

           for (int x = 0; x < texture.width; x++)
           {
               for (int y = 0; y < texture.height; y++)
               {
                   Color pixel = texture.GetPixel(x, y);
                   if (pixel.a > 0.5f)
                   {
                       float normalizedX = (x / (float)texture.width) - 0.5f;
                       float normalizedY = (y / (float)texture.height) - 0.5f;
                       SpreadPoints.Add(new Vector3(normalizedX, normalizedY, 0));
                   }
               }
           }

           Debug.Log($"Spread pattern scanned! Found {SpreadPoints.Count} spread points.");
       }

       public Vector3 GetRandomSpreadPoint(Transform weaponTransform)
       {
           if (SpreadPoints.Count == 0)
           {
               Debug.LogWarning("No spread points available!");
               return weaponTransform.forward;
           }
           Vector3 randomPoint = SpreadPoints[Random.Range(0, SpreadPoints.Count)];
           Vector3 spreadPoint = weaponTransform.TransformDirection(randomPoint * spreadMultiplier);

           return spreadPoint;
       }*/

    [SerializeField, Range(0,1000f)] private float rangeX;
    [SerializeField, Range(0,1000f)] private float rangeY;
    public float RangeX { get => rangeX; private set { } }
    public float RangeY { get => rangeY; private set { } }

    [SerializeField, Range(0,1000)] private float scopeTime;
    public float ScopeTime { get => scopeTime; private set { } }
    [SerializeField] private WeaponMagSO mag;
    public WeaponMagSO Mag { get => mag; set { } }
}

using CustomTools;
using Shooting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "create new weapon")]
public class WeaponSO : ScriptableObject
{
    [Header("View")]
    [SerializeField] private GameObject bulletHole;
    public GameObject BulletHole { get => bulletHole; private set { } }

    [SerializeField] private RaycastProjectile projectile;
    public RaycastProjectile Projectile { get => projectile; private set { } }

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


    [SerializeField, Range(0, 1000)] private float scopeTime;
    public float ScopeTime { get => scopeTime; private set { } }
    [SerializeField] private WeaponMagSO mag;
    public WeaponMagSO Mag { get => mag; set { } }

    [Header("Recoil")]

    [SerializeField] private float recoilRecoverySpeed = 1f;
    public float RecoilRecoverySpeed { get => recoilRecoverySpeed; private set { } }

    [SerializeField] private float maxSpreadTime = 4f;
    public float MaxSpreadTime { get => maxSpreadTime; private set { } }

    [SerializeField, Range(0, 100), Tooltip("Strength of vertical recoil")] private float verticalRecoil;
    public float VerticalRecoil { get => verticalRecoil; private set { } }
    [SerializeField, Range(0, 100), Tooltip("Strength of horizontal recoil")] private float horizontalRecoil;
    public float HorizontalRecoil { get => horizontalRecoil; private set { } }


    [SerializeField] private Sprite spreadPattern;

    public Vector3 GetTextureDirection(float shootTime)
    {

        if (spreadPattern == null)
        {
            Debug.LogError("Spread pattern is null!");
            return Vector3.zero;
        }

        Texture2D spreadTexture = spreadPattern.texture; //FlipTexture(spreadPattern.texture);
            


        if (spreadTexture == null)
        {
            Debug.LogError("Spread pattern texture is null!");
            return Vector3.zero;
        }

        Vector2 halfSize = new Vector2(spreadTexture.width / 2f, spreadTexture.height / 2f);
        int halfSquareExtents = Mathf.CeilToInt(
            Mathf.Lerp(0.01f, halfSize.x, Mathf.Clamp01(shootTime / maxSpreadTime))
        );

        int minX = Mathf.FloorToInt(halfSize.x) - halfSquareExtents;
        int minY = Mathf.FloorToInt(halfSize.y) - halfSquareExtents;

        Color[] sampleColors = spreadTexture.GetPixels(
            minX, minY, halfSquareExtents * 2, halfSquareExtents * 2
        );

        float[] colorsAsGrey = System.Array.ConvertAll(sampleColors, c => c.grayscale);
        float totalGreyValue = colorsAsGrey.Sum();

        float randomGrey = UnityEngine.Random.Range(0, totalGreyValue);
        int i = 0;

        for (; i < colorsAsGrey.Length; i++)
        {
            randomGrey -= colorsAsGrey[i];
            if (randomGrey <= 0) break;
        }

        int x = minX + i % (halfSquareExtents * 2);
        int y = minY + i / (halfSquareExtents * 2);

        Vector2 targetPosition = new Vector2(x,y);

        Vector2 direction = (targetPosition - halfSize) / halfSize.x;

        return direction;
    }
}

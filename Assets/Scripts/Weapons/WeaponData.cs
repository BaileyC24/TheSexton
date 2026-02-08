using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "The Sexton/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public enum WeaponType { Melee, Ranged }

    [Header("Core")]
    public string weaponName;
    public Sprite weaponIcon;
    public GameObject weaponModel;
    public WeaponType weaponType;
    public AnimatorOverrideController animatorOverride;

    [Header("Damage")]
    public int damage = 5;

    [Header("Melee")]
    public float meleeHitDelay = 0.15f;
    public float meleeTotalTime = 0.6f;

    [Header("Ranged")]
    public bool useHitscan = true;
    public float range = 50f;
    public LayerMask hitMask = ~0;

    [Tooltip("If useHitscan = false, this prefab will be spawned.")]
    public GameObject projectilePrefab;
    public int projectileSpeed = 20;
    public float projectileGravity;
    public float projectileVerticalOffset;
    public float projectileLife = 3f;

    [Tooltip("Shots per second.")]
    public float fireRate = 3f;

    [Tooltip("Small random aim variance in degrees.")]
    public float spreadDegrees = 0.5f;

    [Tooltip("time before spawning the projectile / doing raycast (sync with animation).")]
    public float shootDelay = 0.1f;
}
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "The Sexton/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public enum WeaponType { Melee, Utility }
    public enum SpecialEffect { None, Stun, Blind, Knockback }

    [Header("Core")]
    public string weaponName;
    public Sprite weaponIcon;
    public GameObject weaponModel;

    public WeaponType weaponType = WeaponType.Melee;

    [Header("Animation")]
    public AnimatorOverrideController animatorOverride;

    [Header("Combat")]
    public int damage = 5;
    [Tooltip("How far the hit can reach.")]
    public float range = 2f;
    [Tooltip("How wide the swing is (0 = thrust / straight hit).")]
    [Range(0f, 180f)] public float swingArc = 90f;

    [Header("Timing")]
    public float hitDelay = 0.15f;
    public float totalTime = 0.6f;

    [Header("Special")]
    public SpecialEffect specialEffect = SpecialEffect.None;
    [Range(0f, 1f)] public float specialChance;
    public float specialDuration;

    [Header("Utility Tags")]
    public bool canBreakLocks;
    public bool emitsLight;
    public float lightRadius;
}
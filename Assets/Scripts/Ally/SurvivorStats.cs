using UnityEngine;

[CreateAssetMenu(menuName = "Survivors/Ally")]
public class SurvivorStats : ScriptableObject
{
    [Header("Visuals")]
    public GameObject model;

    [Header("Vitals")]
    public int HP;
    public int FOV;
    public int FaceTargetSpeed;

    [Header("Combat")]
    public float ShootRate;
    public float DetectionRadius;
    public GameObject Bullet;

    [Header("Movement")]
    public float Speed;
    public float Acceleration;
}

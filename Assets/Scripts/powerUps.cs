using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;


[CreateAssetMenu]
public class powerUps : ScriptableObject
{
   
    public int healthCurrent;
    [Range (1,10)] public int healAmount;
    public ParticleSystem healEffect;
    public AudioClip healSound;
    [Range(0,1)] public float healSoundIndex;

   
}

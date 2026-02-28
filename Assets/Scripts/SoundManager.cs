using UnityEngine;

   public enum SoundType
   {
      Axe,
      Magic,
      Jump,
      Land,
      Hit,
      Death,
      Heal,
      Buy,
      Pause,
      Unpause,
      Equip,
      Unequip,
      Menu,
      Footsteps1,
      Footsteps2,
      Denied
   }

[RequireComponent(typeof(AudioSource))]

public class SoundManager : MonoBehaviour
{
   [SerializeField] private AudioClip[] soundList;
   public static SoundManager instance;
   private AudioSource audioSource;

   private void Awake()
   {
      instance = this;
   }

   private void Start()
   {
      audioSource = GetComponent<AudioSource>();
   }
   
   public static void PlaySound(SoundType sound, float volume = 1.0f)
   {
      instance.audioSource.PlayOneShot(instance.soundList[(int)sound], volume);
   }
   
   
}

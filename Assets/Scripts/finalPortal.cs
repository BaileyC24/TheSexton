using UnityEngine;

public class finalPortal : MonoBehaviour
{
   public int nextSceneName;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Load the next scene
            transitionsManager.instance.LoadScene(2, "CrossFade");
        }
    }
}

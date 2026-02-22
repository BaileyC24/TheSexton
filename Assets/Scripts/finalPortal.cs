using UnityEngine;

public class finalPortal : MonoBehaviour
{
   public int nextSceneName;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        { 
            
            // Save the player's inventory data
            gameManager.instance.currentPlayerData.SaveData(InventoryManager.instance);

            // Load the next scene
            transitionsManager.instance.LoadScene(2, "CrossFade");
           
        }
    }
}

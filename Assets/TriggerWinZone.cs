using UnityEngine;


// WinZone defines a level completion area for Graveyard.
// When the player enters this volume, the win condition is triggered.

public class TriggerWinZone : MonoBehaviour //can be attached to a GameObject in the scene
{
    private void OnTriggerEnter(Collider other) //Unity calls this automatically when something enters the trigger
    {
        if (other.CompareTag("Player")) //if the thing that enters the trigger is the player...
        {
            Debug.Log("Player reached Win Zone"); //print this message
            gameManager.instance.youWin();

        }
    }
}

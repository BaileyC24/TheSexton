using UnityEngine;

public class VendorInteract : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        gameManager.instance.OpenMenu(gameManager.MenuType.Store);
    }
}

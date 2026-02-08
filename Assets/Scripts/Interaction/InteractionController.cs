using UnityEngine;

public class InteractionController : MonoBehaviour
{
    [SerializeField] private float interactionDistance = 3f; //I am creating a floating point value variable with a default value of 3
                                            //this is how far away an object can be and still allow player to interact with it
    private Camera playerCamera;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerCamera = Camera.main;
    }

    // Update is called once per frame
    void Update() // this is where the scene "beats" live: What is my character/object doing in this moment?
    {
        //Debug.Log("Update tick");

        // Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * interactionDistance, Color.red, 0.1f); //keep the ray visible for 1/10th of a second so humans can see it. 

        if (!Physics.Raycast(ray, out var hitInfo, interactionDistance) 
            || !gameManager.instance.playerScript.GetInput().Player.Interact.triggered
            || gameManager.instance.isPaused) return;
        
        IInteractable interactable = hitInfo.collider.GetComponent<IInteractable>();
        
        if (interactable != null)
            interactable.Interact();
    }
}

using UnityEngine;

public class InteractionController : MonoBehaviour
{
    public Camera playerCamera;
    public float interactionDistance = 3f; //I am creating a floating point value variable with a default value of 3
                                            //this is how far away an object can be and still allow player to interact with it


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerCamera = Camera.main;
        Debug.LogAssertion("Using camera: " + playerCamera.name); 
      //  Debug.Log("Hey, y'all, InteractionController is running. Woohoo!");
    }

    // Update is called once per frame
    void Update() // this is where the scene "beats" live: What is my character/object doing in this moment?
    {
        // Debug.Log("Update tick");

        // Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));  

        Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * interactionDistance, Color.red, 0.1f); //keep the ray visible for 1/10th of a second so humans can see it. 

        RaycastHit hitInfo; 

        if (Physics.Raycast(ray, out hitInfo, interactionDistance))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {

                IInteractable interactable = hitInfo.collider.GetComponent<IInteractable>(); 
                if (interactable != null)
                {
                    interactable.Interact();
                }
                    //Debug.LogWarning("Interact pressed on " + hitInfo.collider.gameObject.name); 
            }

            Debug.Log("HIT: " + hitInfo.collider.gameObject.name);
            //Debug.Log("Looking at: " + hitInfo.collider.gameObject.name);
        }
        else
        {
            Debug.Log("MISS");
        }
    }
}

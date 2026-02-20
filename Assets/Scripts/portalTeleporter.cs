using UnityEngine;

public class portalTeleporter : MonoBehaviour
{
    public Transform enterPoint;
    public Transform exitPoint;

    public GameObject portal;



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.position = exitPoint.position;
          
        }
    }
}


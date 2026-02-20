using UnityEngine;

public class portalManager : MonoBehaviour
{
    public GameObject portalObject;
    public HellGate_Controller hellGate;

    public void ActivatePortal()
    {   
        portalObject.SetActive(true);
        hellGate.ToggleHellGate();
    }
}

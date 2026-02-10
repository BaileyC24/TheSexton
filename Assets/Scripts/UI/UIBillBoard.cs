using System;
using UnityEngine;

public class UIBillBoard : MonoBehaviour
{
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        transform.forward = cam.transform.forward;
    }
}

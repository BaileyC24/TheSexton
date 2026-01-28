using System;
using UnityEngine;

public class AttackTrigger : MonoBehaviour
{
    public Action<Collider> ATKTRIGGER;

    private void OnTriggerEnter(Collider other)
    {
        ATKTRIGGER.Invoke(other);
    }
}
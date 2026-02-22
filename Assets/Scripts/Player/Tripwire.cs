using System;
using UnityEngine;

public class Tripwire : MonoBehaviour
{
    public float duration = 0.15f;
    public GameObject specialEffect;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Enemy")) return;
        
        var stun = other.GetComponent<IStunnable>();
        if (stun != null) 
            stun.Stun(duration);
        
        GameObject special = Instantiate(specialEffect, other.ClosestPoint(transform.position), Quaternion.identity);
        
        Destroy(gameObject);
        Destroy(special, 0.6f);
    }
}
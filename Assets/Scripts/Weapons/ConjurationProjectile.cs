using UnityEngine;

public class ConjurationProjectile : MonoBehaviour
{
    Rigidbody conjurationProjectileRb;

    [SerializeField] float throwSpeed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        conjurationProjectileRb = GetComponent<Rigidbody>(); //initializing Rigidbody
        conjurationProjectileRb.linearVelocity = transform.forward * throwSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.tag = "Ally";
        }
    }

}

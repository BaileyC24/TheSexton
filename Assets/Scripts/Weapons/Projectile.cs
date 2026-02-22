using UnityEngine;

public class Projectile : MonoBehaviour
{
    Rigidbody projectileRb;

    [SerializeField] float throwSpeed;
    [SerializeField] GameObject Burst;
    [SerializeField] float hitDamage;
    [SerializeField] float damagePerSecond;
    [SerializeField] float damageOverTime; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        projectileRb = GetComponent<Rigidbody>(); //initializing Rigidbody
        projectileRb.linearVelocity = transform.forward * throwSpeed; //setting initial value, using velocity relative to this object (not world) 
    }

    private void OnCollisionEnter(Collision collision) //triggered when collider of this object hits another physcial object
    {
        Instantiate(Burst, transform.position, transform.rotation); //instantiates Burst at exact position and rotation of fireball

        //could add here what happens if it hits an enemy, e.g., subtract damage from Enemy's health
        if (collision.gameObject != null)
        { 
        Debug.Log(collision.gameObject.tag); //inspired by Unity documentation "Detect a gameObject's tag via..."
        }

        IDamage damageable = collision.gameObject.GetComponent<IDamage>();

        Debug.Log("HIT IDamage: damage =" + hitDamage);

        if (damageable != null)
        {
            int intDamage = (int)hitDamage; //convert float to int
            
            damageable.takeDamage(intDamage);
        }
      

        Destroy(this.gameObject); //destroys fireball itself
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

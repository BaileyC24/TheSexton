using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class damage : MonoBehaviour
{
    public enum damageType { ranged, melee, DOT}

    public damageType type;
    [SerializeField] Rigidbody rb;

    [Range(1,10)][SerializeField] int damageAmount;
    [Range(0, 2)][SerializeField] float damageRate;
    [Range(0,25)][SerializeField] int speed;
    [Range(0,40)][SerializeField] float gravity;
    [Range(0,5)][SerializeField] float verticalOffset;
    [SerializeField] float destroyTime;
    [SerializeField] GameObject hitEffect;
    
    bool isDamaging;
    public bool allowedToDamage;

    void Start()
    {
        if (type == damageType.ranged)
        {
            // Apply velocity based on the variables in the inspector
            rb.linearVelocity = (transform.forward + new Vector3(0, verticalOffset, 0)) * speed;
            Destroy(gameObject, destroyTime);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (type == damageType.ranged)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y - (gravity * Time.deltaTime), rb.linearVelocity.z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        IDamage dmg = other.GetComponent<IDamage>();
        
        if (dmg != null && type == damageType.ranged)
        {
            dmg.takeDamage(damageAmount);
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }
        IDamage dmg = other.GetComponent<IDamage>();
        if(dmg != null && type == damageType.DOT && !isDamaging)
        {
            StartCoroutine(damageOther(dmg));
        }
        
        if (allowedToDamage)
        {
            if (dmg != null && type == damageType.melee)
            {
                dmg.takeDamage(damageAmount);
                allowedToDamage = false; 
            }
        }
    }

    IEnumerator damageOther(IDamage d)
    {
        isDamaging = true;
        d.takeDamage(damageAmount);
        yield return new WaitForSeconds(damageRate);
        isDamaging = false;
    }
    
}

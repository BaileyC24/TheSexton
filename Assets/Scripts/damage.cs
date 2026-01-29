using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class damage : MonoBehaviour
{
    enum damageType { ranged, melee, DOT}

    [SerializeField] damageType type;
    [SerializeField] Rigidbody rb;

    [Range(1,10)][SerializeField] int damageAmount;
    [Range(0, 2)][SerializeField] float damageRate;
    [Range(0,25)][SerializeField] int speed;
    [Range(0,40)][SerializeField] float gravity;
    [Range(0,5)][SerializeField] float verticalOffset;
    [SerializeField] float destroyTime;
    [SerializeField] GameObject hitEffect;
   

    bool isDamaging;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (type == damageType.ranged)
        {
            rb.linearVelocity = new Vector3(transform.forward.x * speed, (transform.forward.y * speed) + (verticalOffset * speed) , transform.forward.z * speed);
            Destroy(gameObject, destroyTime);
        }
        if (type == damageType.melee)
        {
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
        if (other.isTrigger)
        {
            return;
        }
        IDamage dmg = other.GetComponent<IDamage>();
        if (dmg != null && type != damageType.DOT)
        {
            dmg.takeDamage(damageAmount);
        }
        if(type == damageType.ranged)
        {
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
        if (dmg != null && type != damageType.DOT)
        {
            dmg.takeDamage(damageAmount);
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

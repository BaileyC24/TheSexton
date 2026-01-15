using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class damage : MonoBehaviour
{
    enum damageType { ranged, melee, DOT}

    [SerializeField] damageType type;
    [SerializeField] Rigidbody rb;

    [Range(1,3)][SerializeField] int damageAmount;
    [Range(0, 2)][SerializeField] float damageRate;
    [Range(5,25)][SerializeField] int speed;
    [Range(15,40)][SerializeField] float gravity;
    [Range(1,5)][SerializeField] float verticalOffset;
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
}

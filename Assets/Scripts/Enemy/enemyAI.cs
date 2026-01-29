using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamage
{
    [Header("Components")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer[] models;
    [SerializeField] Animator animator;
    [SerializeField] Transform attackPos;

    [Header("Movement Settings")]
    [SerializeField] int roamDist;
    [Range(0, 10)] [SerializeField] float navCooldown;
    [Range(1, 20)] [SerializeField] int faceTargetSpeed;

    [Header("Detection Settings")]
    [Range(1, 20)] [SerializeField] float detectRange;
    [Range(5, 20)] [SerializeField] int sightDist;
    [Range(0, 360)] [SerializeField] int FOV;
    [SerializeField] LayerMask targetLayer;

    [Header("Combat Settings")]
    [SerializeField] GameObject weapon;
    [Range(0.1f, 2f)] [SerializeField] float attackSpeed;
    [Range(1, 10)] [SerializeField] int HP;

    [Header("Drops")]
    [SerializeField] private GameObject droppedObj;
    [SerializeField] private float offset;

    float attackTimer;
    float navTimer;
    float playerDistance;
    Vector3 playerDir;
    Vector3 pointOrig;
    float origStopDist;
    Color colorOrig;

    void Start()
    {
        colorOrig = models[0].material.color;
        pointOrig = transform.position;
        origStopDist = agent.stoppingDistance;
    }

    void Update()
    {
        attackTimer += Time.deltaTime;

        playerDistance = Vector3.Distance(transform.position, gameManager.instance.player.transform.position);
        animator.SetBool("Movement", agent.velocity.magnitude > 0.01f);

        if (canSeePlayer())
        {
            // Logic handled inside canSeePlayer for chasing/attacking
        }
        else
        {
            checkRoam();
        }
    }

    bool canSeePlayer()
    {
        playerDir = (gameManager.instance.player.transform.position - transform.position);
        float angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, playerDir, out hit, sightDist, targetLayer))
        {
            // If player is within FOV or very close (detectRange)
            if (angleToPlayer <= FOV || playerDistance <= detectRange)
            {
                agent.stoppingDistance = origStopDist;
                agent.SetDestination(gameManager.instance.player.transform.position);

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    faceTarget();
                    if (attackTimer >= attackSpeed)
                    {
                        attack();
                    }
                }
                return true;
            }
        }
        return false;
    }

    void faceTarget()
    {
        Vector3 lookDir = gameManager.instance.player.transform.position - transform.position;
        lookDir.y = 0;
        Quaternion rot = Quaternion.LookRotation(lookDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    void checkRoam()
    {
        agent.stoppingDistance = 0;
        if (agent.remainingDistance < 0.05f)
        {
            navTimer += Time.deltaTime;
            if (navTimer >= navCooldown)
            {
                roam();
            }
        }
    }

    void roam()
    {
        navTimer = 0;
        Vector3 ranPos = Random.insideUnitSphere * roamDist;
        ranPos += pointOrig;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(ranPos, out hit, roamDist, 1))
        {
            agent.SetDestination(hit.position);
        }
    }

    void attack()
    {
        attackTimer = 0;
        animator.SetTrigger("Attack");
        
        damage weaponDmg = weapon.GetComponent<damage>();

        if (weaponDmg != null && weaponDmg.type == damage.damageType.ranged)
        {
            Instantiate(weapon, attackPos.position, transform.rotation);
        }
        else
        {
            StartCoroutine(MeleeHitWindow());
        }
    }

    IEnumerator MeleeHitWindow()
    {
        damage weaponDamage = weapon.GetComponent<damage>();
        if (weaponDamage != null)
        {
            yield return new WaitForSeconds(0.2f);
            weaponDamage.allowedToDamage = true;
            yield return new WaitForSeconds(0.3f);
            weaponDamage.allowedToDamage = false;
        }
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        if (HP <= 0)
        {
            WaveManager.instance.EnemiesDied();
            dropItem();
            gameManager.instance.exp++;
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(flashRed());
        }
    }

    IEnumerator flashRed()
    {
        foreach (Renderer model in models) model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        foreach (Renderer model in models) model.sharedMaterial.color = colorOrig;
    }

    void dropItem()
    {
        if (droppedObj != null)
        {
            Instantiate(droppedObj, transform.position + new Vector3(0, offset, 0), transform.rotation);
        }
    }
}
using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using UnityEngine.Rendering.UI;

public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer[] models;
    [SerializeField] Transform attackPos;
    [SerializeField] Transform pointDestination;
    [SerializeField] LayerMask targetLayer;

    [Range(1, 20)] [SerializeField] float detectRange;
    [Range(1, 10)] [SerializeField] int HP;
    [Range(1, 20)] [SerializeField] int faceTargetSpeed;
    [Range(0, 10)] [SerializeField] float navCooldown;
    [Range(5, 20)] [SerializeField] int sightDist;

    [SerializeField] GameObject weapon;

    [Range((float)0.1, 2)] [SerializeField]
    float attackSpeed;

    [SerializeField] private GameObject droppedObj;
    [SerializeField] private float offset;


    float attackTimer;

    float playerDistance;
    Vector3 targetDir;

    Color colorOrig;
    Vector3 pointOrig;
    float navCDORig;
    float origStopDist;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrig = models[0].material.color;
        gameManager.instance.updateGameGoal(1);
        pointOrig = transform.position;
        navCDORig = navCooldown;
        origStopDist = agent.stoppingDistance;
    }

    // Update is called once per frame
    void Update()
    {
        attackTimer += Time.deltaTime;
        navCooldown -= Time.deltaTime;

        targetDir = (agent.destination - transform.position);
        playerDistance = Vector3.Distance(transform.position, gameManager.instance.player.transform.position);

        StartCoroutine(setNav());
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(targetDir.x, transform.position.y, targetDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    void attack()
    {
        attackTimer = 0;

        Instantiate(weapon, attackPos.position, transform.rotation);
    }

    IEnumerator setNav()
    {
        RaycastHit hit;

        if (gameManager.instance.playerScript.CurrentState.StateKey != PlayerStateMachine.PlayerStates.Idle &&
            playerDistance <= detectRange ||
            Physics.Raycast(transform.position, transform.forward, out hit, sightDist, targetLayer))
        {
            agent.stoppingDistance = origStopDist;
            agent.SetDestination(gameManager.instance.player.transform.position);
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                faceTarget();
            }

            if (attackTimer >= attackSpeed)
            {
                attack();
            }
        }
        else if (transform.position != pointOrig && navCooldown <= 0)
        {
            agent.stoppingDistance = 0;
            agent.SetDestination(pointOrig);
            yield return new WaitForSeconds(navCDORig);
            navCooldown = navCDORig;
        }
        else
        {
            agent.stoppingDistance = 0;
            agent.SetDestination(pointDestination.position);
        }
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        if (HP <= 0)
        {
            gameManager.instance.updateGameGoal(-1);
            dropItem();
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(flashRed());
        }
    }

    IEnumerator flashRed()
    {
        foreach (Renderer model in models)
            model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        foreach (Renderer model in models)
            model.sharedMaterial.color = colorOrig;
    }

    void dropItem()
    {
        if (droppedObj != null)
        {
            Instantiate(droppedObj,
                new Vector3(transform.position.x, transform.position.y + offset, transform.position.z),
                transform.rotation);
        }
    }

}
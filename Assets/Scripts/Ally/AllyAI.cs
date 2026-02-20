using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using UnityEngine.Rendering.UI;

public class AllyAI : MonoBehaviour, IDamage, IHealable
{
    [Header("Core")]
    [SerializeField] private NavMeshAgent agent;

    [Header("Stats")]
    [SerializeField] private int HP;
    [SerializeField] private int FOV;
    [SerializeField] private int faceTargetSpeed;
    [SerializeField] private float detectionRadius;

    [Header("Combat")]
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform shootPos;
    [SerializeField] private float shootRate;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Nav")]
    [SerializeField] private float navCooldown = 0.25f;

    float shootTimer;
    float navTimer;

    Transform player;
    Transform currentTarget;
    Vector3 holdPosition;

    enum AllyState { Follow, Hold, Combat }
    AllyState currentState = AllyState.Follow;

    void Start()
    {
        player = gameManager.instance.player.transform;
    }

    void Update()
    {
        shootTimer += Time.deltaTime;
        navTimer -= Time.deltaTime;


        AcquireTarget();

        switch (currentState)
        {
            case AllyState.Follow:
                FollowPlayer();
                break;

            case AllyState.Hold:
                HoldPosition();
                break;

            case AllyState.Combat:
                CombatBehavior();
                break;
        }
    }

    void OnEnable()        //whenever this component becomes enabled
    {                       //please run this function automatically
        if (agent == null)   //if my agent variable currently points to nothing
        { agent = GetComponent<NavMeshAgent>(); } //look on this same game object for a NavMeshAgent component and store it in my agent variable

        if (agent == null) //if there still isn't one, meaning the game object must not have one
        { Debug.LogWarning("AllyAI requires a NavMeshAgent on this GameObject.");  }
    }
    
    

    // ------------------ Movement ------------------

    void FollowPlayer()
    {
        if (player == null) return;

        agent.stoppingDistance = 5f;

        if (navTimer <= 0)
        {
            agent.SetDestination(player.position);
            navTimer = navCooldown;
        }
    }

    void HoldPosition()
    {
        agent.stoppingDistance = 0;

        if (navTimer <= 0)
        {
            agent.SetDestination(holdPosition);
            navTimer = navCooldown;
        }
    }

    // ------------------ Combat ------------------

    void AcquireTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);

        currentTarget = null;

        foreach (Collider hit in hits)
        {
            Vector3 dir = hit.transform.position - transform.position;
            float angle = Vector3.Angle(dir, transform.forward);

            if (angle <= FOV)
            {
                currentTarget = hit.transform;
                currentState = AllyState.Combat;
                return;
            }
        }

        if (currentState == AllyState.Combat)
        {
            currentState = AllyState.Follow;
        }
    }

    void CombatBehavior()
    {
        if (currentTarget == null)
        {
            currentState = AllyState.Follow;
            return;
        }

        Vector3 dir = currentTarget.position - transform.position;
        dir.y = 0;

        Quaternion rot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);

        agent.stoppingDistance = detectionRadius * 0.6f;

        if (navTimer <= 0)
        {
            agent.SetDestination(currentTarget.position);
            navTimer = navCooldown;
        }

        if (shootTimer >= shootRate)
        {
            Shoot(dir);
        }
    }

    void Shoot(Vector3 dir)
    {
        shootTimer = 0;
        Instantiate(bullet, shootPos.position, Quaternion.LookRotation(dir));
    }


    // ------------------ Commands ------------------

    public void CommandHold(Vector3 position)
    {
        holdPosition = position;
        currentState = AllyState.Hold;
    }

    public void CommandFollow()
    {
        currentState = AllyState.Follow;
    }

    // ------------------ Damage and Healing ------------------

    public void takeDamage(int amount)
    {
        HP -= amount;
        if (HP <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void heal(int amount)
    {
        HP += amount;
    }

    // ------------------ Stat Injection ------------------

    public void GetAllyStats(SurvivorStats stats)
    {
        HP = stats.HP;
        FOV = stats.FOV;
        faceTargetSpeed = stats.FaceTargetSpeed;
        shootRate = stats.ShootRate;
        detectionRadius = stats.DetectionRadius;
        bullet = stats.Bullet;

        agent.speed = stats.Speed;
        agent.acceleration = stats.Acceleration;
    }
}
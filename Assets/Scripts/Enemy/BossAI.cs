using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BossAI : MonoBehaviour, IDamage
{
    [Header("Components")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator animator;
    [SerializeField] Transform attackPos;
    
    [Header("HP Bar Scaling")]
    [SerializeField] private float scaleAt1m = 0.01f;
    [SerializeField] private float minScale = 0.006f;
    [SerializeField] private float maxScale = 0.02f;
    [SerializeField] private float scaleSmooth = 10f;
    [SerializeField] private Image HPBar;

    [Header("Movement Settings")]
    [SerializeField] int roamDist;
    [Range(0, 10)] [SerializeField] float navCooldown;
    [Range(1, 20)] [SerializeField] int faceTargetSpeed;

    [Header("Detection Settings")]
    [Range(1, 20)] [SerializeField] float detectRange;
    [Range(5, 50)] [SerializeField] int sightDist;
    [Range(0, 360)] [SerializeField] int FOV;
    [SerializeField] LayerMask targetLayer;

    [Header("Damage Popup")]
    [SerializeField] private Transform popupAnchor;
    [SerializeField] private float popupRadius = 0.25f;
    [SerializeField] private float popupUpBias = 0.15f;

    [Header("Combat Settings")] 
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] int enemyCount;
    [SerializeField] GameObject weapon;
    [SerializeField] GameObject projectile;
    [Range(0,25)][SerializeField] int speed;
    [Range(0,40)][SerializeField] float gravity;
    [Range(0,5)][SerializeField] float verticalOffset;
    [SerializeField] float destroyTime;
    [Range(0.1f, 2f)] [SerializeField] float attackSpeed;
    [SerializeField] int HP;
    

    [Header("Drops")]
    [SerializeField] private GameObject[] dropPrefabs;
    [SerializeField][Range(0f, 1f)] private float dropChance = 0.35f;
    [SerializeField] private float dropYOffset = 0.5f;

    float attackTimer;
    float navTimer;
    float playerDistance;
    Vector3 playerDir;
    Vector3 pointOrig;
    float origStopDist;
    private int hpOrig;
    private float displayHP;
    private Vector3 baseScale;
    private Camera cam;
    private damage magic;
    private SpawnManager summon;
    
      void Awake()
    {
        pointOrig = transform.position;
        origStopDist = agent.stoppingDistance;
        hpOrig = HP;
        displayHP = HP;
        
        cam = Camera.main;
        baseScale = Vector3.one * scaleAt1m;
        
        HPBar.gameObject.SetActive(false);
    }

    void Update()
    {
        displayHP = Mathf.MoveTowards(displayHP, HP, 8 * Time.deltaTime);
        HPBar.fillAmount = displayHP / hpOrig;
        
        attackTimer += Time.deltaTime;

        playerDistance = Vector3.Distance(transform.position, gameManager.instance.player.transform.position);

        if (canSeePlayer())
        {
            // Logic handled inside canSeePlayer for chasing/attacking
        }
        else 
        {
           checkRoam();
        }
       
        
        UpdateHPBarScale();
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
                animator.SetBool("Movement", true);
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
        animator.SetBool("Movement", agent.velocity.magnitude > 0.01);
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
        animator.SetTrigger("Attack Melee");
        
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
        
        if (!HPBar.gameObject.activeSelf)
            HPBar.gameObject.SetActive(true);
        
        SpawnDamagePopup(amount);
        SpawnDamagePopup(amount, WeaponData.SpecialEffect.Blind);
        
        if (HP <= 0)
        {
            WaveManager.instance.EnemiesDied();
            dropItem();
            gameManager.instance.exp++;
            animator.SetTrigger("Dead");
        }
      
    }
    
    private void SpawnDamagePopup(int amount, WeaponData.SpecialEffect effect = WeaponData.SpecialEffect.None)
    {
        Vector3 basePos = popupAnchor.position;
        
        Vector3 offset = Random.insideUnitSphere * popupRadius;
        offset.y = Mathf.Abs(offset.y) + popupUpBias;
        Vector3 spawnPos = basePos + offset;
        
        if (effect == WeaponData.SpecialEffect.None)
            DamageManager.instance.CreatePopup(spawnPos, amount.ToString());
        else
            DamageManager.instance.CreatePopup(spawnPos, effect.ToString(), WeaponData.SpecialEffectColor[effect]);
    }

    void dropItem()
    {
        if (dropPrefabs == null || dropPrefabs.Length == 0)
            return;

        if (Random.value > dropChance)
            return;

        int index = Random.Range(0, dropPrefabs.Length);
        GameObject drop = dropPrefabs[index];

        Instantiate(
            drop,
            transform.position + Vector3.up * dropYOffset,
            Quaternion.identity
        );
    }
    
    private void UpdateHPBarScale()
    {
        float dist = Vector3.Distance(cam.transform.position, transform.position);
        
        float target = scaleAt1m * dist;
        target = Mathf.Clamp(target, minScale, maxScale);

        Vector3 targetScale = Vector3.one * target;
        HPBar.rectTransform.localScale = Vector3.Lerp(HPBar.rectTransform.localScale, targetScale, scaleSmooth * Time.deltaTime);
    }

    void Shoot()
    {
        attackTimer = 0;
        animator.SetTrigger("Ranged Attack");
        
        damage magicDmg = projectile.GetComponent<damage>();
        magic.SetRangedStats(speed, gravity, verticalOffset,destroyTime);
        
        if (magicDmg != null && magicDmg.type == damage.damageType.ranged)
        {
            Instantiate(projectile, attackPos.position, transform.rotation);
        }
        
        
    }

    void Summon()
    {
        animator.SetBool("Summon Enemies", true);
        summon.StartSpawning(enemyCount);
        animator.SetBool("Summon Enemies", false);
    }
}

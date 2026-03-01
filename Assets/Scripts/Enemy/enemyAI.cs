using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour, IDamage, IKnockbackable, IBlindable, IStunnable
{
    [Header("Components")] [SerializeField]
    NavMeshAgent agent;

    [SerializeField] Renderer[] models;
    [SerializeField] Animator animator;
    [SerializeField] Transform attackPos;

    [Header("HP Bar Scaling")] [SerializeField]
    private float scaleAt1m = 0.01f;

    [SerializeField] private float minScale = 0.006f;
    [SerializeField] private float maxScale = 0.02f;
    [SerializeField] private float scaleSmooth = 10f;
    [SerializeField] private Image HPBar;

    [Header("Movement Settings")] [SerializeField]
    int roamDist;

    [Range(0, 10)] [SerializeField] float navCooldown;
    [Range(1, 20)] [SerializeField] int faceTargetSpeed;

    [Header("Detection Settings")] [Range(1, 20)] [SerializeField]
    float detectRange;

    [Range(5, 20)] [SerializeField] int sightDist;
    [Range(0, 360)] [SerializeField] int FOV;
    [SerializeField] LayerMask targetLayer;

    [Header("Damage Popup")] [SerializeField]
    private Transform popupAnchor;

    [SerializeField] private float popupRadius = 0.25f;
    [SerializeField] private float popupUpBias = 0.15f;

    [Header("Combat Settings")] [SerializeField]
    GameObject weapon;

    [Range(0.1f, 2f)] [SerializeField] float attackSpeed;
    [SerializeField] int HP;

    [Header("Drops")] [SerializeField] private GameObject[] dropPrefabs;
    [SerializeField] [Range(0f, 1f)] private float dropChance = 0.35f;
    [SerializeField] private float dropYOffset = 0.5f;

    [SerializeField] private GameObject stunVfxPrefab;
    [SerializeField] private GameObject blindVfxPrefab;
    [SerializeField] private GameObject knockbackVfxPrefab;
    [SerializeField] private Color blindTint;
    [SerializeField] private Color stunTint;

    [Header("Knockback Tuning")] [SerializeField]
    private float knockbackDuration = 0.12f;

    [SerializeField] private float knockbackNavmeshSnapRadius = 1.0f;

    float attackTimer;
    float navTimer;
    float playerDistance;
    Vector3 playerDir;
    Vector3 pointOrig;
    float origStopDist;
    Color colorOrig;
    private int hpOrig;
    private float displayHP;
    private Vector3 baseScale;
    private Camera cam;
    private bool isBlinded;
    private bool isStunned;
    private float blindUntil;
    private float stunUntil;

    private Coroutine blindRoutine;
    private Coroutine stunRoutine;
    private Coroutine knockbackRoutine;

    private GameObject activeBlindVfx;
    private GameObject activeStunVfx;


    void Awake()
    {
        colorOrig = models[0].material.color;
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

        if (isStunned)
        {
            attackTimer += Time.deltaTime;
            UpdateHPBarScale();
            return;
        }

        attackTimer += Time.deltaTime;

        playerDistance = Vector3.Distance(transform.position, gameManager.instance.player.transform.position);
        animator.SetBool("Movement", agent.velocity.magnitude > 0.01f);

        if (agent.enabled)
        {
            if (canSeePlayer())
            {
                // Logic handled inside canSeePlayer for chasing/attacking
            }
            else
            {
                checkRoam();
            }
        }

        UpdateHPBarScale();
    }

    bool canSeePlayer()
    {
        if (isBlinded) return false;

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

        if (!HPBar.gameObject.activeSelf)
            HPBar.gameObject.SetActive(true);

        SpawnDamagePopup(amount);

        if (HP <= 0)
        {
            WaveManager.instance.EnemiesDied();
            dropItem();
            gameManager.instance.exp++;
            HPBar.gameObject.SetActive(false);
            agent.enabled = false;
            GetComponentInChildren<CapsuleCollider>().enabled = false;
            enabled = false;
            animator.enabled = false;
            Destroy(gameObject, 5f);
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
        HPBar.rectTransform.localScale =
            Vector3.Lerp(HPBar.rectTransform.localScale, targetScale, scaleSmooth * Time.deltaTime);
    }

    public void Knockback(Vector3 dir, float strength)
    {
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) 
            dir = -transform.forward;
        dir.Normalize();

        SpawnDamagePopup(0, WeaponData.SpecialEffect.Knockback);
        
        SpawnOneShotVfx(knockbackVfxPrefab);

        if (knockbackRoutine != null)
            StopCoroutine(knockbackRoutine);

        knockbackRoutine = StartCoroutine(KnockbackRoutine(dir, strength));
    }

    private IEnumerator KnockbackRoutine(Vector3 dir, float strength)
    {
        bool wasEnabled = agent.enabled;
        if (wasEnabled)
        {
            agent.isStopped = true;
            agent.ResetPath();
            agent.enabled = false;
        }
        
        float time = 0f;
        Vector3 start = transform.position;
        Vector3 target = start + dir * strength;

        while (time < knockbackDuration)
        {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(start, target, time / knockbackDuration);
            yield return null;
        }
        
        if (wasEnabled)
        {
            agent.enabled = true;
            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, knockbackNavmeshSnapRadius, NavMesh.AllAreas))
                agent.Warp(hit.position);

            agent.isStopped = false;
        }

        knockbackRoutine = null;
    }

    public void Blind(float duration)
    {
        SpawnDamagePopup(0, WeaponData.SpecialEffect.Blind);
        
        blindUntil = Mathf.Max(blindUntil, Time.time + duration);

        if (blindRoutine == null)
            blindRoutine = StartCoroutine(BlindRoutine());
        
        activeBlindVfx = Instantiate(blindVfxPrefab, popupAnchor);

        ApplyTint(blindTint);
    }

    private IEnumerator BlindRoutine()
    {
        isBlinded = true;
        
        if (agent.enabled)
        {
            agent.ResetPath();
            agent.isStopped = false;
        }

        while (Time.time < blindUntil)
            yield return null;

        isBlinded = false;
        blindRoutine = null;

        if (activeBlindVfx != null) 
            Destroy(activeBlindVfx);
        activeBlindVfx = null;
        
        if (!isStunned)
            RestoreTint();
    }

    public void Stun(float duration)
    {
        SpawnDamagePopup(0, WeaponData.SpecialEffect.Stun);

        stunUntil = Mathf.Max(stunUntil, Time.time + duration);

        if (stunRoutine == null)
            stunRoutine = StartCoroutine(StunRoutine());

        if (stunVfxPrefab != null && activeStunVfx == null)
            activeStunVfx = Instantiate(stunVfxPrefab, popupAnchor);

        ApplyTint(stunTint);
    }

    private IEnumerator StunRoutine()
    {
        isStunned = true;
        
        if (agent.enabled)
        {
            agent.ResetPath();
            agent.isStopped = true;
        }
        
        if (animator != null)
            animator.speed = 0f;

        while (Time.time < stunUntil)
            yield return null;
        
        isStunned = false;
        stunRoutine = null;

        if (agent.enabled)
            agent.isStopped = false;

        if (animator != null)
            animator.speed = 1f;

        if (activeStunVfx != null) 
            Destroy(activeStunVfx);
        activeStunVfx = null;
        
        if (isBlinded)
            ApplyTint(blindTint);
        else
            RestoreTint();
    }


    private void SpawnOneShotVfx(GameObject prefab)
    {
        GameObject vfx = Instantiate(prefab, popupAnchor.position, Quaternion.identity);
        Destroy(vfx, 0.8f);
    }

    private void ApplyTint(Color tint)
    {
        foreach (Renderer render in models)
            render.material.color = tint;
    }

    private void RestoreTint()
    {
        foreach (Renderer render in models)
            render.material.color = colorOrig;
    }
}
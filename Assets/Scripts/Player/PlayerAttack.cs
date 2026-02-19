using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform attackPos;
    [SerializeField] private GameObject weaponVisual;
    [SerializeField] private Camera aimCamera;

    [Header("Targeting Masks")]
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private LayerMask obstructionMask = 0;

    [Header("Ranges / Feel")]
    // hard cap melee reach
    [SerializeField] private float maxTargetDistance = 3.0f;
    // forgiveness for thrust
    [SerializeField] private float thrustSphereRadius = 0.35f;

    [Header("Aim Assist")]
    [Tooltip("If swingArc <= this, treat weapon as thrust.")]
    [SerializeField] private float thrustArcThreshold = 25f;

    [Tooltip("SphereCast radius to pick reticle target for swings.")]
    [SerializeField] private float aimAssistRadius = 0.45f;

    [Tooltip("How far in front of camera the ray starts.")]
    [SerializeField] private float cameraRayStartOffset;

    [Tooltip("If true, require no wall between attacker and target.")]
    [SerializeField] private bool requireLineOfSight = true;

    [Header("Swing Cleave")]
    [SerializeField] private float cleaveRadius = 1.25f;
    [SerializeField] private int maxCleaveTargets = 2;

    [Header("Fallback Stats (if weapon damage is 0)")]
    [Range(0.1f, 2f)] [SerializeField] private float attackSpeed = 0.5f;
    [Range(1, 15)] [SerializeField] private int str = 5;

    private float attackTimer;
    private bool attacking;

    private PlayerStateMachine PSM;
    private int currentWeaponIndex;
    private float mouseScroll;
    private GameObject spawnedVfx;
    private RuntimeAnimatorController defaultAnimatorController;

    [HideInInspector] public WeaponData currentWeapon;

    // Getting self colliders for easy checking to prevent hitting self with
    // attacks or interactions with the weapon raycasts
    private Collider[] selfColliders;

    private void Start()
    {
        PSM = GetComponent<PlayerStateMachine>();
        if (PSM == null)
        {
            Debug.LogError("PlayerAttack requires a PlayerStateMachine component on the same GameObject.");
            enabled = false;
            return;
        }
        
        defaultAnimatorController = PSM.GetAnimator().runtimeAnimatorController;
        selfColliders = GetComponentsInChildren<Collider>(true);

        if (aimCamera == null)
            aimCamera = Camera.main;

        if (PSM.weapons != null && PSM.weapons.Count > 0)
        {
            currentWeaponIndex = 0;
            SwitchWeapon(PSM.weapons[currentWeaponIndex]);
        }
        else
        {
            Debug.LogWarning("No weapons assigned to PlayerAttack.");
        }
    }

    private void Update()
    {
        attackTimer += Time.deltaTime;

        mouseScroll = PSM.GetInput().Player.Scroll.ReadValue<Vector2>().y;
        bool attackPressed = gameManager.instance.playerScript.GetInput().Player.Attack.triggered;

        float cooldown = (currentWeapon != null && currentWeapon.totalTime > 0f)
            ? currentWeapon.totalTime
            : attackSpeed;

        if (attackPressed && attackTimer >= cooldown && !attacking)
        {
            StartCoroutine(AttackRoutine());
            AttackAudio();
        }

        if (mouseScroll != 0 && !attacking)
            ChangeWeapons();


        if (PSM.weapons.Count > 0)
        {
            SwitchWeapon(PSM.weapons[currentWeaponIndex]);
        }
    }

    private IEnumerator AttackRoutine()
    {
        if (currentWeapon == null)
            yield break;

        attacking = true;
        attackTimer = 0f;

        PSM.GetAnimator().SetTrigger("Attack");
        
        yield return new WaitForSeconds(currentWeapon.hitDelay);

        if (currentWeapon.weaponType == WeaponData.WeaponType.Melee)
            DoMeleeHit();
        else
            DoUtility();
        
        yield return new WaitForSeconds(currentWeapon.totalTime);

        attacking = false;
    }
    
    // For thrust weapons a single spherecast is done in the camera forward direction,
    // hitting what the player is aiming at (with some forgiveness).
    // ========================
    // For swing weapons, first try to pick a target based on the reticle (camera forward spherecast),
    // then fallback to a cone check if that fails. optional cleave can hit a few nearby enemies around the main target.
    private void DoMeleeHit()
    {
        int damageAmount = (currentWeapon.damage > 0) ? currentWeapon.damage : str;

        float weaponRange = Mathf.Clamp(currentWeapon.range, 0.1f, maxTargetDistance);
        float arc = Mathf.Clamp(currentWeapon.swingArc, 0f, 180f);
        bool isThrust = arc <= thrustArcThreshold;

        if (isThrust)
            TryThrustHit(damageAmount, weaponRange);
        else
            TrySwingHit(damageAmount, weaponRange, arc);
    }
    
    private void TryThrustHit(int damageAmount, float weaponRange)
    {
        if (aimCamera == null) return;

        Vector3 origin = aimCamera.transform.position + aimCamera.transform.forward * cameraRayStartOffset;
        Vector3 dir = aimCamera.transform.forward;
        float castDistance = weaponRange;

        if (requireLineOfSight && obstructionMask.value != 0)
        {
            // Did some more research on raycast performance, and it seems that doing a single
            // raycast before the spherecast to limit the distance is actually faster than just doing
            // the spherecast with a long distance and letting it check against walls as it goes. Who knew!
            // I also found out that QueryTriggerInteraction.Ignore makes the raycast ignore triggers,
            // which is perfect for our use case since we don't want the weapon raycast to be blocked by
            // trigger colliders (like the ones on loot orbs or spawn volumes).
            if (Physics.Raycast(origin, dir, out RaycastHit wallHit, castDistance, 
                    obstructionMask, QueryTriggerInteraction.Ignore)) 
                castDistance = wallHit.distance;
        }

        if (!Physics.SphereCast(origin, thrustSphereRadius, dir, out RaycastHit hit, castDistance, enemyMask,
                QueryTriggerInteraction.Ignore)) return;
        
        if (IsSelfCollider(hit.collider)) return;

        Vector3 playerOrigin = (attackPos != null) ? attackPos.position : transform.position + Vector3.up;
        float distToPlayer = Vector3.Distance(playerOrigin, hit.collider.bounds.center);
        if (distToPlayer > weaponRange + 0.35f) return;

        ApplyDamageAndEffects(hit.collider, damageAmount);
    }
    
    private void TrySwingHit(int damageAmount, float range, float arc)
    {
        Collider main = GetReticleTarget(range);
        if (main == null)
            main = GetFallbackConeTarget(range, arc);

        if (main == null) return;

        ApplyDamageAndEffects(main, damageAmount);
        
        if (cleaveRadius > 0.01f && maxCleaveTargets > 0)
            CleaveNearby(main, damageAmount, range);
    }

    private Collider GetReticleTarget(float range)
    {
        if (aimCamera == null) return null;

        Vector3 origin = aimCamera.transform.position + aimCamera.transform.forward * cameraRayStartOffset;
        Vector3 dir = aimCamera.transform.forward;
        float castDistance = range;

        if (requireLineOfSight && obstructionMask.value != 0)
        {
            if (Physics.Raycast(origin, dir, out RaycastHit wallHit, castDistance, 
                    obstructionMask, QueryTriggerInteraction.Ignore))
                castDistance = wallHit.distance;
        }

        if (!Physics.SphereCast(origin, aimAssistRadius, dir, out RaycastHit hit, castDistance,
                enemyMask, QueryTriggerInteraction.Ignore)) return null;
        if (IsSelfCollider(hit.collider)) return null;
        
        
        Vector3 playerOrigin = (attackPos != null) ? attackPos.position : transform.position + Vector3.up;
        float distToPlayer = Vector3.Distance(playerOrigin, hit.collider.bounds.center);

        return distToPlayer <= range + 0.35f ? hit.collider : null;
    }

    private Collider GetFallbackConeTarget(float range, float arc)
    {
        // This is a more expensive check, so only do it if the reticle targeting fails.
        // It checks for all enemies in a sphere around the player, then filters them by angle to create a cone,
        // and optionally checks line of sight as well.
        Vector3 origin = (attackPos != null) ? attackPos.position : transform.position + Vector3.up;
        Vector3 forward = (aimCamera != null ? aimCamera.transform.forward : transform.forward);
        // Ignore vertical difference for angle checks,
        // to make it more forgiving and work better with jumps and flying enemies
        forward.y = 0f;
        // If forward is almost zero (looking straight up or down), fallback to character forward.
        if (forward.sqrMagnitude < 0.001f) forward = transform.forward;
        // Normalization is important for consistent angle checks and to prevent errors if the
        // camera forward is very short on the horizontal plane.
        // It's possible for the camera forward to be very short on the horizontal plane if the
        // player is looking almost straight up or down, which would make the angle checks behave erratically.
        // By normalizing after zeroing out the y component, we ensure that we have a valid forward direction
        // for the cone checks.
        forward.Normalize();

        
        // The bottom block of code might be a bit confusing, but it's easier than it looks!
        // We’re selecting the "best" enemy to hit for a swing attack.
        // The idea is simple: among all enemies in range, pick the one the
        // player is most directly aiming at, not just the closest one.
        // =========================
        // FIRST: We convert the swing arc (in degrees) into a cosine threshold.
        // This lets us use a dot product to cheaply check if an enemy
        // is inside the swing cone in front of the player.
        // =========================
        // SECOND: Now we loop through all nearby enemy colliders and:
        // - Ignore ourselves
        // - Ignore anything out of range
        // - Flatten the Y axis so vertical differences don’t break aiming
        // - Use the dot product to see how closely the enemy lines up with the player’s forward direction
        // - Optionally require line of sight (no hitting through walls)
        // ==========================
        // THIRD: Of the valid targets, keep the one with the highest dot value.
        // This effectively chooses the enemy closest to the center of the player’s view / reticle.
        // ==========================
        // Result: swing attacks feel consistent and hit the enemy the player
        // is actually aiming at, instead of a random nearby one.
        Collider[] hits = Physics.OverlapSphere(origin, range, enemyMask, QueryTriggerInteraction.Ignore);
        if (hits == null || hits.Length == 0) return null;

        float cosThreshold = Mathf.Cos((arc * 0.5f) * Mathf.Deg2Rad);
        Collider best = null;
        float bestDot = -1f;

        foreach (Collider enemyCollider in hits)
        {
            if (enemyCollider == null || IsSelfCollider(enemyCollider)) continue;

            Vector3 to = enemyCollider.bounds.center - origin;
            float dist = to.magnitude;
            if (dist > range + 0.25f) continue;

            to.y = 0f;
            if (to.sqrMagnitude < 0.0001f) continue;

            float dot = Vector3.Dot(forward, to.normalized);
            if (dot < cosThreshold) continue;

            if (requireLineOfSight && obstructionMask.value != 0)
            {
                if (Physics.Raycast(origin, (enemyCollider.bounds.center - origin).normalized,
                        dist, obstructionMask, QueryTriggerInteraction.Ignore))
                    continue;
            }

            if (!(dot > bestDot)) continue;
            
            bestDot = dot;
            best = enemyCollider;
        }

        return best;
    }

    private void CleaveNearby(Collider main, int damageAmount, float playerRange)
    {
        Vector3 origin = (attackPos != null) ? attackPos.position : transform.position + Vector3.up;
        Vector3 center = main.bounds.center;
        Collider[] hits = Physics.OverlapSphere(center, cleaveRadius, enemyMask, QueryTriggerInteraction.Ignore);

        int cleaved = 0;

        foreach (Collider c in hits)
        {
            if (c == null || c == main || IsSelfCollider(c)) continue;
            
            if (Vector3.Distance(origin, c.bounds.center) > playerRange + 0.35f)
                continue;

            if (requireLineOfSight && obstructionMask.value != 0)
            {
                float dist = Vector3.Distance(origin, c.bounds.center);
                if (Physics.Raycast(origin, (c.bounds.center - origin).normalized, dist, obstructionMask, QueryTriggerInteraction.Ignore))
                    continue;
            }

            ApplyDamageAndEffects(c, damageAmount);
            cleaved++;

            if (cleaved >= maxCleaveTargets)
                break;
        }
    }
    
    private void DoUtility()
    {
        if (aimCamera == null) return;

        float range = Mathf.Max(0.1f, currentWeapon.range);
        float arc = Mathf.Clamp(currentWeapon.swingArc, 0f, 180f);
        Vector3 origin = aimCamera.transform.position + aimCamera.transform.forward * cameraRayStartOffset;

        Vector3 forward = aimCamera.transform.forward;
        forward.y = 0f;
        if (forward.sqrMagnitude < 0.001f) forward = transform.forward;
        forward.Normalize();

        Collider[] hits = Physics.OverlapSphere(transform.position, range, enemyMask, QueryTriggerInteraction.Ignore);
        if (hits == null || hits.Length == 0) return;

        float cosThreshold = Mathf.Cos((arc * 0.5f) * Mathf.Deg2Rad);

        foreach (Collider c in hits)
        {
            if (c == null || IsSelfCollider(c)) continue;

            Vector3 toTarget = c.bounds.center - origin;
            float dist = toTarget.magnitude;
            if (dist > range) continue;

            toTarget.y = 0f;
            if (toTarget.sqrMagnitude < 0.0001f) continue;

            float dot = Vector3.Dot(forward, toTarget.normalized);
            if (dot < cosThreshold) continue;

            if (requireLineOfSight && obstructionMask.value != 0)
            {
                if (Physics.Raycast(origin, (c.bounds.center - origin).normalized, dist, obstructionMask, QueryTriggerInteraction.Ignore))
                    continue;
            }

            ApplySpecial(c);
        }
    }
    
    private void ApplyDamageAndEffects(Collider target, int damageAmount)
    {
        // TODO: Add hit effects here (particles, sounds, etc.)
        IDamage dmg = target.GetComponent<IDamage>();
        if (dmg != null)
            dmg.takeDamage(damageAmount);

        ApplySpecial(target);
        //SoundManager.PlaySound(Choose sound from Enum for what you want);  Can add more sounds if needed.
    }

    private void ApplySpecial(Collider target)
    {
        if (currentWeapon == null) return;
        if (currentWeapon.specialEffect == WeaponData.SpecialEffect.None) return;
        if (currentWeapon.specialChance > 0f && Random.value > currentWeapon.specialChance) return;

        switch (currentWeapon.specialEffect)
        {
            case WeaponData.SpecialEffect.Stun:
            {
                var stun = target.GetComponent<IStunnable>();
                if (stun != null) 
                    stun.Stun(currentWeapon.specialDuration);
                break;
            }
            case WeaponData.SpecialEffect.Blind:
            {
                var blind = target.GetComponent<IBlindable>();
                if (blind != null) 
                    blind.Blind(currentWeapon.specialDuration);
                break;
            }
            case WeaponData.SpecialEffect.Knockback:
            {
                var knockback = target.GetComponent<IKnockbackable>();
                if (knockback != null)
                {
                    Vector3 dir = (target.bounds.center - transform.position);
                    dir.y = 0f;
                    if (dir.sqrMagnitude < 0.001f) dir = transform.forward;
                    knockback.Knockback(dir.normalized, 1f);
                }
                break;
            }
            case WeaponData.SpecialEffect.None:
                break;
        }
    }
    
    private bool IsSelfCollider(Collider c)
    {
        foreach (var self in selfColliders)
            if (self == c) return true;

        return false;
    }

    private void AttackAudio()
    {
        SoundManager.PlaySound(SoundType.Axe);
    }

   /* private IEnumerator DelayedAttackAudio()
    {
        yield return new WaitForSeconds(0.15f);

        if (PSM.audHit[0] != null)
            PSM.aud.PlayOneShot(PSM.audHit[0], PSM.volume);
    }*/

    private void ChangeWeapons()
    {
        switch (mouseScroll)
        {
            case > 0 when currentWeaponIndex <PSM.weapons.Count - 1:
                currentWeaponIndex++;
                SwitchWeapon(PSM.weapons[currentWeaponIndex]);
                break;
            case < 0 when currentWeaponIndex > 0:
                currentWeaponIndex--;
                SwitchWeapon(PSM.weapons[currentWeaponIndex]);
                break;
        }
    }

    private void SwitchWeapon(WeaponData newWeapon)
    {
        currentWeapon = newWeapon;

        PSM.GetAnimator().runtimeAnimatorController = newWeapon.animatorOverride != null
            ? newWeapon.animatorOverride
            : defaultAnimatorController;

        MeshFilter newFilter = newWeapon.weaponModel.GetComponent<MeshFilter>() ??
                               newWeapon.weaponModel.GetComponentInChildren<MeshFilter>();
        MeshRenderer newRenderer = newWeapon.weaponModel.GetComponent<MeshRenderer>() ??
                                   newWeapon.weaponModel.GetComponentInChildren<MeshRenderer>();

        MeshFilter curFilter = weaponVisual.GetComponent<MeshFilter>();
        MeshRenderer curRenderer = weaponVisual.GetComponent<MeshRenderer>();

        curFilter.sharedMesh = newFilter.sharedMesh;
        curRenderer.sharedMaterials = newRenderer.sharedMaterials;
        weaponVisual.gameObject.transform.localPosition = new Vector3(0f, 0f, newWeapon.zOffset);

        if (newWeapon.outLineMaterial != null)
        {
            var baseMaterials = newRenderer.sharedMaterials;

            Material[] combinedMaterials = new Material[baseMaterials.Length + 1];

            for (int i = 0; i < baseMaterials.Length; i++)
            {
                combinedMaterials[i] = baseMaterials[i];
            }

            combinedMaterials[combinedMaterials.Length - 1] = newWeapon.outLineMaterial;

            curRenderer.sharedMaterials = combinedMaterials;
        }

        if (newWeapon.optionalScale > 0)
            weaponVisual.transform.localScale = Vector3.one * newWeapon.optionalScale;
        else
            weaponVisual.transform.localScale = Vector3.one * 100f;
        
        if (spawnedVfx != null)
        {
            Destroy(spawnedVfx);
            spawnedVfx = null;
        }
        
        ParticleSystem psPrefab = newWeapon.weaponModel.GetComponentInChildren<ParticleSystem>();
        
        if (psPrefab == null)
            return;

        spawnedVfx = Instantiate(psPrefab.gameObject, weaponVisual.transform);
        
        spawnedVfx.transform.localPosition = psPrefab.transform.localPosition;
        spawnedVfx.transform.localRotation = psPrefab.transform.localRotation;
        spawnedVfx.transform.localScale = psPrefab.transform.localScale;
        spawnedVfx.layer = LayerMask.NameToLayer("Player");
        
        var spawnedPs = spawnedVfx.GetComponentInChildren<ParticleSystem>(true);
        if (spawnedPs != null)
        {
            spawnedPs.Clear(true);
            spawnedPs.Play(true);
        }
    }
}

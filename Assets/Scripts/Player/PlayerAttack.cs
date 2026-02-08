using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform AttackPos;
    [SerializeField] private GameObject weapon; 
    [SerializeField] private damage meleeHitbox;

    [Header("Fallback Stats")]
    [Range(0.1f, 2)] [SerializeField] private float attackSpeed = 0.5f;
    [Range(1, 15)] [SerializeField] private int str = 5;

    private float attackTimer;
    private bool attacking;

    private PlayerStateMachine PSM;
    public List<WeaponData> weapons;
    private int currentWeaponIndex;
    private float mouseScroll;
    private RuntimeAnimatorController defaultAnimatorController;

    [HideInInspector] public WeaponData currentWeapon;

    private void Start()
    {
        PSM = GetComponent<PlayerStateMachine>();
        defaultAnimatorController = PSM.GetAnimator().runtimeAnimatorController;

        if (weapons != null && weapons.Count > 0)
        {
            currentWeaponIndex = 0;
            SwitchWeapon(weapons[currentWeaponIndex]);
        }
        else
        {
            Debug.LogWarning("No weapons assigned to PlayerAttack script.");
        }

        // Safety: melee hitbox should start disabled for damaging
        if (meleeHitbox != null)
        {
            meleeHitbox.type = damage.damageType.melee;
            meleeHitbox.allowedToDamage = false;
        }
    }

    private void Update()
    {
        attackTimer += Time.deltaTime;
        mouseScroll = PSM.GetInput().Player.Scroll.ReadValue<Vector2>().y;

        bool attackPressed = gameManager.instance.playerScript.GetInput().Player.Attack.triggered;
        float cooldown = attackSpeed;
        
        if (currentWeapon != null &&
            currentWeapon.weaponType == WeaponData.WeaponType.Ranged &&
            currentWeapon.fireRate > 0f)
        {
            cooldown = 1f / currentWeapon.fireRate;
        }

        if (attackPressed && attackTimer >= cooldown && !attacking)
        {
            StartCoroutine(currentWeapon.weaponType == WeaponData.WeaponType.Ranged
                ? RangedAttackRoutine()
                : MeleeAttackRoutine());

            AttackAudio();
        }

        if (mouseScroll != 0 && !attacking)
            ChangeWeapons();
    }

    IEnumerator MeleeAttackRoutine()
    {
        attacking = true;
        attackTimer = 0f;
        
        PSM.GetAnimator().SetTrigger("Attack");
        
        meleeHitbox.type = damage.damageType.melee;
        int amount = currentWeapon.damage > 0 ? currentWeapon.damage : str;
        meleeHitbox.SetDamage(amount);
        
        yield return new WaitForSeconds(currentWeapon.meleeHitDelay);

        // open hit window
        meleeHitbox.allowedToDamage = true;
        yield return new WaitForSeconds(0.5f);
        meleeHitbox.allowedToDamage = false;

        // lockout
        yield return new WaitForSeconds(currentWeapon.meleeTotalTime);
        attacking = false;
    }
    
    IEnumerator RangedAttackRoutine()
    {
        attacking = true;
        attackTimer = 0f;

        PSM.GetAnimator().SetTrigger("Attack");

        if (currentWeapon.shootDelay > 0f)
            yield return new WaitForSeconds(currentWeapon.shootDelay);

        FireRanged();

        yield return new WaitForSeconds(0.05f);
        attacking = false;
    }

    void FireRanged()
    {
        Transform muzzle = AttackPos != null ? AttackPos : transform;
        
        Vector3 dir = muzzle.forward;
        dir = ApplySpread(dir, currentWeapon.spreadDegrees);

        int amount = currentWeapon.damage > 0 ? currentWeapon.damage : str;

        if (currentWeapon.useHitscan)
        {
            if (Physics.Raycast(muzzle.position, dir, out RaycastHit hit, 
                    currentWeapon.range, currentWeapon.hitMask, QueryTriggerInteraction.Ignore))
            {
                IDamage dmgTarget = hit.collider.GetComponent<IDamage>();
                if (dmgTarget != null)
                    dmgTarget.takeDamage(amount);
            }
        }
        else
        {
            if (currentWeapon.projectilePrefab == null)
            {
                Debug.LogWarning($"{currentWeapon.weaponName} uses projectile but projectilePrefab is NULL.");
                return;
            }

            Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
            GameObject proj = Instantiate(currentWeapon.projectilePrefab, muzzle.position, rot);

            damage projDamage = proj.GetComponent<damage>();
            if (projDamage != null)
            {
                projDamage.type = damage.damageType.ranged;
                projDamage.SetDamage(amount);
                projDamage.SetRangedStats(
                    currentWeapon.projectileSpeed,
                    currentWeapon.projectileGravity,
                    currentWeapon.projectileVerticalOffset,
                    currentWeapon.projectileLife
                );
            }
        }
    }

    Vector3 ApplySpread(Vector3 direction, float spreadDegrees)
    {
        if (spreadDegrees <= 0f) return direction;

        float yaw = Random.Range(-spreadDegrees, spreadDegrees);
        float pitch = Random.Range(-spreadDegrees, spreadDegrees);

        return Quaternion.Euler(pitch, yaw, 0f) * direction;
    }
    
    void AttackAudio()
    {
        StartCoroutine(DelayedAttackAudio());
    }

    IEnumerator DelayedAttackAudio()
    {
        yield return new WaitForSeconds(0.15f);

        if (PSM != null && PSM.aud != null && PSM.audHit.Length > 0 && PSM.audHit[0] != null)
            PSM.aud.PlayOneShot(PSM.audHit[0], PSM.volume);
        else
            Debug.LogError("Attack Audio failed — missing PSM, aud, or audio clip");
    }
    
    private void ChangeWeapons()
    {
        switch (mouseScroll)
        {
            case > 0 when currentWeaponIndex < weapons.Count - 1:
                currentWeaponIndex++;
                SwitchWeapon(weapons[currentWeaponIndex]);
                break;
            case < 0 when currentWeaponIndex > 0:
                currentWeaponIndex--;
                SwitchWeapon(weapons[currentWeaponIndex]);
                break;
        }
    }

    private void SwitchWeapon(WeaponData newWeapon)
    {
        currentWeapon = newWeapon;

        PSM.GetAnimator().runtimeAnimatorController = newWeapon.animatorOverride != null
            ? newWeapon.animatorOverride
            : defaultAnimatorController;

        MeshFilter newFilter = newWeapon.weaponModel.GetComponent<MeshFilter>();
        MeshRenderer newRenderer = newWeapon.weaponModel.GetComponent<MeshRenderer>();

        MeshFilter curFilter = weapon.GetComponent<MeshFilter>();
        MeshRenderer curRenderer = weapon.GetComponent<MeshRenderer>();

        curFilter.sharedMesh = newFilter.sharedMesh;
        curRenderer.sharedMaterial = newRenderer.sharedMaterial;
    }
}

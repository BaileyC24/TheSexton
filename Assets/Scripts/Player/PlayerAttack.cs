using System;
using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Transform AttackPos;
    [Range(0.1f, 2)][SerializeField] private float attackSpeed;
    [SerializeField] private Color damageColor;
    private float attackTimer;
    private bool attacking;
    private readonly int AttackName = Animator.StringToHash("Attack");
    private AttackTrigger attackTrigger;

    private void Start()
    {
        attackTrigger = GetComponentInChildren<AttackTrigger>();
        attackTrigger.ATKTRIGGER += CheckAttack;
    }

    private void Update()
    {
        attackTimer += Time.deltaTime;

        if (gameManager.instance.playerScript.GetInput().Player.Attack.triggered
            && attackTimer >= attackSpeed && !attacking)
        {
            Attack();
        }
    }

    void Attack()
    {
        StartCoroutine(AttackWindow());
    }

    void CheckAttack(Collider _target)
    {
        if (!attacking || !_target.CompareTag("Enemy")) return;
        
        IDamage damage = _target.GetComponent<IDamage>();
        if (damage != null)
            damage.takeDamage(1);
    }

    IEnumerator AttackWindow()
    {
        attacking = true;
        attackTimer = 0f;
        gameManager.instance.playerScript.GetAnimator().SetTrigger(AttackName);
        yield return new WaitForSeconds(gameManager.instance.playerScript.GetAnimator().GetCurrentAnimatorClipInfo(0).Length);
        attacking = false;
    }
}
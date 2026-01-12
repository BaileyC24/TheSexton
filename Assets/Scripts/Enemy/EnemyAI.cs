using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer model;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform meleePos;

    [Range (1,10)][SerializeField] int HP;
    [Range(1, 20)][SerializeField] int faceTargetSpeed;

    [SerializeField] GameObject weapon;
    [Range((float)0.1, 2)][SerializeField] float attackSpeed;

    Color colorOrig;

    float shootTimer;

    Vector3 playerDir;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrig = model.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        shootTimer += Time.deltaTime;

        playerDir = (gameManager.instance.player.transform.position - transform.position);

        //TODO get player state to determine if player is moving. 
        //if moving set agent destination towards player. when player stops moving set destination to
    }
    public void takeDamage(int amount)
    {
        HP -= amount;
    }
}

using System.Net;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPolymorphable : MonoBehaviour, IPolymorphable
{

    [SerializeField] public ParticleSystem magicEffect;
    public void Polymorph(float duration)
    {
        //change this thing I hit to be this other thing (or one of these other things)
        if (gameObject.tag == "Enemy")  //we may also need to disable EnemyAI or change it's state...??
        {
          if (magicEffect != null)
            { 
                    magicEffect.Play();
            }
            gameObject.tag = "Ally";
        }
    }

    void Update() //to check the tag
    {  //right now this updates every frame
        EnemyAI enemyScript = GetComponent<EnemyAI>(); //a MonoBehavior script name is a class, which is a data type
        AllyAI allyScript = GetComponent<AllyAI>();
        NavMeshAgent agent = GetComponent<NavMeshAgent>();

        if (gameObject.tag == "Ally")//if the tag is "Ally"
        {
            if (allyScript == null)//check to see if already AllyAI script
            {  //only if there isn't already an AllyAI script
               
                Destroy(GetComponent<EnemyAI>());  //remove the EnemyAI script  
                gameObject.AddComponent<AllyAI>(); //add the AllyAI script  

                //if (agent == null)       _This is now handled in AllyAI OnEnable
                //{
                //    agent = GetComponent<NavMeshAgent>();
                //}
            }
        }
        //         ...could also do a vsn with disable and reenable component (coroutine)

    }
}

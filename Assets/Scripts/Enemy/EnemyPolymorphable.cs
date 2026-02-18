using UnityEngine;

public class EnemyPolymorphable : MonoBehaviour, IPolymorphable
{
    public void Polymorph(float duration)
    {
        //change this thing I hit to be this other thing (or one of these other things)
        if (gameObject.tag == "Enemy")  //we may also need to disable EnemyAI or change it's state...??
        {
            gameObject.tag = "Ally";
        }
    }
}

using UnityEngine;

public class EnemyAlterable : MonoBehaviour, IAlterable
{
    public void Alter(float duration)
    {
        //change this thing I hit to be invisible for X amount of time
       // if (gameObject.tag == "Enemy")  _not needeed because we're already in the game object
      //  {
            SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer renderer in renderers)
            {
                renderer.enabled = false; //turn of the renderer for each body part in turn
            }

            StartCoroutine(AlterRoutine(duration, renderers));
       // }
    }


private System.Collections.IEnumerator AlterRoutine(float duration, SkinnedMeshRenderer[] renderers) //can do this or using System.Collections at the top
{
    yield return new WaitForSeconds(duration);
    foreach (SkinnedMeshRenderer renderer in renderers)
        {
            renderer.enabled = true; //turn of the renderer for each body part in turn
        }
    }

}
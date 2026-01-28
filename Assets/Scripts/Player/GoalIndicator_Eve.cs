using UnityEngine;

public class GoalIndicator_Eve : MonoBehaviour
{
    [SerializeField] Transform winZone;

    void OnDrawGizmos()  //only runs in editor for visual debugging
    {
        if (winZone == null) return; //if we didn't assign the WinZone, don't crash, just do nothing

        Gizmos.color = Color.green;  //make debug visuals green
        Gizmos.DrawLine(transform.position, winZone.position); //draws line from player to level goal
        Gizmos.DrawSphere(winZone.position, 1f); //draws visible marker at goal location
    }
}

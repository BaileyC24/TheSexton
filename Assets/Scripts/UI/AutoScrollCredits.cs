using UnityEngine;
using UnityEngine.UI;

public class AutoScrollCredits : MonoBehaviour
{

    [SerializeField] RectTransform creditScroller; //the UI element we are moving
    [SerializeField] private float scrollSpeed; //how fast we want it to move

    private ScrollRect scrollRect;

    // Update is called once per frame
    private void Update()
    {
        Debug.Log("The script is running! TimeScale is:" + Time.timeScale);
        creditScroller.anchoredPosition += new Vector2(0f, scrollSpeed * Time.deltaTime);
        //take the current position of the scroller and add to it only a Y direction of our desired speed times the length of time it takes to run a frame (to smooth the scroll across different computers)
    }
}

using UnityEngine;

public class CreditsMoveUp : MonoBehaviour
{
    [SerializeField] private RectTransform content; //what we physically move upward
    [SerializeField] private float pixelsPerSecond = 50f;
    [SerializeField] private float startY = -500f; //starting point (below viewport)
    [SerializeField] private float resetY = 500f; //where it resets (above viewport)

    private void OnEnable()
    {
        if (content == null) content = (RectTransform)transform;
        content.anchoredPosition = new Vector2(content.anchoredPosition.x, startY);
    }

    // Update is called once per frame
    private void Update()
    {
        if (content == null) return;
        content.anchoredPosition += Vector2.up * pixelsPerSecond * Time.unscaledDeltaTime;

        if (content.anchoredPosition.y >= resetY)
            content.anchoredPosition = new Vector2(content.anchoredPosition.x, startY);

    }
}

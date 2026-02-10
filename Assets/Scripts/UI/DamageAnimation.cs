using System;
using TMPro;
using UnityEngine;

public class DamageAnimation : MonoBehaviour
{
    [SerializeField] private AnimationCurve opacityCurve;
    [SerializeField] private AnimationCurve scaleCurve;
    [SerializeField] private AnimationCurve heightCurve;
    [SerializeField] public Color damageColor;

    private TextMeshProUGUI damageText;
    private float time;
    private void Awake()
    {
        damageText = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Update()
    {
        Color currentColor = damageColor;
        currentColor.a = opacityCurve.Evaluate(time);
        damageText.color = currentColor;

        float scale = scaleCurve.Evaluate(time);
        transform.localScale = Vector3.one * scale;

        float height = heightCurve.Evaluate(time);
        damageText.transform.localPosition = new Vector3(0, height, 0);
        
        time += Time.deltaTime;
    }
}
using System;
using TMPro;
using UnityEngine;

public class DamageManager : MonoBehaviour
{
    public static DamageManager instance;
    [SerializeField] private GameObject prefabDamage;

    private void Awake()
    {
        instance = this;
    }


    public void CreatePopup(Vector3 position, string damageAmount, Color CustomColor = default)
    {
        GameObject damagePopup = Instantiate(prefabDamage, position, Quaternion.identity);
        damagePopup.GetComponentInChildren<TextMeshProUGUI>().text = damageAmount;
        if (CustomColor != default)
        {
            damagePopup.GetComponent<DamageAnimation>().damageColor = CustomColor;
            damagePopup.GetComponentInChildren<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
        }

        Destroy(damagePopup, 1f);
    }
}

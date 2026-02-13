using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour, IPointerClickHandler
{
    [Header("UI")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI stackText;

    public int SlotIndex { get; private set; }

    public void Init(int slotIndex)
    {
        SlotIndex = slotIndex;
        SetEmpty();
    }

    public void SetEmpty()
    {
        if (icon != null)
        {
            icon.enabled = false;
            icon.sprite = null;
        }

        if (stackText != null)
            stackText.text = "";
    }

    public void Set(Sprite sprite, int amount)
    {
        if (icon != null)
        {
            icon.enabled = sprite != null;
            icon.sprite = sprite;
        }

        if (stackText != null)
            stackText.text = (amount > 1) ? amount.ToString() : "";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            InventoryManager.instance.OnSlotLeftClick(SlotIndex);
    }
}
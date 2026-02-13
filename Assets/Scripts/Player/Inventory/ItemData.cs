using UnityEngine;

public enum ItemType
{
    Consumable,
    QuestItem
}

[CreateAssetMenu(fileName = "New Item", menuName = "The Sexton/Item Data")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;

    public bool isStackable = true;
    public int maxStackSize = 10;

    public ItemType type = ItemType.Consumable;
}
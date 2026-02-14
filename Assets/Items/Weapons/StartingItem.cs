using UnityEngine;

[System.Serializable]
public class StartingItem
{
    public ItemData item;
    [Min(1)] public int amount;

    public StartingItem(ItemData item, int amount)
    {
        this.item = item;
        this.amount = amount;
    }
}
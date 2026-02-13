using UnityEngine;

[System.Serializable]
public class StartingItem
{
    public ItemData item;
    [Min(1)] public int amount = 1;
}
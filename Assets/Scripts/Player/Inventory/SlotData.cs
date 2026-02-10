using UnityEngine;

[System.Serializable]
public class SlotData
{
    public ItemData item;
    public WeaponData weapon;
    public int amount;

    public bool IsEmpty => item == null && weapon == null;

    public void Clear()
    {
        item = null;
        weapon = null;
        amount = 0;
    }

    public Sprite GetIcon()
    {
        if (weapon != null) 
            return weapon.weaponIcon;
        
        return item != null ? item.itemIcon : null;
    }
}
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Weapon", menuName = "The Sexton/Active Player Data (ONLY FILE)")]
public class PlayerActiveData : ScriptableObject
{
    public List<StartingItem> items;
    public List<WeaponData> weapons;
    public int coins;
    public CharacterData currentCharacter;

    public void Clear()
    {
        items.Clear();
        weapons.Clear();
        coins = 0;
    }

    public void SaveData(InventoryManager inventoryManager)
    {
        foreach (SlotData slot in inventoryManager.slotList)
        {
            if (slot.weapon != null)
                weapons.Add(slot.weapon);
            else
                items.Add(new StartingItem(slot.item, slot.amount));
        }
        
        if (inventoryManager.PrimaryWeapon != null)
            weapons.Add(inventoryManager.PrimaryWeapon);
        if (inventoryManager.SecondaryWeapon != null)
            weapons.Add(inventoryManager.SecondaryWeapon);
    }

    public void LoadData(InventoryManager inventoryManager)
    {
        if (items.Count > 0 || weapons.Count > 0)
        {
            inventoryManager.AddStartingItems(items, weapons);
            return;
        }
        
        inventoryManager.AddStartingItems(currentCharacter.startingItems, currentCharacter.startingWeapons);
    }
}
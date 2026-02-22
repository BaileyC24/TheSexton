using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Weapon", menuName = "The Sexton/Active Player Data (ONLY FILE)")]
public class PlayerActiveData : ScriptableObject
{
    public List<StartingItem> items;
    public List<WeaponData> weapons;
    public int coins;
    public CharacterData currentCharacter;

    [Header("Upgrades")] 
    public int damageUpgrade;
    public float atkSpeedUpgrade;
    public int healthUpgrade;
    
    public void Clear()
    {
        items.Clear();

        foreach (WeaponData weaponData in weapons)
        {
            weaponData.effectChanceUpgrade = 0;
        }
        
        weapons.Clear();
        damageUpgrade = 0;
        atkSpeedUpgrade = 0;
        healthUpgrade = 0;
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
        
        if (inventoryManager.coinsOnHand > 0)
            coins = inventoryManager.coinsOnHand;
    }

    public void LoadData(InventoryManager inventoryManager)
    {
        int startingCoins = coins > 0 ? coins : currentCharacter.startingCoins;
        List<StartingItem> startingItems = items.Count > 0 ? items : currentCharacter.startingItems;
        List<WeaponData> startingWeapons = weapons.Count > 0 ? weapons : currentCharacter.startingWeapons;
        
        inventoryManager.AddStartingItems(startingItems, startingWeapons, startingCoins);
    }
}
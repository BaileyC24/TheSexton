using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    [Header("UI Setup")]
    [SerializeField] private Transform inventoryContent;
    [SerializeField] private InventorySlotUI slotPrefab;
    [SerializeField] private int slotCount = 24;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI strText;
    [SerializeField] private TextMeshProUGUI atkSpeedText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private GameObject useMenu;
    [SerializeField] private TextMeshProUGUI useText;
    [SerializeField] private TextMeshProUGUI itemText;
    [SerializeField] private TextMeshProUGUI coinsText;

    [Header("Equipped Weapons (UI optional)")]
    [SerializeField] private InventorySlotUI primaryWeaponSlotUI;
    [SerializeField] private InventorySlotUI secondaryWeaponSlotUI;
    public WeaponData PrimaryWeapon { get; private set; }
    public WeaponData SecondaryWeapon { get; private set; }
    public List<SlotData> slotList = new();
    private List<InventorySlotUI> slotUIs = new();
    private int selectedIndex = -1;
    [HideInInspector] public int coinsOnHand;
    
    private void Awake()
    {
        instance = this;
        BuildSlots();
        RefreshUI();
    }

    private void Update()
    {
        levelText.text = gameManager.instance.level.ToString("F0") + "/" + gameManager.instance.maxLevel.ToString("F0");
        strText.text = (gameManager.instance.playerStats.currentWeapon.damage + gameManager.instance.currentPlayerData.damageUpgrade).ToString("F0");
        atkSpeedText.text = (gameManager.instance.playerStats.currentWeapon.totalTime - gameManager.instance.currentPlayerData.atkSpeedUpgrade).ToString("F2");
        healthText.text = (gameManager.instance.playerScript.health) + "/" + (gameManager.instance.playerScript.HPOrig + gameManager.instance.currentPlayerData.healthUpgrade);
        coinsText.text = coinsOnHand.ToString();
    }

    private void BuildSlots()
    {
        primaryWeaponSlotUI.Init(30);
        secondaryWeaponSlotUI.Init(31);
        
        for (int i = 0; i < slotCount; i++)
            slotList.Add(new SlotData());
        
        for (int i = 0; i < slotCount; i++)
        {
            InventorySlotUI ui = Instantiate(slotPrefab, inventoryContent);
            ui.Init(i);
            slotUIs.Add(ui);
            slotList[i].transform = ui.transform;
        }
    }

    public bool AddItem(ItemData item, int amount = 1)
    {
        if (item == null || amount <= 0) return false;
        
        if (item.isStackable)
        {
            foreach (SlotData currentSlot in slotList)
            {
                if (currentSlot.item != item || currentSlot.weapon != null ||
                    currentSlot.amount >= item.maxStackSize) continue;
                
                int space = item.maxStackSize - currentSlot.amount;
                int add = Mathf.Min(space, amount);
                currentSlot.amount += add;
                amount -= add;

                if (amount > 0) continue;
                
                RefreshUI();
                return true;
            }
        }
        
        while (amount > 0)
        {
            int emptyIndex = FindEmptySlot();
            if (emptyIndex == -1)
            {
                RefreshUI();
                // TODO: feedback for "inventory full" (UI + sound)
                return false;
            }

            SlotData currentSlot = slotList[emptyIndex];
            currentSlot.weapon = null;
            currentSlot.item = item;

            if (item.isStackable)
            {
                int add = Mathf.Min(item.maxStackSize, amount);
                currentSlot.amount = add;
                amount -= add;
            }
            else
            {
                currentSlot.amount = 1;
                amount -= 1;
            }
        }

        RefreshUI();
        return true;
    }

    public int AddWeapon(WeaponData weapon)
    {
        if (weapon == null)
            return -1;

        int emptyIndex = FindEmptySlot();
        if (emptyIndex == -1) 
            return -1;

        SlotData currentSlot = slotList[emptyIndex];
        currentSlot.Clear();
        currentSlot.weapon = weapon;
        currentSlot.amount = 1;

        RefreshUI();
        return emptyIndex;
    }

    public bool EquipPrimaryFromSlot(int slotIndex)
    {
        if (!IsValidIndex(slotIndex)) return false;

        SlotData currentSlot = slotList[slotIndex];
        if (currentSlot.weapon == null) return false;
        
        WeaponData old = PrimaryWeapon;
        PrimaryWeapon = currentSlot.weapon;

        if (old != null)
        {
            currentSlot.weapon = old;
            currentSlot.amount = 1;
        }
        else
        {
            currentSlot.Clear();
        }

        RefreshUI();
        return true;
    }

    public bool EquipSecondaryFromSlot(int slotIndex)
    {
        if (!IsValidIndex(slotIndex)) return false;

        SlotData currentSlot = slotList[slotIndex];
        if (currentSlot.weapon == null) return false;

        WeaponData old = SecondaryWeapon;
        SecondaryWeapon = currentSlot.weapon;

        if (old != null)
        {
            currentSlot.weapon = old;
            currentSlot.amount = 1;
        }
        else
        {
            currentSlot.Clear();
        }

        RefreshUI();
        return true;
    }
    

    public bool UseItemInSlot(int slotIndex)
    {
        if (!IsValidIndex(slotIndex)) return false;

        SlotData currentSlot = slotList[slotIndex];

        if (currentSlot.weapon != null)
        {
            bool equipped = currentSlot.weapon.isWeaponPrimary
                ? EquipPrimaryFromSlot(slotIndex)
                : EquipSecondaryFromSlot(slotIndex);
            SyncWeaponsToPlayer();
            return equipped;
        }

        if (currentSlot.item == null) return false;
        
        // TODO: this is where you would trigger the item's effect (healing, buff, etc)
        if (currentSlot.item.type == ItemType.Consumable)
        {
            if (currentSlot.item.isHealing)
            {
                gameManager.instance.playerScript.heal(currentSlot.item.healingAmount);
            }

            if (currentSlot.item.isPlaceable)
            {
                bool grounded = Physics.Raycast(
                    gameManager.instance.playerScript.feetPos.position + Vector3.up * 0.1f,
                    Vector3.down,
                    out RaycastHit hit,
                    1.5f,
                    ~LayerMask.GetMask("Player"),
                    QueryTriggerInteraction.Ignore
                );

                if (!grounded) return false;

                Instantiate(currentSlot.item.itemPrefab, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
            }
            
            RemoveFromSlot(slotIndex, 1);
            return true;
        }
        
        return false;
    }

    public void RemoveFromSlot(int slotIndex, int amount)
    {
        if (!IsValidIndex(slotIndex)) 
            return;

        SlotData currentSlot = slotList[slotIndex];
        if (currentSlot.IsEmpty) 
            return;
        
        if (currentSlot.weapon != null)
        {
            currentSlot.Clear();
            RefreshUI();
            return;
        }
        
        currentSlot.amount -= amount;
        if (currentSlot.amount <= 0)
            currentSlot.Clear();

        RefreshUI();
    }
    

    private void RefreshUI()
    {
        for (int i = 0; i < slotList.Count && i < slotUIs.Count; i++)
        {
            SlotData currentSlot = slotList[i];
            if (currentSlot.IsEmpty) 
                slotUIs[i].SetEmpty();
            else 
                slotUIs[i].Set(currentSlot.GetIcon(), currentSlot.amount);
        }
        
        if (primaryWeaponSlotUI != null)
        {
            if (PrimaryWeapon == null) 
                primaryWeaponSlotUI.SetEmpty();
            else 
                primaryWeaponSlotUI.Set(PrimaryWeapon.weaponIcon, 1);
        }

        if (secondaryWeaponSlotUI == null) return;
        
        if (SecondaryWeapon == null) 
            secondaryWeaponSlotUI.SetEmpty();
        else 
            secondaryWeaponSlotUI.Set(SecondaryWeapon.weaponIcon, 1);
    }
    
    public void OnSlotLeftClick(int index)
    {
        if (!IsValidIndex(index)) return;

        SlotData currentSlot = slotList[index];
        if (currentSlot.IsEmpty)
            return;

        bool valid = currentSlot.weapon != null || 
                     (currentSlot.item != null && currentSlot.item.type == ItemType.Consumable);
        if (!valid) return;

        if (currentSlot.weapon != null)
        {
            useText.text = "EQUIP";
            itemText.text = currentSlot.weapon.name;
        }
        else {
            useText.text = "USE";
            itemText.text = currentSlot.item.name;
        }

        
        selectedIndex = index;
        useMenu.SetActive(true);
        useMenu.transform.position = currentSlot.transform.position + new Vector3(130, -80);
    }
    
    public void UseItemInSelectedSlot()
    {
        if (selectedIndex == -1) return;

        UseItemInSlot(selectedIndex);
        useMenu.SetActive(false);
        selectedIndex = -1;
    }
    
    public void CloseUseMenu()
    {
        useMenu.SetActive(false);
        selectedIndex = -1;
    }

    private int FindEmptySlot()
    {
        for (int i = 0; i < slotList.Count; i++)
            if (slotList[i].IsEmpty)
                return i;
        return -1;
    }

    private bool IsValidIndex(int i)
    {
        return i >= 0 && i < slotList.Count;
    }
    
    public void AddStartingItems(List<StartingItem> startingItems, List<WeaponData> startingWeapons, int coins)
    {
        foreach (StartingItem item in startingItems)
            AddItem(item.item, item.amount);
        
        foreach (WeaponData weapon in startingWeapons)
        {            
            if (PrimaryWeapon == null && weapon.isWeaponPrimary)
                EquipPrimaryFromSlot(AddWeapon(weapon));
            else if (SecondaryWeapon == null && !weapon.isWeaponPrimary)
                EquipSecondaryFromSlot(AddWeapon(weapon));
            else
                AddWeapon(weapon);
        }

        coinsOnHand = coins;
        
        SyncWeaponsToPlayer();
    }
    
    void SyncWeaponsToPlayer()
    {
        var player = gameManager.instance.playerScript;

        player.weapons.Clear();

        if (PrimaryWeapon != null)
            player.weapons.Add(PrimaryWeapon);

        if (SecondaryWeapon != null && SecondaryWeapon != PrimaryWeapon)
            player.weapons.Add(SecondaryWeapon);
    }
}

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
    
    [SerializeField] private List<ItemData> items = new List<ItemData>();

    [Header("Equipped Weapons (UI optional)")]
    [SerializeField] private InventorySlotUI primaryWeaponSlotUI;
    [SerializeField] private InventorySlotUI secondaryWeaponSlotUI;
    private WeaponData PrimaryWeapon { get; set; }
    private WeaponData SecondaryWeapon { get; set; }
    private List<SlotData> slots = new();
    private List<InventorySlotUI> slotUIs = new();
    private int selectedIndex = -1;
    
    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        levelText.text = gameManager.instance.level.ToString("F0") + "/" + gameManager.instance.maxLevel.ToString("F0");
        strText.text = gameManager.instance.playerStats.currentWeapon.damage.ToString("F0");
        atkSpeedText.text = "N/A TODO";
        healthText.text = gameManager.instance.playerScript.health.ToString("F0") + 
                          "/" + gameManager.instance.playerScript.HPOrig.ToString("F0");
    }

    private void Start()
    {
        BuildSlots();
        RefreshUI();

        // TODO: load from save data instead of inspector list
        foreach (ItemData item in items)
        {
            AddItem(item, Random.Range(1, 5));
        }
    }

    private void BuildSlots()
    {
        for (int i = 0; i < slotCount; i++)
            slots.Add(new SlotData());
        
        for (int i = 0; i < slotCount; i++)
        {
            InventorySlotUI ui = Instantiate(slotPrefab, inventoryContent);
            ui.Init(i);
            slotUIs.Add(ui);
        }
    }

    public bool AddItem(ItemData item, int amount = 1)
    {
        if (item == null || amount <= 0) return false;
        
        if (item.isStackable)
        {
            foreach (SlotData currentSlot in slots)
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

            SlotData currentSlot = slots[emptyIndex];
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

    public bool AddWeapon(WeaponData weapon)
    {
        if (weapon == null)
            return false;

        int emptyIndex = FindEmptySlot();
        if (emptyIndex == -1) return false;

        SlotData currentSlot = slots[emptyIndex];
        currentSlot.Clear();
        currentSlot.weapon = weapon;
        currentSlot.amount = 1;

        RefreshUI();
        return true;
    }

    public bool EquipPrimaryFromSlot(int slotIndex)
    {
        if (!IsValidIndex(slotIndex)) return false;

        SlotData currentSlot = slots[slotIndex];
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

        SlotData currentSlot = slots[slotIndex];
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

        SlotData currentSlot = slots[slotIndex];
        if (currentSlot.item == null) return false;
        
        // TODO: this is where you would trigger the item's effect (healing, buff, etc)
        if (currentSlot.item.type == ItemType.Consumable)
        {
            // For testing, just remove one from the stack.
            RemoveFromSlot(slotIndex, 1);
            return true;
        }
        
        return false;
    }

    public void RemoveFromSlot(int slotIndex, int amount)
    {
        if (!IsValidIndex(slotIndex)) 
            return;

        SlotData currentSlot = slots[slotIndex];
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
        for (int i = 0; i < slots.Count && i < slotUIs.Count; i++)
        {
            SlotData currentSlot = slots[i];
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
        //TODO: this is where you would handle selecting a slot,
        // showing item details.
    }

    public void OnSlotRightClick(int index)
    {
        // TODO: MAKE THIS DO SOMETHING (use item, equip weapon, etc)
    }
    

    private int FindEmptySlot()
    {
        for (int i = 0; i < slots.Count; i++)
            if (slots[i].IsEmpty)
                return i;
        return -1;
    }

    private bool IsValidIndex(int i)
    {
        return i >= 0 && i < slots.Count;
    }
}

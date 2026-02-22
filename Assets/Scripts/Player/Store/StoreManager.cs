using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreManager : MonoBehaviour
{
    public static StoreManager instance;

    [Header("UI Setup")]
    [SerializeField] private Transform inventoryContent;
    [SerializeField] private InventorySlotUI slotPrefab;
    [SerializeField] private int slotCount = 24;
    [SerializeField] private GameObject useMenu;
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI useText;
    [SerializeField] private TextMeshProUGUI itemText;
    [SerializeField] private TextMeshProUGUI itemPriceText;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI weaponSpecialEffect;
    [SerializeField] private TextMeshProUGUI weaponDamage;
    [SerializeField] private List<ItemData> itemList = new();
    [SerializeField] private List<WeaponData> weaponList = new();
    
    public List<SlotData> slotList = new();
    private List<InventorySlotUI> slotUIs = new();
    private int selectedIndex = -1;
    
    private void Awake()
    {
        instance = this;
        BuildSlots();
        RefreshUI();
    }

    private void Update()
    {
        coinsText.text = InventoryManager.instance.coinsOnHand.ToString();
    }

    private void BuildSlots()
    {
        for (int i = 0; i < slotCount; i++)
            slotList.Add(new SlotData());
        
        for (int i = 0; i < slotCount; i++)
        {
            InventorySlotUI ui = Instantiate(slotPrefab, inventoryContent);
            ui.Init(i);
            slotUIs.Add(ui);
            slotList[i].transform = ui.transform;
        }
        
        foreach (ItemData item in itemList)
            AddItem(item);
        foreach (WeaponData weapon in weaponList)
            AddWeapon(weapon);
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

        useText.text = "Purchase";
        itemText.text = "";
        itemPriceText.text = "";
        weaponSpecialEffect.text = "";
        weaponDamage.text = "";
        itemIcon.sprite = null;
        itemIcon.enabled = false;

        if (currentSlot.weapon != null)
        {
            WeaponData weapon = currentSlot.weapon;

            itemText.text = weapon.name;

            itemPriceText.text = weapon.weaponPrice.ToString();
            Sprite icon = weapon.weaponIcon;
            itemIcon.sprite = icon;
            itemIcon.enabled = true;

            weaponDamage.text = $"Damage: {weapon.damage}";
            weaponSpecialEffect.text = $"Special: {weapon.specialEffect}";
        }
        else
        {
            ItemData item = currentSlot.item;

            itemText.text = item.name;
            itemPriceText.text = item.itemPrice.ToString();

            Sprite icon = item.itemIcon;
            itemIcon.sprite = icon;
            itemIcon.enabled = true;

            weaponDamage.text = "";
            weaponSpecialEffect.text = "";
        }

        if (currentSlot.itemPurchased)
        {
            itemPriceText.text = "Purchased";
        }

        selectedIndex = index;
        useMenu.SetActive(true);
        useMenu.transform.position = currentSlot.transform.position + new Vector3(130, -80);
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
    
    public void PurchaseItem()
    {
        if (selectedIndex == -1) return;

        if (slotList[selectedIndex].itemPurchased)
        {
            gameManager.instance.SendAlert("Item already purchased!");
            SoundManager.PlaySound(SoundType.Denied);
            useMenu.SetActive(false);
            selectedIndex = -1;
            return;
        }
        
        int cost = slotList[selectedIndex].weapon != null
            ? slotList[selectedIndex].weapon.weaponPrice
            : slotList[selectedIndex].item.itemPrice; 
        
        if (cost > InventoryManager.instance.coinsOnHand)
        {
            gameManager.instance.SendAlert("You do not have enough coins for this item");
            SoundManager.PlaySound(SoundType.Denied);
            useMenu.SetActive(false);
            selectedIndex = -1;
            return;
        }

        slotList[selectedIndex].itemPurchased = true;
        
        InventoryManager.instance.coinsOnHand -= cost;
        if (slotList[selectedIndex].weapon != null)
            InventoryManager.instance.AddWeapon(slotList[selectedIndex].weapon);
        else
            InventoryManager.instance.AddItem(slotList[selectedIndex].item);
        SoundManager.PlaySound(SoundType.Buy);
        
        useMenu.SetActive(false);
        selectedIndex = -1;
    }
    
    public void CloseUseMenu()
    {
        useMenu.SetActive(false);
        selectedIndex = -1;
    }

    private bool IsValidIndex(int i)
    {
        return i >= 0 && i < slotList.Count;
    }

    private void AddItem(ItemData item, int amount = 1)
    {
        if (item == null || amount <= 0) return;

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
                return;
            }
        }
        
        while (amount > 0)
        {
            int emptyIndex = FindEmptySlot();
            if (emptyIndex == -1)
            {
                RefreshUI();
                // TODO: feedback for "inventory full" (UI + sound)
                return;
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
    }
    
    private int FindEmptySlot()
    {
        for (int i = 0; i < slotList.Count; i++)
            if (slotList[i].IsEmpty)
                return i;
        return -1;
    }
    
}

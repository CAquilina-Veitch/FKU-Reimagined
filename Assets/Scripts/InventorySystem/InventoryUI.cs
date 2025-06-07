using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Scripts.Items;
using Scripts.UI;
using Scripts.Managers;
using TMPro;
using R3;

namespace Scripts.InventorySystem
{
    public class InventoryUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject content;
        [SerializeField] private Transform slotsParent;
        [SerializeField] private GameObject slotPrefab;
        [SerializeField] private TextMeshProUGUI itemNameText;
        [SerializeField] private TextMeshProUGUI itemDescriptionText;
        [SerializeField] private TextMeshProUGUI itemStatsText;
        
        private Inventory inventory;
        private List<InventorySlotUI> slotUIs = new List<InventorySlotUI>();
        private InventorySlotUI selectedSlotUI;
        
        private void Start()
        {
            // Subscribe to UI Manager
            if (UIManager.HasInstance)
            {
                UIManager.Instance.ActiveWindows
                    .Subscribe(activeWindows =>
                    {
                        bool shouldShow = activeWindows.Contains(UIWindow.Inventory);
                        if (content != null)
                        {
                            content.SetActive(shouldShow);
                        }
                    })
                    .AddTo(this);
            }
            
            // Start with content hidden
            if (content != null)
            {
                content.SetActive(false);
            }
        }
        
        public void Initialize(Inventory inventory)
        {
            this.inventory = inventory;
            inventory.onInventoryChangedCallback += RefreshUI;
            
            CreateSlotUIs();
            RefreshUI();
        }
        
        private void CreateSlotUIs()
        {
            for (int i = 0; i < inventory.MaxSlots; i++)
            {
                GameObject slotGO = Instantiate(slotPrefab, slotsParent);
                InventorySlotUI slotUI = slotGO.GetComponent<InventorySlotUI>();
                
                if (slotUI == null)
                {
                    slotUI = slotGO.AddComponent<InventorySlotUI>();
                }
                
                slotUI.SetSlotIndex(i);
                slotUI.onSlotSelected += OnSlotSelected;
                slotUI.onItemUsed += OnItemUsed;
                slotUI.onItemDropRequested += OnItemDropRequested;
                slotUIs.Add(slotUI);
            }
        }
        
        public void RefreshUI()
        {
            for (int i = 0; i < slotUIs.Count && i < inventory.Slots.Count; i++)
            {
                InventorySlot slot = inventory.Slots[i];
                slotUIs[i].UpdateSlot(slot);
            }
        }
        
        private void OnSlotSelected(InventorySlotUI slotUI)
        {
            if (selectedSlotUI != null)
            {
                selectedSlotUI.Deselect();
            }
            
            selectedSlotUI = slotUI;
            selectedSlotUI.Select();
            
            UpdateItemInfo(slotUI.GetSlot());
        }
        
        private void UpdateItemInfo(InventorySlot slot)
        {
            if (slot == null || slot.IsEmpty)
            {
                ClearItemInfo();
                return;
            }
            
            Item item = slot.item;
            
            if (itemNameText != null)
                itemNameText.text = item.itemName;
            
            if (itemDescriptionText != null)
                itemDescriptionText.text = item.description;
            
            if (itemStatsText != null)
            {
                string stats = $"Type: {item.itemType}\n";
                stats += $"Rarity: {item.rarity}\n";
                stats += $"Value: {item.baseValue}\n";
                stats += $"Weight: {item.weight}kg\n";
                stats += $"Stack: {slot.quantity}/{item.maxStackSize}";
                
                itemStatsText.text = stats;
            }
        }
        
        private void ClearItemInfo()
        {
            if (itemNameText != null)
                itemNameText.text = "";
            
            if (itemDescriptionText != null)
                itemDescriptionText.text = "";
            
            if (itemStatsText != null)
                itemStatsText.text = "";
        }
        
        public InventorySlot GetSelectedSlot()
        {
            if (selectedSlotUI != null)
            {
                int index = selectedSlotUI.GetSlotIndex();
                if (index >= 0 && index < inventory.Slots.Count)
                {
                    return inventory.Slots[index];
                }
            }
            return null;
        }
        
        private void OnItemUsed(Item item)
        {
            // Use item through InventoryManager
            if (InventoryManager.HasInstance)
            {
                InventoryManager.Instance.UseItem(item);
            }
        }
        
        private void OnItemDropRequested(int slotIndex)
        {
            // Find InventoryController and request drop
            var inventoryController = FindObjectOfType<InventoryController>();
            if (inventoryController != null)
            {
                inventoryController.DropItemFromSlot(slotIndex);
            }
        }
        
        private void OnDestroy()
        {
            if (inventory != null)
            {
                inventory.onInventoryChangedCallback -= RefreshUI;
            }
        }
    }
}
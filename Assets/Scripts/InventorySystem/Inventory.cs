using System.Collections.Generic;
using UnityEngine;
using Scripts.Items;

namespace Scripts.InventorySystem
{
    [System.Serializable]
    public class Inventory
    {
        [SerializeField] private List<InventorySlot> slots = new List<InventorySlot>();
        [SerializeField] private int maxSlots = 20;
        
        public delegate void OnInventoryChanged();
        public event OnInventoryChanged onInventoryChangedCallback;
        
        public List<InventorySlot> Slots => slots;
        public int MaxSlots => maxSlots;
        
        public Inventory(int maxSlots)
        {
            this.maxSlots = maxSlots;
            InitializeSlots();
        }
        
        private void InitializeSlots()
        {
            slots.Clear();
            for (int i = 0; i < maxSlots; i++)
            {
                slots.Add(new InventorySlot(null, 0));
            }
        }
        
        public bool AddItem(Item item, int quantity = 1)
        {
            if (item == null || quantity <= 0) return false;
            
            if (item.isStackable)
            {
                InventorySlot existingSlot = FindItemSlot(item);
                if (existingSlot != null && existingSlot.CanAddAmount(quantity))
                {
                    existingSlot.AddQuantity(quantity);
                    onInventoryChangedCallback?.Invoke();
                    return true;
                }
            }
            
            InventorySlot emptySlot = FindEmptySlot();
            if (emptySlot != null)
            {
                emptySlot.item = item;
                emptySlot.quantity = quantity;
                onInventoryChangedCallback?.Invoke();
                return true;
            }
            
            return false;
        }
        
        public bool RemoveItem(Item item, int quantity = 1)
        {
            if (item == null || quantity <= 0) return false;
            
            InventorySlot slot = FindItemSlot(item);
            if (slot != null && slot.quantity >= quantity)
            {
                slot.RemoveQuantity(quantity);
                onInventoryChangedCallback?.Invoke();
                return true;
            }
            
            return false;
        }
        
        public void RemoveItemAt(int index)
        {
            if (index >= 0 && index < slots.Count)
            {
                slots[index].Clear();
                onInventoryChangedCallback?.Invoke();
            }
        }
        
        public InventorySlot FindItemSlot(Item item)
        {
            foreach (var slot in slots)
            {
                if (!slot.IsEmpty && slot.item == item)
                {
                    return slot;
                }
            }
            return null;
        }
        
        public InventorySlot FindEmptySlot()
        {
            foreach (var slot in slots)
            {
                if (slot.IsEmpty)
                {
                    return slot;
                }
            }
            return null;
        }
        
        public int GetItemCount(Item item)
        {
            int count = 0;
            foreach (var slot in slots)
            {
                if (!slot.IsEmpty && slot.item == item)
                {
                    count += slot.quantity;
                }
            }
            return count;
        }
        
        public bool HasItem(Item item, int quantity = 1)
        {
            return GetItemCount(item) >= quantity;
        }
        
        public void Clear()
        {
            foreach (var slot in slots)
            {
                slot.Clear();
            }
            onInventoryChangedCallback?.Invoke();
        }
    }
}
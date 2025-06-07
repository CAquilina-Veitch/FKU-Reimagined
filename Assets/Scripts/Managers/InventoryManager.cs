using UnityEngine;
using Scripts.Behaviours;
using Scripts.Items;
using Scripts.InventorySystem;

namespace Scripts.Managers
{
    public class InventoryManager : SingletonBehaviour<InventoryManager>
    {
        [Header("Inventory Settings")]
        [SerializeField] private int playerInventorySize = 20;
        
        private Inventory playerInventory;
        
        public Inventory PlayerInventory => playerInventory;
        
        public delegate void OnItemPickedUp(Item item, int quantity);
        public delegate void OnItemDropped(Item item, int quantity);
        public delegate void OnItemUsed(Item item);
        
        public event OnItemPickedUp onItemPickedUpCallback;
        public event OnItemDropped onItemDroppedCallback;
        public event OnItemUsed onItemUsedCallback;
        
        protected override void OnAwake()
        {
            InitializeInventory();
        }
        
        private void InitializeInventory()
        {
            playerInventory = new Inventory(playerInventorySize);
            playerInventory.onInventoryChangedCallback += OnInventoryChanged;
        }
        
        public bool AddItemToPlayer(Item item, int quantity = 1)
        {
            if (item == null) return false;
            
            bool success = playerInventory.AddItem(item, quantity);
            if (success)
            {
                onItemPickedUpCallback?.Invoke(item, quantity);
                Debug.Log($"Added {quantity}x {item.itemName} to inventory");
            }
            else
            {
                Debug.Log($"Failed to add {item.itemName} - Inventory full");
            }
            
            return success;
        }
        
        public bool RemoveItemFromPlayer(Item item, int quantity = 1)
        {
            if (item == null) return false;
            
            bool success = playerInventory.RemoveItem(item, quantity);
            if (success)
            {
                onItemDroppedCallback?.Invoke(item, quantity);
                Debug.Log($"Removed {quantity}x {item.itemName} from inventory");
            }
            
            return success;
        }
        
        public void UseItem(Item item)
        {
            if (item == null) return;
            
            if (playerInventory.HasItem(item))
            {
                item.Use();
                onItemUsedCallback?.Invoke(item);
                
                if (item.itemType == ItemType.Consumable)
                {
                    RemoveItemFromPlayer(item, 1);
                }
            }
        }
        
        public bool CanPickUpItem(Item item, int quantity = 1)
        {
            if (item == null) return false;
            
            if (item.isStackable)
            {
                var existingSlot = playerInventory.FindItemSlot(item);
                if (existingSlot != null && existingSlot.CanAddAmount(quantity))
                {
                    return true;
                }
            }
            
            return playerInventory.FindEmptySlot() != null;
        }
        
        public void TransferItem(Inventory from, Inventory to, Item item, int quantity = 1)
        {
            if (from.RemoveItem(item, quantity))
            {
                if (!to.AddItem(item, quantity))
                {
                    from.AddItem(item, quantity);
                    Debug.Log("Transfer failed - destination inventory full");
                }
            }
        }
        
        private void OnInventoryChanged()
        {
            // You can add any global inventory change logic here
        }
        
        public void SaveInventory()
        {
            // Implement save logic here using PlayerPrefs or a save system
        }
        
        public void LoadInventory()
        {
            // Implement load logic here
        }
    }
}
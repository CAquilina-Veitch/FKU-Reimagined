using UnityEngine;
using Scripts.Managers;
using Scripts.Items;

namespace Scripts.InventorySystem
{
    public class InventoryController : MonoBehaviour
    {
        [Header("Drop Settings")]
        [SerializeField] private Transform dropPoint;
        [SerializeField] private float dropForce = 5f;
        [SerializeField] private GameObject itemDropPrefab;
        
        private InventoryManager inventoryManager;
        
        private void Start()
        {
            inventoryManager = InventoryManager.Instance;
            if (inventoryManager == null)
            {
                Debug.LogError("InventoryController: InventoryManager not found!");
                return;
            }
        }
        
        public void UseItem(Item item)
        {
            inventoryManager.UseItem(item);
        }
        
        public void DropItem(Item item, int quantity = 1)
        {
            if (inventoryManager.RemoveItemFromPlayer(item, quantity))
            {
                SpawnDroppedItem(item, quantity);
            }
        }
        
        public void DropItemFromSlot(int slotIndex, int quantity = 1)
        {
            var inventory = inventoryManager.PlayerInventory;
            if (slotIndex >= 0 && slotIndex < inventory.Slots.Count)
            {
                var slot = inventory.Slots[slotIndex];
                if (!slot.IsEmpty && slot.quantity >= quantity)
                {
                    DropItem(slot.item, quantity);
                }
            }
        }
        
        private void SpawnDroppedItem(Item item, int quantity)
        {
            if (itemDropPrefab == null || dropPoint == null) return;
            
            GameObject droppedItem = Instantiate(itemDropPrefab, dropPoint.position, Quaternion.identity);
            
            // You'll need to create an ItemPickup component that handles the dropped item
            var itemPickup = droppedItem.GetComponent<ItemPickup>();
            if (itemPickup != null)
            {
                itemPickup.SetItem(item, quantity);
            }
            
            Rigidbody rb = droppedItem.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(dropPoint.forward * dropForce, ForceMode.Impulse);
            }
        }
        
    }
}
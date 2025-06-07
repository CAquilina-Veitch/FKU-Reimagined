using UnityEngine;
using Scripts.InventorySystem;
using Scripts.Interaction;
using Scripts.Managers;

namespace Scripts.Items
{
    [RequireComponent(typeof(Collider))]
    public class ItemPickup : Interactable
    {
        [Header("Item Settings")]
        [SerializeField] private Item item;
        [SerializeField] private int quantity = 1;
        
        [Header("Pickup Settings")]
        [SerializeField] private bool autoPickup = false;
        [SerializeField] private float pickupDelay = 0.5f;
        
        [Header("Visuals")]
        [SerializeField] private MeshRenderer itemMesh;
        [SerializeField] private float rotationSpeed = 50f;
        [SerializeField] private float bobHeight = 0.1f;
        [SerializeField] private float bobSpeed = 2f;
        
        private float pickupTimer = 0f;
        private Vector3 startPosition;
        private bool canPickup = false;
        
        protected override void Awake()
        {
            base.Awake();
            startPosition = transform.position;
            
            if (item != null)
            {
                interactionPrompt = $"Pick up {item.itemName}";
            }
        }
        
        private void Start()
        {
            pickupTimer = pickupDelay;
            
            // Update interaction prompt if item is set
            if (item != null)
            {
                interactionPrompt = $"Pick up {quantity}x {item.itemName}";
            }
        }
        
        private void Update()
        {
            if (pickupTimer > 0)
            {
                pickupTimer -= Time.deltaTime;
                if (pickupTimer <= 0)
                {
                    canPickup = true;
                }
            }
            
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
            
            float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
        
        protected override void OnInteract(Interactor interactor)
        {
            if (!canPickup) return;
            
            // Check if interactor is a player (has PlayerInteraction component)
            var playerInteraction = interactor.GetComponent<PlayerInteraction>();
            if (playerInteraction != null)
            {
                TryPickupItem();
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (!autoPickup || !canPickup) return;
            
            // Check if other has PlayerInteraction component (is the player)
            var playerInteraction = other.GetComponent<PlayerInteraction>();
            if (playerInteraction != null)
            {
                TryPickupItem();
            }
        }
        
        private void TryPickupItem()
        {
            if (InventoryManager.HasInstance)
            {
                bool success = InventoryManager.Instance.AddItemToPlayer(item, quantity);
                if (success)
                {
                    Destroy(gameObject);
                }
                else
                {
                    Debug.Log($"Cannot pick up {item.itemName} - Inventory full!");
                }
            }
            else
            {
                Debug.LogError("ItemPickup: InventoryManager not found!");
            }
        }
        
        public void SetItem(Item newItem, int newQuantity)
        {
            item = newItem;
            quantity = newQuantity;
            
            if (item != null)
            {
                interactionPrompt = $"Pick up {quantity}x {item.itemName}";
                
                if (itemMesh != null && item.icon != null)
                {
                    // You might want to set a 3D model based on the item here
                    // For now, we'll just update the prompt
                }
            }
        }
        
        public Item GetItem() => item;
        public int GetQuantity() => quantity;
    }
}
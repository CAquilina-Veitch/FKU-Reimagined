using UnityEngine;
using Scripts.Managers;
using Scripts.UI;
using Scripts.InventorySystem;
using R3;

namespace Scripts.Player.Controls
{
    public class UIInputHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private InventoryController inventoryController;
        
        private InventoryUI inventoryUI;
        
        private void Start()
        {
            // Find InventoryUI if not assigned
            if (inventoryUI == null)
            {
                inventoryUI = FindObjectOfType<InventoryUI>();
            }
            
            // Subscribe to input manager events
            if (InputManager.HasInstance)
            {
                InputManager.Instance.OnInventoryTogglePressed
                    .Subscribe(_ => ToggleInventory())
                    .AddTo(this);
                    
                // Subscribe to drop item input (you'll need to add this to InputManager)
                // For now, we'll handle it in the UI
            }
            else
            {
                Debug.LogError("UIInputHandler: InputManager not found!");
            }
        }
        
        private void ToggleInventory()
        {
            if (UIManager.HasInstance)
            {
                UIManager.Instance.ToggleWindow(UIWindow.Inventory);
            }
            else
            {
                Debug.LogError("UIInputHandler: UIManager not found!");
            }
        }
        
        public void DropSelectedItem()
        {
            if (inventoryController != null && inventoryUI != null)
            {
                var selectedSlot = inventoryUI.GetSelectedSlot();
                if (selectedSlot != null && !selectedSlot.IsEmpty)
                {
                    inventoryController.DropItem(selectedSlot.item, 1);
                }
            }
        }
    }
}
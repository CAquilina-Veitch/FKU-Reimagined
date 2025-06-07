using UnityEngine;

namespace Scripts.Items
{
    [CreateAssetMenu(fileName = "New Consumable", menuName = "Inventory/Items/Consumable")]
    public class ConsumableItem : Item
    {
        [Header("Consumable Properties")]
        public int healthRestore = 0;
        public int manaRestore = 0;
        public float duration = 0f;
        
        public override void Use()
        {
            base.Use();
            
            // Add your consumable logic here
            Debug.Log($"Used {itemName}: Restored {healthRestore} health, {manaRestore} mana");
            
            // You would typically apply effects to the player here
            // Example: PlayerHealth.Instance.Heal(healthRestore);
        }
        
        public override string GetTooltipText()
        {
            string tooltip = base.GetTooltipText();
            
            if (healthRestore > 0)
                tooltip += $"\nRestores {healthRestore} Health";
            if (manaRestore > 0)
                tooltip += $"\nRestores {manaRestore} Mana";
            if (duration > 0)
                tooltip += $"\nDuration: {duration}s";
                
            return tooltip;
        }
    }
}
using UnityEngine;

namespace Scripts.Items
{
    public enum EquipmentSlot
    {
        Head,
        Chest,
        Legs,
        Feet,
        MainHand,
        OffHand,
        Ring,
        Necklace
    }
    
    [CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Items/Equipment")]
    public class EquipmentItem : Item
    {
        [Header("Equipment Properties")]
        public EquipmentSlot equipmentSlot;
        public int attackBonus = 0;
        public int defenseBonus = 0;
        public int speedBonus = 0;
        
        public override void Use()
        {
            base.Use();
            
            // Equip the item
            Debug.Log($"Equipped {itemName} to {equipmentSlot} slot");
            
            // You would typically handle equipment here
            // Example: EquipmentManager.Instance.Equip(this);
        }
        
        public override string GetTooltipText()
        {
            string tooltip = base.GetTooltipText();
            tooltip += $"\nSlot: {equipmentSlot}";
            
            if (attackBonus != 0)
                tooltip += $"\nAttack: {(attackBonus > 0 ? "+" : "")}{attackBonus}";
            if (defenseBonus != 0)
                tooltip += $"\nDefense: {(defenseBonus > 0 ? "+" : "")}{defenseBonus}";
            if (speedBonus != 0)
                tooltip += $"\nSpeed: {(speedBonus > 0 ? "+" : "")}{speedBonus}";
                
            return tooltip;
        }
    }
}
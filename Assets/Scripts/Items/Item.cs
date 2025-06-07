using UnityEngine;

namespace Scripts.Items
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
    public class Item : ScriptableObject
    {
        [Header("Basic Info")]
        public string itemName = "New Item";
        public string description = "";
        public Sprite icon = null;
        
        [Header("Item Properties")]
        public int maxStackSize = 99;
        public bool isStackable = true;
        public float weight = 0.1f;
        
        [Header("Item Type")]
        public ItemType itemType = ItemType.Misc;
        public ItemRarity rarity = ItemRarity.Common;
        
        [Header("Value")]
        public int baseValue = 1;
        
        public virtual void Use()
        {
            Debug.Log($"Using {itemName}");
        }
        
        public virtual string GetTooltipText()
        {
            return $"<b>{itemName}</b>\n{description}\nValue: {baseValue}";
        }
    }
    
    public enum ItemType
    {
        Misc,
        Consumable,
        Equipment,
        Quest,
        Material,
        Tool
    }
    
    public enum ItemRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }
}
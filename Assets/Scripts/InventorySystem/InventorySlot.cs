using Scripts.Items;

namespace Scripts.InventorySystem
{
    [System.Serializable]
    public class InventorySlot
    {
        public Item item;
        public int quantity;
        
        public InventorySlot(Item item, int quantity)
        {
            this.item = item;
            this.quantity = quantity;
        }
        
        public void AddQuantity(int amount)
        {
            quantity += amount;
        }
        
        public void RemoveQuantity(int amount)
        {
            quantity -= amount;
            if (quantity <= 0)
            {
                Clear();
            }
        }
        
        public void Clear()
        {
            item = null;
            quantity = 0;
        }
        
        public bool IsEmpty => item == null || quantity <= 0;
        
        public bool CanAddAmount(int amount)
        {
            if (item == null) return true;
            if (!item.isStackable) return false;
            return quantity + amount <= item.maxStackSize;
        }
    }
}
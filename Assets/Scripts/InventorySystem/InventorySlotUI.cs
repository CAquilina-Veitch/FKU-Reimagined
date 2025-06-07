using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Scripts.Items;
using TMPro;

namespace Scripts.InventorySystem
{
    public class InventorySlotUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("UI Elements")]
        [SerializeField] private Image itemIcon;
        [SerializeField] private TextMeshProUGUI quantityText;
        [SerializeField] private GameObject selectedOverlay;
        [SerializeField] private GameObject hoverOverlay;
        
        private InventorySlot slot;
        private int slotIndex;
        private bool isSelected = false;
        
        public delegate void OnSlotSelected(InventorySlotUI slotUI);
        public delegate void OnItemUsed(Item item);
        public delegate void OnItemDropRequested(int slotIndex);
        
        public event OnSlotSelected onSlotSelected;
        public event OnItemUsed onItemUsed;
        public event OnItemDropRequested onItemDropRequested;
        
        private void Awake()
        {
            if (selectedOverlay != null)
                selectedOverlay.SetActive(false);
            if (hoverOverlay != null)
                hoverOverlay.SetActive(false);
        }
        
        public void SetSlotIndex(int index)
        {
            slotIndex = index;
        }
        
        public int GetSlotIndex()
        {
            return slotIndex;
        }
        
        public InventorySlot GetSlot()
        {
            return slot;
        }
        
        public void UpdateSlot(InventorySlot newSlot)
        {
            slot = newSlot;
            
            if (slot == null || slot.IsEmpty)
            {
                ClearSlot();
            }
            else
            {
                itemIcon.sprite = slot.item.icon;
                itemIcon.enabled = true;
                
                if (slot.quantity > 1)
                {
                    quantityText.text = slot.quantity.ToString();
                    quantityText.enabled = true;
                }
                else
                {
                    quantityText.enabled = false;
                }
            }
        }
        
        private void ClearSlot()
        {
            itemIcon.sprite = null;
            itemIcon.enabled = false;
            quantityText.enabled = false;
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                onSlotSelected?.Invoke(this);
            }
            else if (eventData.button == PointerEventData.InputButton.Right && slot != null && !slot.IsEmpty)
            {
                onItemUsed?.Invoke(slot.item);
            }
            else if (eventData.button == PointerEventData.InputButton.Middle && slot != null && !slot.IsEmpty)
            {
                onItemDropRequested?.Invoke(slotIndex);
            }
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (hoverOverlay != null && !isSelected)
                hoverOverlay.SetActive(true);
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            if (hoverOverlay != null)
                hoverOverlay.SetActive(false);
        }
        
        public void Select()
        {
            isSelected = true;
            if (selectedOverlay != null)
                selectedOverlay.SetActive(true);
            if (hoverOverlay != null)
                hoverOverlay.SetActive(false);
        }
        
        public void Deselect()
        {
            isSelected = false;
            if (selectedOverlay != null)
                selectedOverlay.SetActive(false);
        }
    }
}
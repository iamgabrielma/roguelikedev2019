using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EnhancedInventorySlot : EnhancedItemSlotUI, IDropHandler
{
    [SerializeField] private EnhancedInventory inventory = null;
    [SerializeField] private Text itemQuantityText = null;

    public override EnhancedHotbarItem EnhancedSlotItem {

        get { return ItemSlot.item; }
        set { } // works but has nothing .... check.
    }

    public EnhancedItemSlot ItemSlot => inventory.enhancedItemContainer.GetSlotsByIndex(EnhancedSlotIndex);

    public override void OnDrop(PointerEventData eventData) {

        // When we drop something in this slot, try to get the EnhancedItemDragHandler script from the thing that dropped into
        EnhancedItemDragHandler enhancedItemDragHandler = eventData.pointerDrag.GetComponent<EnhancedItemDragHandler>();

        if (enhancedItemDragHandler == null)
        {
            return;
        }

        if ((enhancedItemDragHandler.ItemSlotUI as EnhancedInventorySlot != null))
        {
            inventory.enhancedItemContainer.Swap(enhancedItemDragHandler.ItemSlotUI.EnhancedSlotIndex, EnhancedSlotIndex);
        }
    }

    public override void UpdateSlotUI()
    {
        if (ItemSlot.item == null)
        {
            EnableSlotUI(false); // If the item is null, disable the slot
            return;
        }
        // If the item is not null, enable the slot and fil it up with sprite and quantity
        EnableSlotUI(true);
        itemIconImage.sprite = ItemSlot.item.Icon;
        itemQuantityText.text = ItemSlot.quantity > 1 ? ItemSlot.quantity.ToString() : ""; // if is 1 or 0 we want the string to be empty
    }

    protected override void EnableSlotUI(bool enable) {

        base.EnableSlotUI(enable); // this will do the icon
        itemQuantityText.enabled = enable; // this will override the quantity
    }
}

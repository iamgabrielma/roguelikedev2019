using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/* The behavior of the hotbar will be different than the inventory, as while inventory holds items, the hotbar holds references to these items (like WOW hotbars) */
public class EnhancedHotbarSlot : EnhancedItemSlotUI, IDropHandler 
{

    [SerializeField] private EnhancedInventory inventory = null; // reference to the inventory
    //NOT IMPLEMENTED FOR HOTBARS [SerializeField] private Text itemQuantityText = null;
    private EnhancedHotbarItem slotItem = null; // our slot item, can represent anything dropped into it or null if is empty

    public override EnhancedHotbarItem EnhancedSlotItem { 

        get { return slotItem; }
        set { slotItem = value; UpdateSlotUI(); } // If we set the value from here, we also want to update the UI
    }

    public bool AddItem(EnhancedHotbarItem itemToAdd) {

        // If there's already an item on EnhancedSlotItem, just return false: Nope, we cannot AddItem here.
        if (EnhancedSlotItem != null)
        {
            return false;
        }
        // Otherwise the slot will be equal to the itemToAdd, and return true as we added it
        EnhancedSlotItem = itemToAdd;
        return true;
    }

    // press key + listen for input event (ex: key 1 => slot 0)
    public void UseSlot(int index)
    {
        if (index != EnhancedSlotIndex)
        {
            return;
        }

        // otherwise USE ITEM
    }

    // What happens when you drop something into the hotbar slot
    public override void OnDrop(PointerEventData eventData)
    {
        // Making sure the thing we're dropping has a drag handler, otherwise return
        EnhancedItemDragHandler itemDragHandler = eventData.pointerDrag.GetComponent<EnhancedItemDragHandler>();
        if (itemDragHandler == null)
        {
            return;
        }
        // Check: What it is (inventory, hotbar, etc, .. ), and do something different based on what it is.
        // 1. Some casting to solve this: Treat the slot as inventory slot, and if happens to be an inventory slot then proceed with getting the item from that inventory slot
        EnhancedInventorySlot inventorySlot = itemDragHandler.ItemSlotUI as EnhancedInventorySlot;
        if (inventorySlot != null)
        {
            EnhancedSlotItem = inventorySlot.ItemSlot.item;
            return;
        }
        // 2. Same flow as before: If is hotbar slot, proceed and swap items.
        EnhancedHotbarSlot hotbarSlot = itemDragHandler.ItemSlotUI as EnhancedHotbarSlot;
        if (hotbarSlot != null)
        {
            EnhancedHotbarItem oldItem = EnhancedSlotItem; // store the old item
            EnhancedSlotItem = hotbarSlot.EnhancedSlotItem; // set our item to the hotbar slot
            hotbarSlot.EnhancedSlotItem = oldItem; // then drop the old item into this slot
            return;
        }
    }

    public override void UpdateSlotUI()
    {
        if (EnhancedSlotItem == null)
        {
            EnableSlotUI(false); // if there's nothing in the slot (null) disable the UI image for that slot
            return;
        }
        // otherwise, set the sprite to the slot icon, and enable the slot UI
        itemIconImage.sprite = EnhancedSlotItem.Icon;
        EnableSlotUI(true);
        //NOT IMPLEMENTED FOR HOTBARS SetItemQuantityUI();
    }

    //NOT IMPLEMENTED FOR HOTBARS public void SetItemQuantityUI()
    //{
    //    if (EnhancedSlotItem is EnhancedInventoryItem inventoryItem)
    //    {
    //        if (inventory.enhancedItemContainer.HasItem(inventoryItem))
    //        {
    //            int quantityCount = inventory.enhancedItemContainer.GetTotalQuantity(inventoryItem);
    //            itemQuantityText.text = quantityCount > 1 ? quantityCount.ToString() : "";
    //        }
    //        else
    //        {
    //            EnhancedSlotItem = null; // We clear the quantity if the player does not have the item anymore
    //        }
    //    }
    //    else
    //    {
    //        // If is not an inventory item, then disable the text
    //        itemQuantityText.enabled = false;
    //    }
    //}

    protected override void EnableSlotUI(bool enable)
    {
        base.EnableSlotUI(enable);
        //NOT IMPLEMENTED FOR HOTBARS itemQuantityText.enabled = enable;
    }
}

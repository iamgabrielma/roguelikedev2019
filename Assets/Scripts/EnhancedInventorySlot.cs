using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EnhancedInventorySlot : EnhancedItemSlotUI
{
    [SerializeField] private EnhancedInventory inventory = null;
    [SerializeField] private Text itemQuantityText = null;

    public override EnhancedHotbarItem EnhancedSlotItem {

        get { return ItemSlot.item; }
        set { } // works but has nothing .... check.
    }

    public EnhancedItemSlot ItemSlot => inventory.enhancedItemContainer.GetSlotsByIndex(EnhancedSlotIndex);

    public override void OnDrop(PointerEventData eventData) {

        EnhancedItemDragHandler enhancedItemDragHandler = eventData.pointerDrag.GetComponent<EnhancedItemDragHandler>();

        if (enhancedItemDragHandler == null)
        {
            return;
        }

        // NOT IMPLEMENTED
        //if ((enhancedItemDragHandler.ItemSlotUI as EnhancedInventorySlot != null))
        //{
        //    //inventory.Swap();
        //}
    }

    public override void UpdateSlotUI()
    {
        //throw new System.NotImplementedException();
        if (ItemSlot.item == null)
        {
            EnableSlotUI(false);
            return;
        }
        EnableSlotUI(true);
        itemIconImage.sprite = ItemSlot.item.Icon;
        itemQuantityText.text = ItemSlot.quantity > 1 ? ItemSlot.quantity.ToString() : ""; // if is 1 or 0 we want the string to be empty
    }

    protected override void EnableSlotUI(bool enable) {

        base.EnableSlotUI(enable);
        itemQuantityText.enabled = enable;
    }
}

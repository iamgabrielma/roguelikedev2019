using UnityEngine;
using UnityEngine.EventSystems;

/* Specific logic for when you drag on inventory, as opposed to hotbar, spells, etc, ...  */
public class EnhancedInventoryItemDragHandler : EnhancedItemDragHandler
{
    [SerializeField] private EnhancedItemDestroyer itemDestroyer = null;

    public override void OnPointerUp(PointerEventData eventData) // Overrides the default OnPointerUp logic for when we release the dragged item from Inventory
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            base.OnPointerUp(eventData); // Do the base logic

            if (eventData.hovered.Count == 0) // When we release our item over nothing, we'll destroy the item:
            {
                EnhancedInventorySlot thisSlot = ItemSlotUI as EnhancedInventorySlot;
                itemDestroyer.Activate(thisSlot.ItemSlot, thisSlot.EnhancedSlotIndex);
            }
        }
    }
}

using UnityEngine.EventSystems;

public class EnhancedHotbarItemDragHandler : EnhancedItemDragHandler
{
    public override void OnPointerUp(PointerEventData eventData)
    {
        // If we release the left mouse button into a hotbar slot:
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            base.OnPointerUp(eventData);

            if (eventData.hovered.Count == 0)
            {
                // The reason for this is that if we drag an item out of the inventory we want to destroy it, but if is from the hotbar we only want to clear the reference.
                // this way if we release over nothing, we clear the slot
                (itemSlotUI as EnhancedHotbarSlot).EnhancedSlotItem = null;
            }
        }

    }
}

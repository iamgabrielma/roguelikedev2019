using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnhancedItemDestroyer : MonoBehaviour
{

    // reference to the inventory
    [SerializeField] private EnhancedInventory inventory = null;
    [SerializeField] private Text areYouSureText = null;
    private int slotIndex = 0; // will use it to check what position is the item rather than which item is it, defaults to zero. If you have 2 stacks of the same item in different slots, you want to target the correct one as on destroy will loop through the inventory and destroy the first it founds, not the one we selected.

    private void OnDisable()
    {
        slotIndex = -1; // -1 as none on an enum, or while is disabled.
    }

    public void Activate(EnhancedItemSlot itemSlot, int slotIndex) {

        this.slotIndex = slotIndex;
        areYouSureText.text = $"Are you sure you want to destroy {itemSlot.quantity}x {itemSlot.item.ColoredName} ?";

        gameObject.SetActive(true);
    }

    public void Destroy()
    {
        inventory.enhancedItemContainer.RemoveAt(slotIndex);
        gameObject.SetActive(false);
    }
}

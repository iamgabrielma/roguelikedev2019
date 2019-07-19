using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* I'm using the hotbar as equipment manager, so don't need much space at the moment, 5 slots for all technology items */
public class EnhancedHotbar : MonoBehaviour
{
    [SerializeField] private EnhancedHotbarSlot[] hotbarSlots = new EnhancedHotbarSlot[5];

    // Adds to the hotbar
    // We'll go through each slot asking if can have this item, as soon as we find one that can, return and don't check anymore
    // This function will have a different use than ADDING VIA DROPPING IT in the hotbar, and is more for calling it when something happens (like shortkey to add spells to hotbar)
    // If we release an item in a slot, all we care is if the slot can have the item, here we're asking the hotbar as a whole if it can take the item, which asks its slots to see if there's space.
    public void Add(EnhancedHotbarItem itemToAdd)
    {
        foreach (EnhancedHotbarSlot hotbarSlot in hotbarSlots)
        {
            if (hotbarSlot.AddItem(itemToAdd)) // bool, if successfully add the item to hotbar, return.
            {
                Debug.Log("Added item to the Hotbar");
                return; 
            }
        }

    }
}

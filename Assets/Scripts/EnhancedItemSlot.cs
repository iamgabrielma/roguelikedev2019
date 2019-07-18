using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct EnhancedItemSlot
{
    /* We pass by value instead of by reference (classes) */
    public EnhancedInventoryItem item;
    public int quantity;

    // Constructor
    public EnhancedItemSlot(EnhancedInventoryItem item, int quantity)
    {

        this.item = item;
        this.quantity = quantity;
    }

    /*
     * This has a very specific use: 
     * In EnhancedItemContainer.Swap() we want to check what happens when you pick up an item, and drop it back to the same spot. For this we have to compare firstSlot == secondSlot 
     * However we cannot compare 2 structs directly, so we'll create this helper function in orer to do so  
    */
    public static bool operator == (EnhancedItemSlot a, EnhancedItemSlot b)
    {
        return a.Equals(b);
    }
    public static bool operator !=(EnhancedItemSlot a, EnhancedItemSlot b)
    {
        return !a.Equals(b);
    }

}

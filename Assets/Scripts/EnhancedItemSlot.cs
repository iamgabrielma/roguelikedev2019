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

}

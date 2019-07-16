using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnhancedItemContainer
{

    // store all the functions of everything that contains items has to have

    EnhancedItemSlot AddItem(EnhancedItemSlot enhancedItemSlot);

    void RemoveItem(EnhancedItemSlot enhancedItemSlot);

    void RemoveAt(int enhancedItemSlotIndex);

    bool HasItem(EnhancedInventoryItem item);

    int GetTotalQUantity(EnhancedInventoryItem item);

    // swamp
}
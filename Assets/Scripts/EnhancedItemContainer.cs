using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnhancedItemContainer: IEnhancedItemContainer
{
    private EnhancedItemSlot[] enhancedItemSlots = new EnhancedItemSlot[0];

    public Action OnItemsUpdated = delegate { }; // Delegates are used to pass methods as arguments to other methods. Event handlers are nothing more than methods that are invoked through delegates. 

    public EnhancedItemContainer(int size) => enhancedItemSlots = new EnhancedItemSlot[size]; // So we initialize it to a certain number of sizes for different characters

    public EnhancedItemSlot GetSlotsByIndex(int index) => enhancedItemSlots[index];

    public EnhancedItemSlot AddItem(EnhancedItemSlot enhancedItemSlot)
    {
        for (int i = 0; i < enhancedItemSlots.Length; i++)
        {
            // If the item on that position is not null (meaning is something there)
            if (enhancedItemSlots[i].item != null)
            {
                // If the item type in the slot is the same type of the item we're trying to add:
                if (enhancedItemSlots[i].item == enhancedItemSlot.item)
                {
                    // Remaining space will be the max tack minus the quantity
                    int slotRemainingSpace = enhancedItemSlots[i].item.MaxStack - enhancedItemSlots[i].quantity;

                    // If we're trying to add LESS items than the remaining space, just sum it up (example: We want to add 5 items and there's 8 spaces left, then go ahead)
                    if (enhancedItemSlot.quantity <= slotRemainingSpace)
                    {
                        enhancedItemSlots[i].quantity += enhancedItemSlot.quantity;
                        enhancedItemSlot.quantity = 0;

                        OnItemsUpdated.Invoke();

                        return enhancedItemSlot; // We return the same that was passed in, but with different values
                    }
                    // If we're trying to add MORE items than the remaining space, 
                    else if (slotRemainingSpace > 0)
                    {
                        enhancedItemSlots[i].quantity += slotRemainingSpace; //We can have some of it, but not all of it, then in next iterations of the loop will fill it up somewhere else
                        enhancedItemSlot.quantity -= slotRemainingSpace;
                    }
                }
            }
        }

        // We loop again through the slots
        for (int i = 0; i < enhancedItemSlots.Length; i++)
        {
            // If we find an empty slot:
            if (enhancedItemSlots[i].item == null)
            {
                // If the quantity we want to add is LESS than the max stack of that item slot, then we have found a place where to add the item:
                if (enhancedItemSlot.quantity <= enhancedItemSlot.item.MaxStack)
                {
                    enhancedItemSlots[i] = enhancedItemSlot;
                    enhancedItemSlot.quantity = 0;

                    OnItemsUpdated.Invoke();

                    return enhancedItemSlot;
                }
                // If the quantity we want to add is MORE than the max stack of that item slot (case of a boss or chest drop, where may be 20 potions in one pickup but your stacks only allow 5 potions, you need to make 4 stacks of 5)
                else
                {
                    enhancedItemSlots[i] = new EnhancedItemSlot(enhancedItemSlot.item, enhancedItemSlot.item.MaxStack);
                    enhancedItemSlot.quantity -= enhancedItemSlot.item.MaxStack;
                }
            }
        }

        OnItemsUpdated.Invoke();

        return enhancedItemSlot;
    }

    public int GetTotalQUantity(EnhancedInventoryItem item)
    {
        int totalCount = 0;

        foreach (EnhancedItemSlot itemSlot in enhancedItemSlots)
        {
            if (itemSlot.item == null)
            {
                continue; // Don't add if the slot is empty. Next iteration
            }
            if (itemSlot.item != item)
            {
                continue; // Don't add if the item on the slot is not the item we're counting. Next iteration
            }

            totalCount += itemSlot.quantity; // For all valid items (not empty and the right type), add it to totalCount
        }

        return totalCount;
    }

    // For Lists there's a generic method, but as our inventory will be fixed we're using arrays for performance.
    public bool HasItem(EnhancedInventoryItem item)
    {
        foreach (EnhancedItemSlot itemSlot in enhancedItemSlots)
        {
            if (itemSlot.item == null) 
            {
                continue; // Is not our item because the slot doesn't have any item, continue
            }
            if (itemSlot.item != item)
            {
                continue; // Is not our item, continue
            }

            return true; // If we find our item, as soon as we do return true
        }

        return false; // We reach the end of items array and we haven't found it, return false
    }

    public void RemoveAt(int enhancedItemSlotIndex)
    {
        // Safety check: We'll check if the index is valid or not, if is out of range ( min (0) or max(length -1)) then return
        if (enhancedItemSlotIndex < 0 || enhancedItemSlotIndex > enhancedItemSlots.Length - 1)
        {
            return; 
        }

        enhancedItemSlots[enhancedItemSlotIndex] = new EnhancedItemSlot(); // If checks out, we clear it.

        OnItemsUpdated.Invoke(); // Update UI when we change anything in the inventory via raising an event

    }

    // Important to play with indexes in the Swap() method instead of with types of objects, as we may find the same type in different slots, but slots are unique (as their indexes)
    public void Swap(int indexOne, int indexTwo)
    {
        EnhancedItemSlot firstSlot = enhancedItemSlots[indexOne]; // What we're dragging
        EnhancedItemSlot secondSlot = enhancedItemSlots[indexTwo]; // What we're dragging into

        if (firstSlot == secondSlot) // We cannot compare two structs, so we have added some helper operator functions within EnhancedItemSlot to do this comparison.
        {
            return;
        }

        if (secondSlot.item != null) // If we're dropping into something that is already there (not empty)
        {
            if (firstSlot.item == secondSlot.item) // Let's check if is the same item type, if they are, let's combine them
            {
                int secondSlotRemainingSpace = secondSlot.item.MaxStack - secondSlot.quantity; // How much space is left in the slot

                if (firstSlot.quantity <= secondSlotRemainingSpace)
                {
                    // If there's enough space, then we can shove all the items from the 1st slot into the 2nd slot:
                    enhancedItemSlots[indexTwo].quantity += secondSlot.quantity;
                    // Because we have added all the stuff into the 2nd, we want to clear the 1st position
                    enhancedItemSlots[indexOne] = new EnhancedItemSlot();

                    OnItemsUpdated.Invoke();

                    return;
                }
            }
        }

        // If is not the same item type in both slots, just swamp them
        enhancedItemSlots[indexOne] = secondSlot;
        enhancedItemSlots[indexTwo] = firstSlot;

        OnItemsUpdated.Invoke();

    }

    public void RemoveItem(EnhancedItemSlot enhancedItemSlot)
    {
        for (int i = 0; i < enhancedItemSlots.Length; i++)
        {
            if (enhancedItemSlots[i].item != null) // If the slot is not empty:
            {
                if (enhancedItemSlots[i].item == enhancedItemSlot.item) // If the item in the slot is the samesame item we have passed through the method and we want to remove
                {
                    // If the quantity of the item we're checking is LESS than the quantity of the item we want to remove:
                    if (enhancedItemSlots[i].quantity < enhancedItemSlot.quantity) 
                    {
                        enhancedItemSlot.quantity -= enhancedItemSlots[i].quantity; // We remove them entirely and we're done.
                        enhancedItemSlots[i] = new EnhancedItemSlot();
                    }
                    // If the quantity of the item we're checking is MORE than the quantity of the item we want to remove:
                    else
                    {
                        // Is the right item, but there's too much of it ( we try to remove 5, but there's 10, we'll remove only 5 and leave the other 5 there)
                        enhancedItemSlots[i].quantity -= enhancedItemSlot.quantity;

                        if (enhancedItemSlots[i].quantity == 0) // If happens to be 0 the remaining items, then we clear the slot and update the UI
                        {
                            // if the slot is now empty:
                            enhancedItemSlots[i] = new EnhancedItemSlot();

                            OnItemsUpdated.Invoke();

                            return;
                        }

                        OnItemsUpdated.Invoke();
                    }
                }
            }
        }
    }
}


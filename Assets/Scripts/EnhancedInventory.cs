using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New inventory", menuName ="Items/Inventory")]
public class EnhancedInventory : ScriptableObject //, IEnhancedItemContainer
{
    [SerializeField] private VoidEvent onInventoryItemsUpdated = null;
    [SerializeField] private EnhancedItemSlot test_item_slot = new EnhancedItemSlot();

    public EnhancedItemContainer enhancedItemContainer { get; } = new EnhancedItemContainer(10);

    public void OnEnable()
    {
        enhancedItemContainer.OnItemsUpdated += onInventoryItemsUpdated.Raise; // += subscribe to the event
    }

    public void OnDisable()
    {
        enhancedItemContainer.OnItemsUpdated -= onInventoryItemsUpdated.Raise; // -= unsubscribe to the event
    }

    [ContextMenu("Test Add")]
    public void Test_Add()
    {
        enhancedItemContainer.AddItem(test_item_slot);
    }

    /* MOVED ALL OF THIS TO ENHANCEDITEMCONTAINER INSTEAD */
    //    private EnhancedItemSlot[] enhancedItemSlots = new EnhancedItemSlot[10];

    //    public Action OnItemsUpdated = delegate { };

    //    public EnhancedItemSlot GetSlotsByIndex(int index) => enhancedItemSlots[index];

    //    public EnhancedItemSlot AddItem(EnhancedItemSlot enhancedItemSlot)
    //    {
    //        for (int i = 0; i < enhancedItemSlots.Length; i++)
    //        {
    //            if (enhancedItemSlots[i].item == null)
    //            {
    //                if (enhancedItemSlots[i].item == enhancedItemSlot.item)
    //                {
    //                    int slotRemainingSpace = enhancedItemSlots[i].item.MaxStack - enhancedItemSlots[i].quantity;

    //                    if (enhancedItemSlot.quantity <= slotRemainingSpace)
    //                    {
    //                        enhancedItemSlots[i].quantity += enhancedItemSlot.quantity;
    //                        enhancedItemSlot.quantity = 0;

    //                        OnItemsUpdated.Invoke();

    //                        return enhancedItemSlot;
    //                    }
    //                    else if(slotRemainingSpace > 0)
    //                    {
    //                        enhancedItemSlots[i].quantity += enhancedItemSlot.quantity;
    //                        enhancedItemSlot.quantity -= slotRemainingSpace;
    //                    }
    //                }
    //            }
    //        }

    //        for (int i = 0; i < enhancedItemSlots.Length; i++)
    //        {
    //            if (enhancedItemSlots[i].item == null)
    //            {
    //                if(enhancedItemSlot.quantity <= enhancedItemSlot.item.MaxStack)
    //                {
    //                    enhancedItemSlots[i] = enhancedItemSlot;
    //                    enhancedItemSlot.quantity = 0;

    //                    OnItemsUpdated.Invoke();

    //                    return enhancedItemSlot;
    //                }
    //                else
    //                {
    //                    enhancedItemSlots[i] = new EnhancedItemSlot(enhancedItemSlot.item, enhancedItemSlot.item.MaxStack);
    //                    enhancedItemSlot.quantity -= enhancedItemSlot.item.MaxStack;
    //                }
    //            }
    //        }

    //        OnItemsUpdated.Invoke();

    //        return enhancedItemSlot;
    //    }

    //    public int GetTotalQUantity(EnhancedInventoryItem item)
    //    {
    //        int totalCount = 0;

    //        foreach (EnhancedItemSlot itemSlot in enhancedItemSlots)
    //        {
    //            if (itemSlot.item == null)
    //            {
    //                continue; // Don't add if the slot is empty. Next iteration
    //            }
    //            if (itemSlot.item != item)
    //            {
    //                continue; // Don't add if the slot is not the one that we want. Next iteration
    //            }

    //            totalCount += itemSlot.quantity; // for all valid ones, add it to totalcount
    //        }

    //        return totalCount;
    //    }

    //    public bool HasItem(EnhancedInventoryItem item)
    //    {
    //        foreach (EnhancedItemSlot itemSlot in enhancedItemSlots)
    //        {
    //            if (itemSlot.item == null)
    //            {
    //                continue; 
    //            }
    //            if (itemSlot.item != item)
    //            {
    //                continue;
    //            }

    //            return true;
    //        }

    //        return false; // We reach the end of items array and we haven't found it
    //    }

    //    public void RemoveAt(int enhancedItemSlotIndex)
    //    {
    //        // We'll check if the index is valid, if is, then set the item to be a new empty itemSlot
    //        if (enhancedItemSlotIndex < 0 || enhancedItemSlotIndex > enhancedItemSlots.Length -1)
    //        {
    //            return; // if is out of range, min or max, return. safety check
    //        }

    //        enhancedItemSlots[enhancedItemSlotIndex] = new EnhancedItemSlot(); // If checks out, we clear it.

    //        OnItemsUpdated.Invoke();

    //    }

    //    public void RemoveItem(EnhancedItemSlot enhancedItemSlot)
    //    {
    //        for (int i = 0; i < enhancedItemSlots.Length; i++)
    //        {
    //            if (enhancedItemSlots[i].item !=  null)
    //            {
    //                if (enhancedItemSlots[i].item == enhancedItemSlot.item) // if is the same item we have passed through the method
    //                {
    //                    if (enhancedItemSlots[i].quantity < enhancedItemSlot.quantity)
    //                    {
    //                        enhancedItemSlot.quantity -= enhancedItemSlots[i].quantity;
    //                        enhancedItemSlots[i] = new EnhancedItemSlot();
    //                    }
    //                    else
    //                    {
    //                        // Is the right item, but there's too much of it:
    //                        enhancedItemSlots[i].quantity -= enhancedItemSlot.quantity;

    //                        if (enhancedItemSlots[i].quantity == 0)
    //                        {
    //                            // if the slot is now empty:
    //                            enhancedItemSlots[i] = new EnhancedItemSlot();

    //                            OnItemsUpdated.Invoke();

    //                            return;
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }
}

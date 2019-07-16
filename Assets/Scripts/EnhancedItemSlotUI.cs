using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/* Base class for any item slot */
public abstract class EnhancedItemSlotUI: MonoBehaviour
{
    [SerializeField] protected Image itemIconImage = null;
    public int EnhancedSlotIndex { get; private set; }

    public abstract EnhancedHotbarItem EnhancedSlotItem { get; set; }

    // When the object becomes active:
    // This will be called only once
    private void OnEnable()
    {
        UpdateSlotUI();
    }

    // protected virtual because this Start() function may be different for the children
    // This will be called every time we enable the inventory
    protected virtual void Start()
    {
        EnhancedSlotIndex = transform.GetSiblingIndex(); // Checks where we are in the hyerarchy

        UpdateSlotUI(); // This will be called twice on Start but that's alright as we need to get the sibling first in order to use it correctly.
    }

    public abstract void OnDrop(PointerEventData eventData);

    public abstract void UpdateSlotUI();

    protected virtual void EnableSlotUI(bool enable) {

        itemIconImage.enabled = enable; // No matter what the slot is, the icon will be enabled or disabled.
    }

}

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/* Base class for any item slot */
public abstract class EnhancedItemSlotUI: MonoBehaviour, IDropHandler
{
    [SerializeField] protected Image itemIconImage = null;  // Each item will have an image, and protected because we want to access it in all our child classes
    public int EnhancedSlotIndex { get; private set; } // We store an int which is a reference to the slot index, so we want to (SET) the index here, but be able to reference it (GET) from elsewhere

    public abstract EnhancedHotbarItem EnhancedSlotItem { get; set; } // While this has a setter as well, doesn't mean we have to technically SET it. Different slots will get their items in a different way, as we may have inventory, actionbar, gear, etc, ... each one will reference its own

    // When the object becomes active. This will be called only once.
    private void OnEnable()
    {
        UpdateSlotUI();
    }

    // protected virtual because this Start() function may be different for the children and we want to be able to override it. This will be called every time we enable the inventory
    protected virtual void Start()
    {
        EnhancedSlotIndex = transform.GetSiblingIndex(); // Checks where we are in the hyerarchy
        UpdateSlotUI(); // This will be called twice on Start but that's alright as we need to get the sibling first in order to use it correctly.
    }

    public abstract void OnDrop(PointerEventData eventData); // abstract, we'll take care of it in our child classes

    public abstract void UpdateSlotUI(); // abstract, we'll take care of it in our child classes. In this case for example a hotbar may have cooldowns, but the inventory won't, so there's different ways to update the UI based on the child

    protected virtual void EnableSlotUI(bool enable) {

        itemIconImage.enabled = enable; // No matter what the slot is, the icon will be enabled or disabled.
    }

}

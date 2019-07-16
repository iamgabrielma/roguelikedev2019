using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Item : ScriptableObject //, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private new string name; //item.name hides object.name, so we use "new" keyword
    [SerializeField] private string description;
    [SerializeField] private int sellPrice;
    [SerializeField] private string test_itemtype;
    public enum ItemType {
        none,
        consumable,
        propulsion,
        armament,
        sensors,
        navigation,
        communication,
        capacity
    }

    // "=>" is a getter. We cannot get name because is private, but we can get it (not set it) through Name.
    public string Name { get { return name; } }
    public int SellPrice { get { return sellPrice; } }
    public string Test_ItemType { get { return test_itemtype; } }

    // I'm not sure if I want to go this way, commented for now:
    //public _ItemType ItemType{ get { return _ItemType.consumable; } } // Declares a property of TYPE _ItemType , with a name of ItemType

    public abstract string ColoredName { get; }

    public abstract string GetToolTipInfoText();

    /* Testing item type for scriptable objects*/
    //[SerializeField] private ItemType itemType;
    public ItemType typeOfItem { get { return ItemType.none; } } // default has no item type.
}   

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Item : ScriptableObject //, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private new string name; //item.name hides object.name, so we use "new" keyword
    [SerializeField] private string description;
    [SerializeField] private int sellPrice;
    public enum _ItemType { 
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

    // I'm not sure if I want to go this way, commented for now:
    //public _ItemType ItemType{ get { return _ItemType.consumable; } } // Declares a property of TYPE _ItemType , with a name of ItemType

    public abstract string ColoredName { get; }

    public abstract string GetToolTipInfoText();

}

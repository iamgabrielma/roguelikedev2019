using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Item : ScriptableObject //, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private new string name; //item.name hides object.name, so we use "new" keyword
    [SerializeField] private string description;
    [SerializeField] private int sellPrice;

    // "=>" is a getter. We cannot get name because is private, but we can get it (not set it) through Name.
    public string Name { get { return name; } }
    public int SellPrice { get { return sellPrice; } }

    public abstract string ColoredName { get; }

    public abstract string GetToolTipInfoText();

    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    Item.DisplayInfo();
    //}

    //public void OnPointerExit(PointerEventData eventData)
    //{

    //}
}

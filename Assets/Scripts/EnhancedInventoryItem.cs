using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnhancedInventoryItem : EnhancedHotbarItem
{
    /* We make this ABSTRACT so we dont need to implement this, as will be other class job to do so */
    //public override string ColoredName => throw new System.NotImplementedException();
    //public override string GetToolTipInfoText()
    //{
    //    throw new System.NotImplementedException();
    //}
    [Header("Enhanced Item Data")]
    [Min(0)] private int sellPrice = 1;
    [Min(1)] private int maxStack = 1;

    public override string ColoredName
    {
        get
        {
            return Name;
        }
    }

    public int SellPrice => sellPrice;
    public int MaxStack => maxStack;
}

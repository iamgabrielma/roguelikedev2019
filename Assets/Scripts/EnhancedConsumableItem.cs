using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New consumable item", menuName ="EItems/Consumable")]
public class EnhancedConsumableItem : EnhancedInventoryItem
{
    //[Header("Consumable item")]
    public override string GetToolTipInfoText()
    {
        throw new System.NotImplementedException();
    }

}

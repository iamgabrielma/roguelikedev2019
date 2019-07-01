using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName ="Items/Consumable")]
public class Consumable : Item
{

    [SerializeField] private Rarity rarity;
    [SerializeField] private string useText = "Use: Something";

    public Rarity Rarity { get { return rarity; } }

    //public override string ColoredName => throw new System.NotImplementedException();
    public override string ColoredName {
        get {
            string hexcolor = ColorUtility.ToHtmlStringRGB(rarity.TextColor);
            return $"<color=#{hexcolor}> { Name } </color>";
        }
    }

    public override string GetToolTipInfoText()
    {
        //throw new System.NotImplementedException();
        // String builder to avoid lots of garbage and garbage collection
        StringBuilder builder = new StringBuilder();

        // Now we build the string
        builder.Append(Rarity.name).AppendLine();
        builder.Append("<color=green>Use: ").Append(useText).Append("</color>").AppendLine();
        builder.Append("Sell Price: ").Append(SellPrice).Append(" credits");

        // Finally we return an unique string, not dozens:
        return builder.ToString();

    }
}

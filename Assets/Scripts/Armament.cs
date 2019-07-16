using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "New Armament", menuName = "Items/Armament")]
public class Armament : Item
{

    [SerializeField] private Rarity rarity;
    [SerializeField] private int damage;
    [SerializeField] private string useText = "Use: ";

    public Rarity Rarity { get { return rarity; } }

    public new ItemType ItemType = ItemType.armament; // Specific to this class



    // TODO: Both ColoredName and GetToolTipInfoText() methods are cloned from the Consumables class, most likely I can abstract this somewhere else as I'll be using it more often.
    public override string ColoredName
    {
        get
        {
            string hexcolor = ColorUtility.ToHtmlStringRGB(rarity.TextColor);
            return $"<color=#{hexcolor}> { Name } </color>";
        }
    }

    public override string GetToolTipInfoText()
    {
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

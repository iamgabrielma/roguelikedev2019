using UnityEngine;

[CreateAssetMenu]
public class Rarity: ScriptableObject
{
    [SerializeField] private new string name;
    [SerializeField] private Color textColor;

    public string Name { get { return name; } }
    public Color TextColor { get { return textColor; } }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Lower inventory class, everything will be a hotbaritem as root */
public abstract class EnhancedHotbarItem : ScriptableObject
{

    [Header("Enhanced Basic Info")]
    [SerializeField] private new string name;
    [SerializeField] private Sprite icon = null;

    public string Name => name;
    public Sprite Icon => icon;
    public abstract string ColoredName { get; } // Old code
    public abstract string GetToolTipInfoText(); // Old code

}

using UnityEngine;

// Deriving from ScriptableObject we don't need a GameObject, as this will add as a template or data container to generate other objects
[CreateAssetMenu(fileName= "New Tile", menuName="Tiles")]
public class Tile: ScriptableObject
{


    public string tileName; // TODO: struct
    public string tileDescription;
    public Sprite tileSprite;
    public GameObject tileObject;

    public bool isWalkable;
    public bool isVisualBlock; // For our FOV implementation later.

    public string InspectTile() {

        return "Inspecting tile... this seems to be " + tileDescription;
    }

    // TODO: In the future we may need to edit the Sprite based on if is within FOV or not.

    /* Eg: Other potential properties */
    //public bool isTileBlocked; // if it’s blocked, you can’t move through it // isWalkable more clear?
    //public bool isTileBlockSight; // whether or not it blocks sight ( FOV )
    //public bool isTileDestructable; // whether or not it can be destroyed
    //public bool isTileDestroyed; // whether or not is destroyed, and for example switch to debrisTile
    //public bool isTileExplored; // Not explored ==> Not instantiated? Something like ComputeFov() on player movement can recalculate this
    //public bool isTileinFov; // Within field of view
    //public string typeOfTile; // TODO: Enum

}

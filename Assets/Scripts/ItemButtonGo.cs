using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/* ToolTip for GameObjects in the scene: Works by click-hold over the object*/
public class ItemButtonGo : MonoBehaviour
{
    [SerializeField] private ToolTipManager toolTip;
    [SerializeField] private Item item;

    // Shows tooltip if click-hold mouse
    public void OnMouseDown ()
    {
        toolTip.DisplayInfo(item);
    }

    // Hides tooltip if mouse is released
    public void OnMouseUp()
    {
        toolTip.HideInfo();
    }

    // Shows tooltip if mouse is over the GameObject and "I" (Inspect) is pressed
    public void OnMouseOver()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            toolTip.DisplayInfo(item);
        }
    }
    // Hides tooltip if mouse exits the GameObject collider area
    public void OnMouseExit()
    {
        toolTip.HideInfo();
    }
}

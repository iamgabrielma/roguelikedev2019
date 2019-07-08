using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/* ToolTip for GameObjects in the scene: Works by click-hold over the object*/
public class ItemButtonGo : MonoBehaviour
{
    [SerializeField] private ToolTipManager toolTip;
    //[SerializeField] private Item item;
    public Item item; // We make it public now so we can reach it from ItemGenerator.

    private void Awake()
    {
        // We assign this on Awake instead of directly in the Prefab due to some bug with calling DisplayInfo.
        if (toolTip == null)
        {
            toolTip = GameObject.FindWithTag("ToolTip").GetComponent<ToolTipManager>();
        }
    }
    // Shows tooltip if click-hold mouse
    public void OnMouseDown ()
    {
        toolTip.DisplayInfo(item);
        Debug.Log("click");
    }

    // Hides tooltip if mouse is released
    public void OnMouseUp()
    {
        toolTip.HideInfo();
        Debug.Log("unclick");
    }

    // Shows tooltip if mouse is over the GameObject and "I" (Inspect) is pressed
    public void OnMouseOver()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            toolTip.DisplayInfo(item);
            Debug.Log("hover + p");
        }
    }
    // Hides tooltip if mouse exits the GameObject collider area
    public void OnMouseExit()
    {
        toolTip.HideInfo();
    }
}

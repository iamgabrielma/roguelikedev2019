using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/* ToolTip for UI Elements in the scene: Works by hovering over the elements */
public class ItemButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private ToolTipManager toolTip;
    [SerializeField] private Item item;

    public void OnPointerEnter(PointerEventData eventData)
    {
        toolTip.DisplayInfo(item);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        toolTip.HideInfo();
    }
}

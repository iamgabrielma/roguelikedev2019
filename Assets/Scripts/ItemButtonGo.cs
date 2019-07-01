using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemButtonGo : MonoBehaviour //, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private ToolTipManager toolTip;
    [SerializeField] private Item item;

    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    //toolTip.DisplayInfo(item);
    //    Debug.Log("Enter");
    //}

    public void OnMouseUp()
    {
        toolTip.HideInfo();
    }

    public void OnMouseDown ()
    {
        Debug.Log("Clicking in GO "); // OK
        toolTip.DisplayInfo(item);

    }

    // TESTING FOR GAMEOBJECTS:
    //public void OnPointerDown(PointerEventData eventData)
    //{
    //    Debug.Log("Clicking in GO");
    //}
    //public void OnMouseDown ()
    //{
    //    Debug.Log("Clicking in GO 2");
    //}
}

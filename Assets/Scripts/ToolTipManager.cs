using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ToolTipManager : MonoBehaviour
{

    //[SerializeField] private GameObject toolTipCanvasObject; // So we can reference it for enable/disable
    public GameObject toolTipCanvasObject; // So we can reference it for enable/disable
    [SerializeField] private RectTransform toolTipObject; // UI
    [SerializeField] private Text infoText;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float padding; // So when the toolTip goes to the borders is still readable and stays within the screen

    private Canvas toolTipCanvas;

    private void Awake()
    {
        toolTipCanvas = toolTipCanvasObject.GetComponent<Canvas>();
    }

    private void Update()
    {
        // We want this to follow the cursor
        FollowCursor();
    }

    private void FollowCursor()
    {
        if (!toolTipCanvasObject.activeSelf) { return; }

        Vector3 newPos = Input.mousePosition + offset;
        newPos.z = 0f;
        float rightEdgeToScreenEdgeDistance = Screen.width - (newPos.x + toolTipObject.rect.width * toolTipCanvas.scaleFactor / 2) - padding;
        if (rightEdgeToScreenEdgeDistance < 0)
        {
            newPos.x += rightEdgeToScreenEdgeDistance;
        }
        float leftEdgeToScreenEdgeDistance = 0 - (newPos.x - toolTipObject.rect.width * toolTipCanvas.scaleFactor / 2) + padding;
        if (leftEdgeToScreenEdgeDistance > 0)
        {
            newPos.x += leftEdgeToScreenEdgeDistance;
        }
        float topEdgeToScreenEdgeDistance = Screen.height - (newPos.y + toolTipObject.rect.height * toolTipCanvas.scaleFactor) - padding;
        if (topEdgeToScreenEdgeDistance < 0)
        {
            newPos.y += topEdgeToScreenEdgeDistance;
        }

        toolTipObject.transform.position = newPos;
    }

    public void DisplayInfo(Item item) {

        if (toolTipCanvas == null)
        {
            toolTipCanvas = GameObject.FindWithTag("ToolTip").GetComponent<Canvas>();
        }

        Debug.Log("called successfully");
        StringBuilder builder = new StringBuilder();

        builder.Append("<size=14>").Append(item.ColoredName).Append("</size>").AppendLine();
        builder.Append(item.GetToolTipInfoText());

        infoText.text = builder.ToString(); // We set it in the UI

        toolTipCanvasObject.SetActive(true); // So we activate it
        LayoutRebuilder.ForceRebuildLayoutImmediate(toolTipObject); // This resizes correctly the RectTransform we pass through
    }

    public void HideInfo() {

        toolTipCanvasObject.SetActive(false); // So we hide it

    }

}

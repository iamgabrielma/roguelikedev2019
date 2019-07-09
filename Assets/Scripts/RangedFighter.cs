using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedFighter : MonoBehaviour
{
    // GetCurrentMouseVectorPosition
    private Camera cam;
    private Canvas targetLockCanvas;
    public GameObject targetLockCanvasObject; // So we can reference it for enable/disable

    GridGenerator grid;

    // SetMouseToRangedLockTexture
    //public Texture2D cursorTexture;
    //private CursorMode cursorMode = CursorMode.Auto;

    void Start()
    {
        cam = Camera.main;
        targetLockCanvas = targetLockCanvasObject.GetComponent<Canvas>();

        grid = FindObjectOfType<GridGenerator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) // (T)arget
        {
            GetCurrentMouseVectorPosition();
            //CheckIfObjectWhereMouseClick();

        }

    }

    Vector2 GetCurrentMouseVectorPosition()
    {

        /*
        A world space point is defined in global coordinates (for example, Transform.position).
        A screen space point is defined in pixels. The bottom-left of the screen is (0,0); the right-top is (pixelWidth,pixelHeight).
        Camera.ScreenToWorldPoint transforms position from screen space into world space.
        */
        Vector2 point = new Vector2();
        Vector2 currentMousePosition = new Vector2();
        currentMousePosition = Input.mousePosition;

        point = cam.ScreenToWorldPoint(new Vector2(currentMousePosition.x, currentMousePosition.y));

        Debug.Log(point);
        //SetVectorToTexture(point);
        CheckIfObjectWhereMouseIs(point);

        return point;
    }

    void SetVectorToTexture(Vector2 point)
    {

        //grid.listOfEnemyEntities.Contains
        //if (targetLockCanvas == null)
        //{
        //    targetLockCanvas = GameObject.FindWithTag("Canvas").GetComponent<Canvas>();
        //}
        // GetCurrentMouseVectorPosition() returns a very specific point, like 52.5,56.7 , we want something like 52,57. 
        Vector2 lowerCoordinates = new Vector2(Mathf.Floor(point.x), Mathf.Floor(point.y));
        //Vector2 upperCoordinates = new Vector2(lowerCoordinates.x + 1, lowerCoordinates.y + 1);


    }

    void CheckIfObjectWhereMouseIs(Vector2 _point)
    {
        RaycastHit2D hit = Physics2D.Raycast(_point, Vector2.zero);
        //Vector2.zero as the direction of the Raycast to ensure only objects located directly at the point of the click are detected:
        if (hit.collider != null)
        {
            Debug.Log("Something was clicked!");
            Debug.Log(hit.collider.gameObject.name);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedFighter : MonoBehaviour
{
    private Camera cam;
    //public GameObject targetLockCanvasObject; // So we can reference it for enable/disable

    public GameObject aim;
    private Sprite aimSprite;

    private bool targetLocked;

    [SerializeField] private GameObject playerReference;
    [SerializeField] private GameObject enemyTarget;
    //GridGenerator grid;

    void Start()
    {
        playerReference = Engine.__player;
        targetLocked = false;
        enemyTarget = null;
        cam = Camera.main;

        aimSprite = aim.GetComponent<SpriteRenderer>().sprite;
        HideAim();

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && targetLocked == false) // (T)arget
        {
            GetCurrentMouseVectorPosition();
            ShowAim();
            targetLocked = true;
            //TargetLocked();
            //CheckIfObjectWhereMouseClick();

        }
        else if (Input.GetKeyDown(KeyCode.T) && targetLocked == true && enemyTarget != null) // Shoot
        {
            targetLocked = false;
            // shoot + pass turn + resolve combat
            // TODO NEEDS AN INTERMEDIATE STEP CONDITION THAT CHECKS IF THERE'S WALLS IN THE MIDDLE OR ENEMY IS WITHIN RANGE.
            // TODO NEEDS TO SKIP TURN AS WELL ONCE ATTACK IS COMPLETED AND RESOLVED.
            ShootEffect();
            HideAim();
        }
        else if(Input.GetKeyDown(KeyCode.T) && targetLocked == true && enemyTarget == null)
        {
            targetLocked = false;
            HideAim();
            MessageLogManager.Instance.AddToQueue("No target");
        }

        if (targetLocked == true && Input.GetKeyDown(KeyCode.Escape))
        {
            targetLocked = false;
            HideAim();
            MessageLogManager.Instance.AddToQueue("Lock cancelled");
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

        aim.transform.position = point; //Setting GO to this position

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
            //Debug.Log("Something was clicked!");
            MessageLogManager.Instance.AddToQueue("Target " + hit.collider.gameObject.name + " Locked!");
            Debug.Log(hit.collider.gameObject.name);

            if (hit.collider.gameObject.tag == "Enemy")
            {
                enemyTarget = hit.collider.gameObject;
            }
            else
            {
                enemyTarget = null;
            }

        }
    }

    void ShowAim()
    {

        aim.SetActive(true);
    }
    void HideAim()
    {

        aim.SetActive(false);

    }

    //void TargetLocked()
    //{
    //    // Target locked!


    //}

    void ShootEffect()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        DrawLine(gameObject.transform.position, new Vector3(ray.origin.x, ray.origin.y, 0), Color.red, 0.3f);
        MessageLogManager.Instance.AddToQueue("piu piu! shooting piu piu!");

        if (playerReference == null)
        {
            playerReference = GameObject.FindWithTag("Player");
        }
        //Entity.ResolveRangedAttack(playerReference, enemyTarget, Entity.EntityMode.CombatEngaged);
        Entity.ResolveDefense(playerReference, enemyTarget);

        GameStateManager.__gameTimeTicks++; // Adds a tick to Game Time.
        GameStateManager.__gameState = GameStateManager.GameState.enemyTurn; // Finished our turn, is enemy turn.

    }

    void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        //lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.SetColors(color, color);
        lr.SetWidth(0.1f, 0.1f);
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        // Add gradient?
        Destroy(myLine, duration);
    }

}

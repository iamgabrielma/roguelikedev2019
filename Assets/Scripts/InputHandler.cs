using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{

    public bool IsPlayerTurn { get; set; } // Property

    public void EndPlayerTurn()
    {
        IsPlayerTurn = false;
    }

    /* InputHandler may be a limited name, as at the moment deals with Player logic as well */


    private bool isPlayerMoving; // This avoids that there's multiple movement until the previous movement hasn't completed. While isPlayerMoving = True, no new movements can start.
    //private bool isPlayerMovementTurn; // Allows/Disallows the Player to perform movement based on the Game State.

    GameObject player; // Self reference, in order to use MovePlayer() easily
    InventoryManager playerInventoryReference;
    private Vector3 _lastKnownPlayerPosition; // We use this to track players last position before their next move

    public static bool isFOVrecompute; // If True, FOV is recomputed on GridGenerator.cs (temporary, maybe this needs to go into GameMap or something)
    //public static MessageLog MessageLog { get; private set; }
    private bool isPlayerOnTopOfExitTile;


    private void Start()
    {
        // TODO: add nullcheck. We assign player to the static __player instance, shouldn't be null but check for safety.
        player = Engine.__player;
        if (player = null)
        {
            player = GameObject.FindWithTag("Player");
        }
        player = Engine.__player;
        isFOVrecompute = true; // When the player appears for first time, we need to calculate the initial FOV
        isPlayerOnTopOfExitTile = false;
    }

    private void Update()
    {
        if (GameStateManager.__gameState == GameStateManager.GameState.playerTurn)
        {
            IsPlayerTurn = true;
        }
        else
        {
            IsPlayerTurn = false;
        }
        // MOVEMENT
        if (Input.GetKeyDown(KeyCode.W) && isPlayerMoving == false && IsPlayerTurn)
            {
                isPlayerMoving = true;
                MovePlayer("up");
            }
            else if (Input.GetKeyDown(KeyCode.S) && isPlayerMoving == false && IsPlayerTurn)
            {
                isPlayerMoving = true;
                MovePlayer("down");
            }
            else if (Input.GetKeyDown(KeyCode.D) && isPlayerMoving == false && IsPlayerTurn)
            {
                isPlayerMoving = true;
                MovePlayer("right");
            }
            else if (Input.GetKeyDown(KeyCode.A) && isPlayerMoving == false && IsPlayerTurn)
            {
                isPlayerMoving = true;
                MovePlayer("left");
            }
        // USING ITEMS DEACTIVATED FOR NOW UNTIL I CAN RETHINK THE INVENTORY SYSTEM
        //if (Input.GetKeyDown(KeyCode.Alpha0) && isPlayerMoving == false && IsPlayerTurn)
        //{
        //    // Check if item is in inventory, otherwise skip:
        //    if (gameObject.GetComponent<InventoryManager>().itemsInInventory[0] != null)
        //    {
        //        isPlayerMoving = true; // While is not moving we activate this to avoid multiple actions
        //        Debug.Log("Using item assigned to 0");
        //        UseItem(0);
        //    }
        //    else
        //    {
        //        Debug.Log("No item in [0]");
        //    }

        //}

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameObject tempObject = GameObject.Find("Canvas");
            var menu = tempObject.GetComponent<_testingUIElements>();
            menu.ToggleMenu();
        }

        if (Input.GetKeyDown(KeyCode.Less) && isPlayerOnTopOfExitTile == true )
        {
            Debug.Log("Discovering new depths");
            GridGenerator.__generateNextFloor = true; // Switches the generateNewFloor to True and the rest is dealt within the Grid Generator.
        }

    }

    public void UseItem(int _numericKeycode) {

        // TODO: WIP, still figuring this out. Something like "if item position x in inventory is not null, use it".
        playerInventoryReference = player.gameObject.GetComponent<InventoryManager>();
        //InventoryManager.InputHandlerAndUseItem(player, _numericKeycode);
        playerInventoryReference.InputHandlerAndUseItem(player, _numericKeycode);

        EndOfPlayerTurn(); //We need to end the turn if the player decides to use an item
    }

    public void MovePlayer(string direction)
    {
        _lastKnownPlayerPosition = player.transform.localPosition; 

        switch (direction)
        {
            case "up":
                float yPositive = player.transform.localPosition.y + 1.0f;
                player.transform.localPosition = new Vector3(player.transform.localPosition.x, yPositive, 0);
                break;
            case "down":
                float yNegative = player.transform.localPosition.y - 1.0f;
                player.transform.localPosition = new Vector3(player.transform.localPosition.x, yNegative, 0);
                break;
            case "right":
                float xPositive = player.transform.localPosition.x + 1.0f;
                player.transform.localPosition = new Vector3(xPositive, player.transform.localPosition.y, 0);
                break;
            case "left":
                float xNegative = player.transform.localPosition.x - 1.0f;
                player.transform.localPosition = new Vector3(xNegative, player.transform.localPosition.y, 0);
                break;
            case "none":
                Debug.Log("There's a wall there, not walkable.");
                break;

        }

        EndOfPlayerTurn();

    }

    // After a movement, or other player action, we end the turn
    private void EndOfPlayerTurn()
    {
        isFOVrecompute = true; // When a movement is success, then we recompute FOV
        isPlayerMoving = false;
        IsPlayerTurn = false;

        GameStateManager.__gameTimeTicks++; // Adds a tick to Game Time.
        GameStateManager.__gameState = GameStateManager.GameState.enemyTurn; // Finished our turn, is enemy turn.
    }

    // TODO: Move this to PlayerBrain.cs
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // This checks if the player has touched a wall or enemy, if does, "teleports" the player to its last position, as cannon be crossed. This is done this way because physics and the rigidbody moves the player to a float position after colliding, breaking the logic afterwards as movement relies on integer vectors
        // Elements must be "isTrigger" and "Static collider" at the moment for this to work, if changes, still breaks the player position.
        if (collision.tag == "Wall" || collision.tag == "Enemy")
        {
            player.transform.position = new Vector3(_lastKnownPlayerPosition.x, _lastKnownPlayerPosition.y, 0);
        }

        // TODO: Bump attack when the player hits the enemy by hittings its tile
        if (collision.tag == "Enemy")
        {
            //Entity _enemy = new Entity((int)collision.transform.localPosition.x, (int)collision.transform.localPosition.y, "Enemy", _test_npc, new Vector3(_randomVector.x, _randomVector.y, 0)); ;
            Debug.Log("Bump attack: resolving defense!");
            Entity.ResolveDefense(player, collision.gameObject);
            Entity.ResolveDeath(player, collision.gameObject);

        }

        if (collision.tag == "Item")
        {
            Entity.ResolveItem(player, collision.gameObject);
        }

    }

    // Exit logic. This must be this way because the collisions is called on FixedUpdate, which may not coincide with your key press and not trigger, so we have a bool switch and check for changes in Update() instead.
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Exit")
        {
            isPlayerOnTopOfExitTile = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Exit")
        {
            isPlayerOnTopOfExitTile = false;
        }
    }

    public void ActivateEnemies()
    {
        IScheduleable scheduleable = Engine.SchedulingSystem.Get(); // In theory this must return the player, or the enemies, but returns null. Fixed.
        //Debug.Log(scheduleable);
        if (scheduleable is Entity) // TODO: We'll need to create more classes that inherit from Entity, otherwise only this will be used.
        {
            IsPlayerTurn = true;
            Debug.Log("Player turn!");
            Engine.SchedulingSystem.Add(scheduleable);
        }
    }
}

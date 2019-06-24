using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStateManager: MonoBehaviour
{
    public GameObject playerReference;
    public enum GameState // possible states
    {
        //noTurns,
        playerTurn,
        enemyTurn
    }
    public static GameState __gameState; // current state
    public static int __gameTimeTicks; // Starting with events queue.
    public GameState previousGameState; // 1 - Declare

    List<Entity> gridReference = new List<Entity>();
    List<Entity> updatedEntities = new List<Entity>();

    public Text textGameTimeTicks;



    public void Start()
    {
     

        // Create a reference to the Player
        playerReference = Engine.__player;
        if (playerReference == null)
        {
            playerReference = GameObject.FindWithTag("Player");
        }

        // Initial game state is set to playerTurn
        //previousGameState = GameState.noTurns;
        __gameTimeTicks = 0;   // Starts with gameTime = 0
        textGameTimeTicks.text = __gameTimeTicks.ToString();
        __gameState = GameState.playerTurn; // Initial state
        SetGameState(__gameState);

        // 2 - Assign
        gridReference = FindObjectOfType<GridGenerator>().GetComponent<GridGenerator>().listOfEnemyEntities;


    }

    // 4 - get'em
    private void GetCurrentNumberOfEnemies(List<Entity> gridReference) {

        // Get reference to the current entities

        // We keep track of this here, so doesn't affect the gridgenerator and we can play with state easier.
        Debug.Log("number of entities: " + gridReference.Count);
        //foreach (var item in gridReference)
        //{
        //    // Testing basic enemy movement.
        //    //float yPositive = item.transform.localPosition.y + 1.0f;
        //    //item.transform.localPosition = new Vector3(item.transform.localPosition.x, yPositive, 0);
        //    //Debug.Log(item.name + " | vector: " + item.transform.localPosition.ToString()); // Completely wrong, wtf. 

        //    //Attempt 2, passed Vector3 instead of the gameobjects.
        //    Debug.Log(item.ToString()); // Now is correc, but I cannot use it.

        //    // Atempt 3, passed entities.
        //    Debug.Log(item.entityLocation.ToString()); // Works, and can use.
        //    //Testing basic enemy movement.
        //    float yPositive = item.entityLocation.x + 1;
        //    item.entityLocation = new Vector3(item.entityLocation.x, yPositive, 0);
        //    updatedEntities.Add(item);
        //    // TODO: current problem, only updates once because from that point takes again the items with its original x, and y from the list.
        //    // When working with structs you need to get the contents of the list item - modify it and then put it back again.
        //    // Alternative solution: https://answers.unity.com/questions/318922/why-cant-i-directly-change-the-value-of-a-list-ele.html
        //    //  What you can do is, you can set i directly to the value you wish. I can't tdo this with foreach and has to be done with for loop :
        //}

        for (int i = 0; i < gridReference.Count; i++)
        {
            Debug.Log(gridReference[i].entityLocation.ToString());
            float yPositive = gridReference[i].entityLocation.y + 1;
            gridReference[i].entityLocation = new Vector3(gridReference[i].entityLocation.x, yPositive, 0);
            Debug.Log(gridReference[i].entityLocation.ToString());
            // This works and the valies change, but not the gameobject location.
        }
    }

    private void Update()
    {
        // If 
        //if (InputHandler.isFOVrecompute == true)
        //{
            textGameTimeTicks.text = "Time: " + __gameTimeTicks.ToString();
        //}

        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("__gameState: " + __gameState.ToString());
            Debug.Log("__gameTimeTicks: " + __gameTimeTicks.ToString());
            GetCurrentNumberOfEnemies(gridReference); // 3 - Pass as parameter

        }

        // Tring to get the object and change its vector to update it to the result of GetCurrentNumberOfEnemies(gridReference);
        for (int i = 0; i < gridReference.Count; i++)
        {
            //Vector3 newPos = gridReference[i].entityGameObject.GetComponent<Transform>().localPosition;

            Vector3 newPos = new Vector3(gridReference[i].entityLocation.x, gridReference[i].entityLocation.y, 0); // new position for each object.
            gridReference[i].entityLocation = new Vector3(newPos.x, newPos.y, 0); // change its transform to the new position
            // Not working :D
        }


        //if (__gameState == GameState.enemyTurn)
        //{
        //Debug.Log("State 2: Enemy turn");
        //Debug.Log("State 3: Setting back to player turn");
        //__gameState = GameState.playerTurn;
        //}
        // If there's no change in Game State, do nothing.
        //if (__gameState == previousGameState)
        //{
        //    return;
        //}

        /* While */
        //if (__gameState == GameState.playerTurn)
        //{
        //    SetGameState(GameState.playerTurn);
        //    //previousGameState = GameState.playerTurn;
        //}
        //} else if (__gameState == GameState.enemyTurn)
        //{
        //    __gameState = GameState.playerTurn; // Switch it back
        ////    SetGameState(GameState.enemyTurn);
        ////    previousGameState = GameState.enemyTurn;
        //}
    }

    public void SetGameState( GameState newGameState) {

        // Do a check to see if the gamestate is new or do nothing (maybe?) You could have multiple turns if fast enough.

        switch (newGameState)
        {
            case GameState.playerTurn:
                //Debug.Log("Player turn");
                //Debug.Log("Time:" + __gameTimeTicks);
                //__gameState = GameState.enemyTurn;
                break;
            case GameState.enemyTurn:
                //Debug.Log("Enemy turn");
                //StartCoroutine(_temp_enemy_turn());
                // myObject.GetComponent<MyScript>().MyFunction(); for the enemies.
                break;
        }
    }

    // This shouldn't be here, but on EnemyAI.
    //private IEnumerator _temp_enemy_turn()
    //{
    //    Debug.Log("Enemy turn: Enemies think about their own existence");

    //    yield return new WaitForSeconds(3);
    //    __gameState = GameState.playerTurn;
    //}

}

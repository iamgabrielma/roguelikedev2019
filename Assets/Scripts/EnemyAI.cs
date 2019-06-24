using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public bool enemyCanMove;
    public Transform enemyReference; //Self refence
    // Start is called before the first frame update
    public int health;

    void Start()
    {
        //enemyCanMove = true;
        enemyCanMove = false;
        health = 3; // TODO: rename EnemyAi to something else
        //enemyReference = gameObject.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: Check for __gameticks instead and compare with inner clock. Variable _bornInTimestamp = __gametick to compare this and the difference for actions.
        if (GameStateManager.__gameState == GameStateManager.GameState.enemyTurn && enemyCanMove == false)
        {
            Debug.Log("State 2.1: Enemy turn. This was called as well!");
            _temp_MoveEnemy(4); // Moves enemy down.
            enemyCanMove = true; // So we avoid calling this function more than one by checking the game state alone
        }
        //if (GameStateManager.__gameState == GameStateManager.GameState.enemyTurn && enemyCanMove == false)
        //{

        //    int _rand = Random.Range(1,4);
        //    _temp_MoveEnemy(_rand);
        //    //isEnemyActing = false;
        //    enemyCanMove = true;
        //}
    }

    public void _temp_MoveEnemy(int _rand_direction)
    {


        switch (_rand_direction)
        {
            case 1:
                float yPositive = enemyReference.localPosition.y + 1.0f;
                enemyReference.transform.localPosition = new Vector3(enemyReference.transform.localPosition.x, yPositive, 0);
                break;
            case 2:
                float yNegative = enemyReference.transform.localPosition.y - 1.0f;
                enemyReference.transform.localPosition = new Vector3(enemyReference.transform.localPosition.x, yNegative, 0);
                break;
            case 3:
                float xPositive = enemyReference.localPosition.x + 1.0f;
                enemyReference.localPosition = new Vector3(xPositive, enemyReference.localPosition.y, 0);
                break;
            case 4:
                float xNegative = enemyReference.localPosition.x - 1.0f;
                enemyReference.localPosition = new Vector3(xNegative, enemyReference.localPosition.y, 0);
                break;

        }

        // Once enemies moved, is the player turn:
        // Problem: Multiple enemies cause multiple changes of state, prone to error. I should read the state only from this class, not set a new one.
        Debug.Log("State 2.2: Enemy movement has finished.");
        //GameStateManager.__gameState = GameStateManager.GameState.playerTurn; // Player turn
        //enemyCanMove = true; // able to move again as soon as the game state changes back to enemy turn . This cannot happen before the state changes in the previous line, or we'll execute the Update()  again

    }
}

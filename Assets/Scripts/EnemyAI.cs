using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour, IFight // IFight implementation temporary until final decision.
{
    public bool enemyCanMove;
    public GameObject enemy; //Self refence
    public Transform enemyTransform; //Self transform

    public int health;
    private bool isEnemyTimeToDoOneAction;

    private Vector3 _lastKnownEnemyPosition;

    void Start()
    {
        //enemyCanMove = true;
        //enemyCanMove = false;
        health = 3; // TODO: rename EnemyAi to something else
        isEnemyTimeToDoOneAction = false; // by default, enemy is idle.
        //enemyReference = transform.parent.gameObject;
        //enemyTransform = enemyReference.transform;
        // TODO nullcheck
        if (enemy != null)
        {
            enemy = transform.parent.gameObject; // initial transform value
        }

    }

    // Update is called once per frame
    void Update()
    {
        // TODO: Check for __gameticks instead and compare with inner clock. Variable _bornInTimestamp = __gametick to compare this and the difference for actions.
        if (GameStateManager.__gameState == GameStateManager.GameState.enemyTurn && isEnemyTimeToDoOneAction == false)
        {
            _lastKnownEnemyPosition = enemy.transform.localPosition;

            // TODO nullcheck
            isEnemyTimeToDoOneAction = true; // We switch this to true so stops after just this one
            GameObject newenemypost = this.gameObject;
            EnemyMoves(newenemypost);
            Debug.Log("State 2.1: Enemy turn. This was called as well!");

        }

    }

    void EnemyMoves(GameObject _newenemypost) {

        // TODO: Check for valid walls/floors around before performing a movement. Before fixing this I'll fix the map interconnected rooms as I have to get valid tiles from there already.
        // TODO: Will also need the above to implement A*
        Entity.TestMove(_newenemypost, Entity.EntityMode.Wander, 1.0f, 0.0f);

        isEnemyTimeToDoOneAction = false; // We switch this to false so they can move again once is their turn


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // This checks if the player has touched a wall or enemy, if does, "teleports" the player to its last position, as cannon be crossed. This is done this way because physics and the rigidbody moves the player to a float position after colliding, breaking the logic afterwards as movement relies on integer vectors
        // Elements must be "isTrigger" and "Static collider" at the moment for this to work, if changes, still breaks the player position.
        if (collision.tag == "Wall" || collision.tag == "Enemy")
        {
            enemy.transform.position = new Vector3(_lastKnownEnemyPosition.x, _lastKnownEnemyPosition.y, 0);
        }

        // TODO: Bump attack when the player hits the enemy by hittings its tile
        //if (collision.tag == "Enemy")
        //{
        //    //Entity _enemy = new Entity((int)collision.transform.localPosition.x, (int)collision.transform.localPosition.y, "Enemy", _test_npc, new Vector3(_randomVector.x, _randomVector.y, 0)); ;
        //    Debug.Log("Bump attack: resolving defense!");
        //    Entity.ResolveDefense(player, collision.gameObject);
        //    Entity.ResolveDeath(collision.gameObject);

        //}
    }

    //public void _temp_MoveEnemy(int _rand_direction)
    //{


    //    switch (_rand_direction)
    //    {
    //        case 1:
    //            float yPositive = enemyReference.localPosition.y + 1.0f;
    //            enemyReference.transform.localPosition = new Vector3(enemyReference.transform.localPosition.x, yPositive, 0);
    //            break;
    //        case 2:
    //            float yNegative = enemyReference.transform.localPosition.y - 1.0f;
    //            enemyReference.transform.localPosition = new Vector3(enemyReference.transform.localPosition.x, yNegative, 0);
    //            break;
    //        case 3:
    //            float xPositive = enemyReference.localPosition.x + 1.0f;
    //            enemyReference.localPosition = new Vector3(xPositive, enemyReference.localPosition.y, 0);
    //            break;
    //        case 4:
    //            float xNegative = enemyReference.localPosition.x - 1.0f;
    //            enemyReference.localPosition = new Vector3(xNegative, enemyReference.localPosition.y, 0);
    //            break;

    //    }

    //    // Once enemies moved, is the player turn:
    //    // Problem: Multiple enemies cause multiple changes of state, prone to error. I should read the state only from this class, not set a new one.
    //    Debug.Log("State 2.2: Enemy movement has finished.");
    //    //GameStateManager.__gameState = GameStateManager.GameState.playerTurn; // Player turn
    //    //enemyCanMove = true; // able to move again as soon as the game state changes back to enemy turn . This cannot happen before the state changes in the previous line, or we'll execute the Update()  again

    //}
}

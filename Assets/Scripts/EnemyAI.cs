using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public bool enemyCanMove;
    public GameObject enemy; //Self refence
    public Transform enemyTransform; //Self transform
    public GameObject playerReference;// player reference

    public int health;
    private bool isEnemyTimeToDoOneAction;

    private Vector3 _lastKnownEnemyPosition;

    //public bool isEnemyAttacked;

    Entity.EntityMode _currentEntityMode;

    void Start()
    {

        health = 3; // TODO: rename EnemyAi to something else
        isEnemyTimeToDoOneAction = false; // by default, enemy is idle.
        _currentEntityMode = Entity.EntityMode.Wander;

        // TODO nullcheck
        if (enemy == null)
        {
            enemy = transform.parent.gameObject; // initial transform value
        }

        if (playerReference == null)
        {
            playerReference = Engine.__player;
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
            //EnemyMoves(newenemypost);
            // Updates status from Alert or Wander to Agressive:

            /* ## Important: this only happens here, not on Update() ##*/
            CheckCurrentState(newenemypost); // We change EnemyMoves() for CheckCurrentState()


        }

    }

    // If is enemy turn, check their current state to decide which action will be next:
    void CheckCurrentState(GameObject _newenemypost)
    {
        // TODO switch statement + functions better organized?
        // Default mode = wander

        // If player is within reach: Entity.Alerted -> IsEntityAlerted()
        bool isEnemyAlerted = IsEntityAlerted();
        bool isEnemyAttacked = IsEnemyAtacked();
        //isEnemyAttacked = IsEnemyAtacked();

        if (isEnemyAlerted && isEnemyAttacked)
        { 
            // If enemy is attacked, will attack back.
            Entity.ResolveAttack(_newenemypost, playerReference, Entity.EntityMode.CombatEngaged);
        }
        else if (isEnemyAlerted)
        {
            Entity.Alert(_newenemypost, Entity.EntityMode.Alerted);
        }
        else
        {
            // if is not, Entity.Wander
            Entity.Move(_newenemypost, Entity.EntityMode.Wander, 1.0f, 0.0f);
        }

        isEnemyTimeToDoOneAction = false; // We switch this to false so they can move again once is their turn
    }
    //void EnemyMoves(GameObject _newenemypost) {
    
    //    Entity.TestMove(_newenemypost, Entity.EntityMode.Wander, 1.0f, 0.0f);

    //    isEnemyTimeToDoOneAction = false; // We switch this to false so they can move again once is their turn


    //}
    bool IsEnemyAtacked() {

        if (gameObject.GetComponent<Fighter>().isAgressive == true)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    bool IsEntityAlerted()
    {

        //Vector3 _entityLocation = new Vector3(enemy.entityLocation.x, enemy.entityLocation.y, 0); // Gets the position vector of each entity
        List<Vector2> _entityVisibilityArea = new List<Vector2>();
        _entityVisibilityArea.Clear(); // Reset for safely from prvious iterations

        // // player coord: x53.5 y 53.5 -- . Area has 121 items: pero son int tipo 40,75, no tienen decimales. We use Mathf.Floor to remove this 0.5f bit
        Vector2 _playerCoordinates = new Vector2(Mathf.Floor(playerReference.transform.localPosition.x), Mathf.Floor(playerReference.transform.localPosition.y));
        //Debug.Log("_playerCoordinates" + _playerCoordinates.ToString());
        //Debug.Log("player ref" + playerReference.transform.position.ToString());

        int _entityVisibilityRadius = 5; // 
                
        int _offsetQuadrant4 = (int)enemyTransform.localPosition.x - _entityVisibilityRadius;
        int _offsetQuadrant1 = (int)enemyTransform.localPosition.y - _entityVisibilityRadius;
                                                                                        
        for (int x = _offsetQuadrant4; x < enemyTransform.localPosition.x + _entityVisibilityRadius; x++) // from x-5 to x+5
        {
            for (int y = _offsetQuadrant1; y < enemyTransform.localPosition.y + _entityVisibilityRadius; y++) // from y-5 to y+5
            {
                 _entityVisibilityArea.Add(new Vector2(x, y));
            }
        }


        if (_entityVisibilityArea.Contains(_playerCoordinates)) 
        {
            Debug.Log("Enemy alerted");
            return true;
        }
        else 
        {
            //Debug.Log("Enemy NOT alerted");
            return false; 
        }
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

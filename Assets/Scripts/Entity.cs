using UnityEngine;
using System.Collections.Generic; // Lists

// inherits from Monobehavior so we can use Destroy()
// inherits from ISchedulable so can be added to our scheduling system
public class Entity : MonoBehaviour //, IScheduleable
{
    public int x;
    public int y;
    public string entityName;
    public GameObject entityGameObject;
    public Vector3 entityLocation;
    public bool isBlockingEntity; // This will differentiate if is a physic body or we can pass through (potion), without need to check colliders or is trigger.
    //public int health;


    // If sentient ... possibly needs a new class inherits from this one Sentient : Entity , and we add Move() there.
    public enum EntityMode
    {
        Wander, // Done
        Hunt, // TODO // A*
        Sleep, // TODO Change to start the game as Sleep or Wander, wakes up if player is near.
        Alerted, // Done . If player within FOV, enemy changes from wander to alerted .
        CombatEngaged // TODO: WIP combat started. Once receive or do damage.
    }

    private Entity _currentStatus;

    // If sentient
    //enum EntityStatus
    //{
    //    Healthy,
    //    Confused,
    //    Allucinating,
    //    Paralysed,
    //    Lit
    //}

    // Entity constructor
    public Entity(int aX, int aY, string aName, GameObject aEntityGameObject, Vector3 aEntityLocation ) {

        x = aX;
        y = aY;
        //entityName = aName;
        entityName = aName;
        entityGameObject = aEntityGameObject;
        entityLocation = reallocateEntity(aX, aY);

        //health = 3; // Base health and speed and then ++ based on subclasses?

        // Reallocates the Entities to fit the tiles by offsetting its position +0.5f in X and Y
        Vector3 reallocateEntity(int ax, int ay)
        {
            return new Vector3(ax + 0.5f, ay + 0.5f, 0);
        }

        // This doesn't work here, as happens before the instance.
        //string renameEntity(GameObject entityObject, string entityName) {

        //    //entityName = entityObject.tag;

        //    if (entityObject.tag == "Player(Clone)")
        //    {
        //        entityName = entityObject.tag;
        //    }

        //    return entityName;
        //}




        // TODO: Add: Awareness
        // TODO: if submarines theme: oxygen/energy/temperature/pressure -> Hunger clock oxygen/energy . Pressure++ when we go next level



    }

    public GameObject TestGameObject() {

        return gameObject;
    }

    // Make sure the Entity class implements IScheduleable so that we can add them to our scheduling system.int Time()
    //public int Time
    //{
    //    // Error: Does not implement interface member Ischeduleable.Time , this was because I was using () on Time declaration:
    //    // https://stackoverflow.com/questions/15215900/c-sharp-get-accessor-not-recognised
    //    // The () part is incorrect , this will declare a method rather than a property.
    //    get { return speed; }
    //}

    void Move(int dx, int dy) { 

        // TODO: Enemy movement logic
    }

    // TODO MOVE THESE TO FIGHTER.CS
    public void Attack(Entity attacker, Entity defender) {

        // Resolve attack __static

        // Resolve defense __static

        // Resolve damage __static

        // Resolve death __static



    }

    public static void ResolveDefense(GameObject attacker, GameObject defender)
    {

        Debug.Log("## COMBAT MODE: Enabled");
        int _resolved = defender.gameObject.GetComponent<Fighter>().defense - attacker.gameObject.GetComponent<Fighter>().attack;
        Debug.Log("## COMBAT: Attack vs Defense resolved:" + _resolved);
        if (_resolved == 0) // Will be because at the moment both are 10
        {
            float _rand = (int)Random.Range(0, 100);
            if (_rand < 50)
            {
                MessageLogManager.Instance.AddToQueue("Attack blocked!");
                Debug.Log("## COMBAT: Attack blocked!");
            }
            else
            {
                MessageLogManager.Instance.AddToQueue("Attack successful for (1) hit point!");
                Debug.Log("## COMBAT: Attack successful for (1) hit point!"); // TODO this number will be dynamic
                defender.gameObject.GetComponent<Fighter>().health--;
            }
        }

        // Whoever resolves its defense, success or fail, attacking an enemy will put them aggressive back as well in their next turn
        defender.gameObject.GetComponent<Fighter>().isAgressive = true;
        ResolveDeath(defender); // Check if somebody is death after a successful attack

        // Resolve defense means as well that the enemy moves from whatever state they are, to EntityMode.CombatEngaged
        //defender.
        // resolve properly . Attack vs Defense, if success then hit, otherwise nope.
        //defender.gameObject.GetComponent<EnemyAI>().isEnemyAttacked = true;
        //defender.gameObject.GetComponent<EnemyAI>().health--; // TODO this only takes into account emeies, not the player health. 
        //_defenderHealth = _defenderHealth - 1;
        //Debug.Log("_defenderHealth" + defender.gameObject.GetComponent<EnemyAI>().health.ToString());

    }

    public static void ResolveAttack(GameObject attacker, GameObject defender, EntityMode _entityMode) {

        MessageLogManager.Instance.AddToQueue("Entity attacks back!");
        Debug.Log("## COMBAT: Entity attacks back!!");
        ResolveDefense(attacker, defender);

    }

    public static void ResolveDeath(GameObject defender)
    {
        if (defender.gameObject.GetComponent<Fighter>().health <= 0) // TODO: Move this health too to different place, not in enemyAI, either full entity or subclasses
        {
            if (defender.tag == "Player")
            {
                // Run PlayerDeath() function
                defender.SetActive(false);

            }
            else
            {
                // Note 23.06.19 -> Now inherits from Monobehavior so we can use Destroy()
                MessageLogManager.Instance.AddToQueue("Entity has been destroyed");
                Debug.Log("## COMBAT: Entity has been destroyed");
                Debug.Log("## COMBAT MODE: Disabled");
                //Destroy(defender);
                defender.SetActive(false); // TODO Instead of killing them, we can disable them, and reactivate them after x turns. Start a turn checker here and add them to a list:
            }
        }
    }

    public static void Move(GameObject entityThatMoves, EntityMode _entityMode, float dx, float dy)
    {

        Color _wanderColor = new Color32(255, 255, 255, 255);
        entityThatMoves.gameObject.GetComponent<SpriteRenderer>().color = _wanderColor;
        // TODO: refactor
        // dx and dy can be speed, in this case they move 1
        float _dx = entityThatMoves.gameObject.GetComponent<Transform>().localPosition.x; //+ dx;
        float _dy = entityThatMoves.gameObject.GetComponent<Transform>().localPosition.y; //+ dy;

        float _rand = (int)Random.Range(0, 3.99f);
        //Q: Does this gets a new _rand for each entity?  A: Yes.
        //Debug.Log(entityThatMoves.name + " moves " + _rand); 
        //Q: Once assigned, they always take the same direction for all moves? A: No. 
        if (_entityMode == EntityMode.Wander)
        {

            switch (_rand)
            {
                case 0: //up x=0 y++
                    //_dy += 1.0f;
                    entityThatMoves.gameObject.GetComponent<Transform>().localPosition = new Vector3(_dx, _dy +1, 0);
                    break;
                case 1: //down x=0 y--
                    //_dy -= 1.0f;
                    entityThatMoves.gameObject.GetComponent<Transform>().localPosition = new Vector3(_dx, _dy -1, 0);
                    break;
                case 2: //left x-- y=0
                    //_dx -= 1.0f;
                    entityThatMoves.gameObject.GetComponent<Transform>().localPosition = new Vector3(_dx -1, _dy, 0);
                    break;
                case 3://right x++ y=0
                    //_dx += 1.0f;
                    entityThatMoves.gameObject.GetComponent<Transform>().localPosition = new Vector3(_dx +1, _dy, 0);
                    break;
            }
        }



    }


    public static void Alert(GameObject _entity, EntityMode _entityMode) {
        // Entity.Alert(_newenemypost, Entity.EntityMode.Alerted);
        Color _alertColor = new Color32(226,44,10, 255);
        Debug.Log("!!! " + _entity.name + " has been alerted.");
        // TESTING WIP Switch sprite
        _entity.gameObject.GetComponent<SpriteRenderer>().color = _alertColor;
    }

    public void PlayerDeath(GameObject player)
    {

        player.SetActive(false);
        Debug.Log("You die!");
    }

    void AnalizeMapAroundEntity()
    {

        // Before passing the parameters, each entity should check around them for valid tiles, if tile is invalid, roll again or wait:
        //List<Vector2> _globalMapTiles = GridGenerator.listOfFloorTiles; // reference to the global map
        //List<Vector2> _tilesAroundEntity = GridGenerator.listOfFloorTiles; // reference to the tiles around entity

        //Vector2 _ePos = entityThatMoves.gameObject.GetComponent<Transform>().localPosition;
        //float _ey = entityThatMoves.gameObject.GetComponent<Transform>().localPosition.y;

        //Vector2 neighborUp = new Vector2(_ePos.x, _ePos.y + 1);
        //Vector2 neighbordown = new Vector2(_ePos.x, _ePos.y - 1);
        //Vector2 neighborLeft = new Vector2(_ePos.x - 1, _ePos.y);
        //Vector2 neighborRight = new Vector2(_ePos.x + 1, _ePos.y);
        //_tilesAroundEntity.Add(neighborUp);
        //_tilesAroundEntity.Add(neighbordown);
        //_tilesAroundEntity.Add(neighborLeft);
        //_tilesAroundEntity.Add(neighborRight);

        // If the new Vector3(_dx, _dy, 0); is valid, move, if is not, do nothing
        //foreach (var _coordinate in _tilesAroundEntity)
        //{
        //    // These 4 directions can be either floor or wall. If the global FLOOR tiles contains the coordinate, this is valid, can walk
        //    if (_globalMapTiles.Contains(_coordinate))
        //    {
        //        // valid, we use this direction and get out of the loop
        //        float _x = _coordinate.x + _dx;
        //        float _y = _coordinate.y + _dy;
        //        entityThatMoves.gameObject.GetComponent<Transform>().localPosition = new Vector3(_x,_y, 0);
        //        break;
        //    }

        //}
        //entityThatMoves.gameObject.GetComponent<Transform>().localPosition = new Vector3(_dx, _dy, 0);

        //1- Use listOfFloorTiles
        // TODO : Using SetcColor works without set tile, most likely this gets broken from this somwhere along the line of generating the level
        //foreach (var item in listOfFloorTiles)
        //{
        //    Vector2 neighborUp = new Vector2(item.x, item.y + 1);
        //    Vector2 neighbordown = new Vector2(item.x, item.y - 1);
        //    Vector2 neighborLeft = new Vector2(item.x - 1, item.y);
        //    Vector2 neighborRight = new Vector2(item.x + 1, item.y);

        //    // If the up neighbor is in the list as well, means is a floor, do nothing
        //    if (!listOfFloorTiles.Contains(neighborUp)) //|| listOfFloorTiles.Contains(neighbordown) || listOfFloorTiles.Contains(neighborLeft) || listOfFloorTiles.Contains(neighborRight))
        //    {
        //        wallMap.SetColor(new Vector3Int((int)neighborUp.x, (int)neighborUp.y, 0), Color.red);
        //        borders.Add(new Vector2(neighborUp.x, neighborUp.y));
        //        //continue; //skips to the next part, breaking the loop will get us entirely out

        //    }
        //    if (!listOfFloorTiles.Contains(neighbordown)) //|| listOfFloorTiles.Contains(neighbordown) || listOfFloorTiles.Contains(neighborLeft) || listOfFloorTiles.Contains(neighborRight))
        //    {
        //        wallMap.SetColor(new Vector3Int((int)neighbordown.x, (int)neighbordown.y, 0), Color.red);
        //        borders.Add(new Vector2(neighbordown.x, neighbordown.y));
        //        //continue; //skips to the next part, breaking the loop will get us entirely out

        //    }
        //    if (!listOfFloorTiles.Contains(neighborLeft)) //|| listOfFloorTiles.Contains(neighbordown) || listOfFloorTiles.Contains(neighborLeft) || listOfFloorTiles.Contains(neighborRight))
        //    {
        //        wallMap.SetColor(new Vector3Int((int)neighborLeft.x, (int)neighborLeft.y, 0), Color.red);
        //        borders.Add(new Vector2(neighborLeft.x, neighborLeft.y));
        //        //continue; //skips to the next part, breaking the loop will get us entirely out

        //    }
        //    if (!listOfFloorTiles.Contains(neighborRight)) //|| listOfFloorTiles.Contains(neighbordown) || listOfFloorTiles.Contains(neighborLeft) || listOfFloorTiles.Contains(neighborRight))
        //    {
        //        wallMap.SetColor(new Vector3Int((int)neighborRight.x, (int)neighborRight.y, 0), Color.red);
        //        borders.Add(new Vector2(neighborLeft.x, neighborLeft.y));
        //        //continue; //skips to the next part, breaking the loop will get us entirely out

        //    }
        //}

    }

}

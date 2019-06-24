using UnityEngine;

// inherits from Monobehavior so we can use Destroy()
// inherits from ISchedulable so can be added to our scheduling system
public class Entity : MonoBehaviour , IScheduleable
{
    public int x;
    public int y;
    public string entityName;
    public GameObject entityGameObject;
    public Vector3 entityLocation;
    public bool isBlockingEntity; // This will differentiate if is a physic body or we can pass through (potion), without need to check colliders or is trigger.
    public int health;
    public int speed;

    // If sentient ... possibly needs a new class inherits from this one Sentient : Entity , and we add Move() there.
    enum EntityMode
    {
        Wander,
        Hunt,
        Sleep
    }

    // If sentient
    enum EntityStatus
    {
        Healthy,
        Confused,
        Allucinating,
        Paralysed,
        Lit
    }

    // Entity constructor
    public Entity(int aX, int aY, string aName, GameObject aEntityGameObject, Vector3 aEntityLocation ) {

        //x = aX;
        //y = aY;
        entityName = aName;
        entityGameObject = aEntityGameObject;
        health = 3;
        speed = 1;

        // Reallocates the Entities to fit the tiles by offsetting its position +0.5f in X and Y
        Vector3 reallocateEntity(int ax, int ay)
        {

            return new Vector3(ax + 0.5f, ay + 0.5f, 0);

        }

        // Sets the entity tag to "Enemy"
        if (aName != null && aName == "Enemy")
        {
            aEntityGameObject.tag = aName;
        }

        entityLocation = reallocateEntity(aX, aY);


        // TODO: Add: Awareness
        // TODO: if submarines theme: oxygen/energy/temperature/pressure -> Hunger clock oxygen/energy . Pressure++ when we go next level



    }

    // Make sure the Entity class implements IScheduleable so that we can add them to our scheduling system.int Time()
    public int Time
    {
        // Error: Does not implement interface member Ischeduleable.Time , this was because I was using () on Time declaration:
        // https://stackoverflow.com/questions/15215900/c-sharp-get-accessor-not-recognised
        // The () part is incorrect , this will declare a method rather than a property.
        get { return speed; }
    }

    void Move(int dx, int dy) { 

        // TODO: Enemy movement logic
    }


    public void Attack(Entity attacker, Entity defender) {

        // Resolve attack __static

        // Resolve defense __static

        // Resolve damage __static

        // Resolve death __static



    }

    public static void ResolveDefense(GameObject attacker, GameObject defender)
    {

        //defender.health
        //int _defenderHealthHit = 

        // resolve properly . Attack vs Defense, if success then hit, otherwise nope.
        defender.gameObject.GetComponent<EnemyAI>().health--;
        //_defenderHealth = _defenderHealth - 1;
        Debug.Log("_defenderHealth" + defender.gameObject.GetComponent<EnemyAI>().health.ToString());

    }

    public static void ResolveDeath(GameObject defender)
    {
        if (defender.gameObject.GetComponent<EnemyAI>().health <= 0)
        {
            // Note 23.06.19 -> Now inherits from Monobehavior so we can use Destroy()
            Destroy(defender);
        }
    }

}

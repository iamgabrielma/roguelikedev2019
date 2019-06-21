using UnityEngine;

public class Entity
{
    public int x;
    public int y;
    public string name;
    public GameObject entityGameObject;
    public Vector3 entityLocation;
    public bool isBlockingEntity; // This will differentiate if is a physic body or we can pass through (potion), without need to check colliders or is trigger.

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
        name = aName;
        entityGameObject = aEntityGameObject;

        // Reallocates the Entities to fit the tiles by offsetting its position +0.5f in X and Y
        Vector3 reallocateEntity(int ax, int ay)
        {

            return new Vector3(ax + 0.5f, ay + 0.5f, 0);

        }

        entityLocation = reallocateEntity(aX, aY);


        // TODO: Add: Awareness
        // TODO: if submarines theme: oxygen/energy/temperature/pressure -> Hunger clock oxygen/energy . Pressure++ when we go next level

    }

    void Move(int dx, int dy) { 

        // TODO: Enemy movement logic
    }

}

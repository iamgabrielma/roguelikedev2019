using UnityEngine;

public class Entity
{
    public int x;
    public int y;
    public string name;
    public GameObject entityGameObject;

    // Entity constructor
    public Entity(int aX, int aY, string aName, GameObject aEntityGameObject ) {

        x = aX;
        y = aY;
        name = aName;
        entityGameObject = aEntityGameObject;

    }

    void Move(int dx, int dy) { 

        // TODO: Enemy movement logic
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour
{
    // Temporary screen_width & height
    readonly int screen_width = 80;
    readonly int screen_height = 50;

    public static GameObject __player; // TODO -> Question: Is using a static gameobject for player a good idea?

    public void Start()
    {
        // Coordinates where we'll instantiate the player.
        //TODO: Is not clear at the moment if we will use these. Revisit and pass (0,0,0) vector directly to the __player instantiation otherwise
        int player_x = 0;
        int player_y = 0;

        // The playerObject is assigned and the __player instance is created
        GameObject playerObject = Resources.Load<GameObject>("Prefabs/Player"); 
        __player = Instantiate(playerObject, new Vector3(player_x, player_y, 0), Quaternion.identity);

    }


}

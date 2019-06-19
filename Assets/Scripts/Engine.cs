using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using System.Diagnostics;
//using System;

public class Engine : MonoBehaviour
{

    /* DEBUG FUNCTIONS */
    private bool _debug_instantiating_multiple_enemies = false;

    // Temporary screen_width & height
    public Transform _floorHolder;
    public Transform _wallHolder;
    //readonly int screen_width = 20;
    //readonly int screen_height = 20;
    // 30 rows allow each row to be 36 pixels high in 1080p, the most common monitor size.
    //readonly int mapX_width = 106; // eventually Moved to GameMap.cs
    //readonly int mapY_height = 30; // eventually  Moved to GameMap.cs

    public static GameObject __player; // TODO -> Question: Is using a static gameobject for player a good idea?
    public static GameMap __gameMap; // TODO -> Question: Is using a static gameobject for player a good idea? Assuming it should be only one...yes?

    List<Entity> listOfEntities = new List<Entity>(); // To keep track of created Entities
    List<GameObject> listOfGameObjects = new List<GameObject>(); // To keep track of created GameObjects

    public void Start()
    {
        // TODO: Add it from the GameMap class.
        _floorHolder = new GameObject("floorHolder").transform; // Will hold all our floor tiles, so the Inspector is not cluttered with GameObjects
        _wallHolder = new GameObject("wallHolder").transform; // Will hold all our wall tiles, so the Inspector is not cluttered with GameObjects

        // TODO: Move this into a separate h_helper function
        //Stopwatch stopwatch = new Stopwatch();
        //stopwatch.Start();
        //FloorSetup(map_width, map_height);
        //stopwatch.Stop();
        //TimeSpan ts = stopwatch.Elapsed;
        //int _ms = ts.Milliseconds; // Calculated 14ms for a 20x20 via GameMap.cs . 14ms as well for 20x20 instantiating Prefabs. 165ms for 80x80

        //FloorSetup(map_width,map_height); // -> Testing via GameMap.cs
        //WallsSetup(mapX_width, mapY_height);

        // Coordinates where we'll instantiate the player.
        //TODO: Is not clear at the moment if we will use these. Revisit and pass (0,0,0) vector directly to the __player instantiation otherwise
        int player_x = 0;
        int player_y = 0;

        // The playerObject is assigned and the __player instance is created
        GameObject playerObject = Resources.Load<GameObject>("Prefabs/Player"); 

        // TODO: Added +0.5f temporarily to where the player instantiated, most likely we want to generate this through Entity, that already calculates this.
        __player = Instantiate(playerObject, new Vector3(player_x+0.5f, player_y+0.5f, 0), Quaternion.identity);

        // TODO: Testing the Entity class for the player.

        listOfGameObjects.Add(__player);

        // Testing the Entity class for Enemies. Seems a bit convoluted and either would have to go with GO's or with Entities but seems to go.
        GameObject _test_npc = Resources.Load<GameObject>("Prefabs/Enemy");

        // TODO: Check this class, redudant info adding x, y and V3.
        Entity npcInstance = new Entity(1, 1, "Enemy", _test_npc, new Vector3(1,1,0));
        //Instantiate(npcInstance.entityGameObject, new Vector3(npcInstance.x, npcInstance.y, 0), Quaternion.identity);
        // Modified the Entity class to accept a vector3 that relocates the entity 0.5f to fit the tiles: npcInstance.entityLocation
        Instantiate(npcInstance.entityGameObject, npcInstance.entityLocation, Quaternion.identity);

        listOfEntities.Add(npcInstance);
        listOfGameObjects.Add(_test_npc);

        if (_debug_instantiating_multiple_enemies)
        {
            //Debug.Log("#############################------+ DEBUG +------#############################");
            int maxEnemies = 5;

            for (int i = 0; i < maxEnemies; i++)
            {
                GameObject _go = Instantiate(npcInstance.entityGameObject, new Vector3(npcInstance.x+i, npcInstance.y+i, 0), Quaternion.identity);
                listOfGameObjects.Add(_go);
            }

            //Debug.Log("Number of GO's: " + listOfGameObjects.Count.ToString());
        }

    }

    public void FloorSetup(int width, int height)
    {
        GameObject _test_tileObject = Resources.Load<GameObject>("Prefabs/Floor1");

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject _floorTile = Instantiate(_test_tileObject, new Vector3(x, y, 0), Quaternion.identity); // instantiates tiles from x =0 to max width and from y=0 to max height
                _floorTile.transform.SetParent(_floorHolder); // We set each _tile instance to be a child of _floorHolder, so the Inspector is not clutered.
            }
        }
    }

    public void WallsSetup(int width, int height) {

        GameObject _test_wallObject = Resources.Load<GameObject>("Prefabs/Wall1");

        // TODO: For memory sake, shall we draw only the tiles that are visible? Needs Analysis

        // Fills from 20,20 to 20, -1
        for (int y = width; y >= -1 ; y--)
        {
            GameObject _wallTile = Instantiate(_test_wallObject, new Vector3(width, y, 0), Quaternion.identity);
            _wallTile.transform.SetParent(_wallHolder);
        }
        // Fills from -1,20 to -1, -1
        for (int y = width; y >= -1; y--)
        {
            GameObject _wallTile = Instantiate(_test_wallObject, new Vector3(-1, y, 0), Quaternion.identity);
            _wallTile.transform.SetParent(_wallHolder);
        }
        // Fills from 0,0 to 20, 0
        for (int x = -1; x < height; x++)
        {
            GameObject _wallTile = Instantiate(_test_wallObject, new Vector3(x, -1, 0), Quaternion.identity);
            _wallTile.transform.SetParent(_wallHolder);
        }
        // Fills from 0,20 to 20,20
        for (int x = -1; x < height; x++)
        {
            GameObject _wallTile = Instantiate(_test_wallObject, new Vector3(x, height, 0), Quaternion.identity);
            _wallTile.transform.SetParent(_wallHolder);
        }


    }


}

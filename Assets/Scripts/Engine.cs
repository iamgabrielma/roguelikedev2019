using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using System.Diagnostics;
//using System;

public class Engine : MonoBehaviour
{
    //Monster monsterTest;
    //public enum CurrentState { player_turn, enemy_turn };
    /* DEBUG FUNCTIONS */
    //private bool _debug_instantiating_multiple_enemies = false;

    // Temporary screen_width & height
    public Transform _floorHolder;
    public Transform _wallHolder;
    //readonly int screen_width = 20;
    //readonly int screen_height = 20;
    // 30 rows allow each row to be 36 pixels high in 1080p, the most common monitor size.
    //readonly int mapX_width = 106; // eventually Moved to GameMap.cs
    //readonly int mapY_height = 30; // eventually  Moved to GameMap.cs

    public static GameObject __player;
    public static GameMap __gameMap;

    List<Entity> listOfEntities = new List<Entity>(); // To keep track of created Entities
    List<GameObject> listOfGameObjects = new List<GameObject>(); // To keep track of created GameObjects

    bool isPlayerPlaced;


    public static SchedulingSystem SchedulingSystem { get; private set; }

    public void Start()
    {

        isPlayerPlaced = false;
        // TODO: Add it from the GameMap class.
        _floorHolder = new GameObject("floorHolder").transform; // Will hold all our floor tiles, so the Inspector is not cluttered with GameObjects
        _wallHolder = new GameObject("wallHolder").transform; // Will hold all our wall tiles, so the Inspector is not cluttered with GameObjects

        // We instantiate our static schedule system:
        SchedulingSystem = new SchedulingSystem();
        Debug.Log("Schedulungsystem is created"); // this seems to be the first, so shouldn't be problems

        // Places player in a valid floorTile
        GameObject _test_player = Resources.Load<GameObject>("Prefabs/Player");
        int playerX = 106/2;
        int playerY = 30/2; // If we spawn the player in the center, waklkers always have opened the place up:

        Entity playerInstance = new Entity(playerX, playerY, "Player", _test_player, new Vector3(playerX, playerY, 0)); // Place player in a fixed place (temporary)
        //int _randomIndex = Random.Range(1, GridGenerator.listOfFloorTiles.Count);
        //Vector2 _randomVector = GridGenerator.listOfFloorTiles[_randomIndex];
        //Entity playerInstance = new Entity((int)_randomVector.x, (int)_randomVector.y, "Player", _test_player, new Vector3(_randomVector.x, _randomVector.y, 0));

        //SchedulingSystem.Add(playerInstance); // Now I can pass an Entity as a parameter because the Entity implements IScheduleable
        // Console message: Null added to the schedule. we'll see :D 
        //Debug.Log("player instance is added");
        __player = Instantiate(playerInstance.entityGameObject, playerInstance.entityLocation, Quaternion.identity);
        __player.name = __player.tag;
        listOfGameObjects.Add(__player);

        // TODO: Move this into a separate h_helper function
        //Stopwatch stopwatch = new Stopwatch();
        //stopwatch.Start();
        //FloorSetup(map_width, map_height);
        //stopwatch.Stop();
        //TimeSpan ts = stopwatch.Elapsed;
        //int _ms = ts.Milliseconds; // Calculated 14ms for a 20x20 via GameMap.cs . 14ms as well for 20x20 instantiating Prefabs. 165ms for 80x80




    }

    // WIP: Adding gamestate changes here for the moment.
    private void Update()
    {
        //if (__player != null && isPlayerPlaced == false && GridGenerator.__isMapReady == true)
        //{
        //    PlacePlayer();
        //}
        //else
        //{
        //    return;
        //}

        // If player turn, we can move and do actions
        // If enemy turn, we’re looping through each of the entities (excluding the player) and allowing them to take a turn. 
    }

    private void PlacePlayer() {
        // Moved the Player to be an instance of Entity.
        int _randomIndex = Random.Range(1, GridGenerator.listOfFloorTiles.Count);
        Vector2 _randomVector = GridGenerator.listOfFloorTiles[_randomIndex];
        __player.gameObject.transform.localPosition = new Vector3((int)_randomVector.x, (int)_randomVector.y, 0);
        isPlayerPlaced = true;

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

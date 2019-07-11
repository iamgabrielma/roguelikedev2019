using System.Collections;
using System.Collections.Generic;
using System.IO; // files + OS
using System.Runtime.Serialization.Formatters.Binary; // BinaryFormatter access
using UnityEngine;
using UnityEngine.UI;

//using System.Diagnostics;
//using System;

public class Engine : MonoBehaviour
{

    //public Text messageLogText;
    //Monster monsterTest;
    //public enum CurrentState { player_turn, enemy_turn };
    /* DEBUG FUNCTIONS */
    //private bool _debug_instantiating_multiple_enemies = false;

    // Temporary screen_width & height
    public Transform _floorHolder;
    public Transform _wallHolder;
    //public Transform _FOVCollisionHolder;
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

    // Message Logs:
    //public static MessageLog MessageLog { get; private set; }

    Entity _playerInstance;

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
        int playerY = 106/2; // If we spawn the player in the center, waklkers always have opened the place up:

        Entity playerInstance = new Entity(playerX, playerY, "Player", _test_player, new Vector3(playerX, playerY, 0)); // Place player in a fixed place (temporary)
        _playerInstance = playerInstance; // Assigned here to use later within Save()

        __player = Instantiate(playerInstance.entityGameObject, playerInstance.entityLocation, Quaternion.identity);
        __player.name = __player.tag;
        listOfGameObjects.Add(__player);

        //Hack -> Setting FOV holder this way because unity crashes if I try to add it as nested Object to the player. Doesn't seem to work either. Spawns the player in 0,0
        //_FOVCollisionHolder = new GameObject("_FOVCollisionHolder").transform;
        //_FOVCollisionHolder.transform.SetParent(__player.transform);
        //BoxCollider2D _fovbc2d = (BoxCollider2D)_FOVCollisionHolder.gameObject.AddComponent(typeof(BoxCollider2D));
        //_fovbc2d.tag = "FOVCollisionHolder";
        //_fovbc2d.size = new Vector2(10, 10);

        // TODO: Move this into a separate h_helper function
        //Stopwatch stopwatch = new Stopwatch();
        //stopwatch.Start();
        //FloorSetup(map_width, map_height);
        //stopwatch.Stop();
        //TimeSpan ts = stopwatch.Elapsed;
        //int _ms = ts.Milliseconds; // Calculated 14ms for a 20x20 via GameMap.cs . 14ms as well for 20x20 instantiating Prefabs. 165ms for 80x80

        // NullReferenceException: Object reference not set to an instance of an object... so we create an instance of _guiText...but not possible due to its protection level
        //MessageLog._guiText = MessageLog.OutputLogs();
        //Text _guiText_prefab = Resources.Load<Text>("Prefabs/_guiText");
        //Text tempTextBox = Instantiate(_guiText_prefab, new Vector3(0,0,0), Quaternion.identity) as Text;
        //tempTextBox.transform.SetParent();


        //MessageLog.OutputLogs();
        //messageLogText.text = "";
        //var _foo = MessageLog.OutputLogs();

        //var _foo = MessageLog;
        //foreach (var item in _foo.OutputLogs())
        //{
        //    messageLogText.text += _foo[item].ToString();
        //}
        //messageLogText.text = "hello";//MessageLog.OutputLogs().ToString();
        //messageLogText = GetComponent<Text>();






    }

    // WIP: Adding gamestate changes here for the moment.
    private void Update()
    {
        //if (true)
        //{
        //    MessageLog.OutputLogs();
        //}
        //messageLogText.text = "hello";
    }

    /* DEPRECATED */
    private void DEPRECATED_PlacePlayer() {
        // Moved the Player to be an instance of Entity.
        int _randomIndex = Random.Range(1, GridGenerator.listOfFloorTiles.Count);
        Vector2 _randomVector = GridGenerator.listOfFloorTiles[_randomIndex];
        __player.gameObject.transform.localPosition = new Vector3((int)_randomVector.x, (int)_randomVector.y, 0);
        isPlayerPlaced = true;

    }

    public void DEPRECATED_FloorSetup(int width, int height)
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

    public void DEPRECATED_WallsSetup(int width, int height) {

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


    public void SaveGame()
    {
        // Create a Save instance with all the data for the current session saved into it.
        Save save = CreateSaveGameObject();

        // It serializes the data (into bytes) and writes it in binary to disk, and closes the FileStream.
        // Loading works the other way around, the binary formatter reads from the binary source and converts it to Unity stuff
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
        bf.Serialize(file, save);
        file.Close();

        // (?) Reset the game so after saving everything in in default state? Why?

        Debug.Log("Game Saved");

    }

    // https://www.raywenderlich.com/418-how-to-save-and-load-a-game-in-unity
    private Save CreateSaveGameObject()
    {
        // Instance of the Save() class with all the Entity information 
        GameObject _currentPlayerReference = GameObject.Find("Player");
        GameObject[] _currentListOfEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        Save save = new Save(_currentPlayerReference, _currentListOfEnemies);

        return save;
    }

    public void LoadGame()
    {

        if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
        {
            Debug.Log("Game Loaded");

            // 1. Clear the game before loading the saving?

            // 2. again create a BinaryFormatter, only this time we are providing it with a stream of bytes to read instead of write.
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
            Save save = (Save)bf.Deserialize(file);
            file.Close();

            // 3. we have the save information, we still need to convert that into the game state.
            //_playerInstance.entityLocation = new Vector3(save.playerPosition[0], save.playerPosition[1], save.playerPosition[2]);
            //Debug.Log("Player position loaded at x:" + _playerInstance.entityLocation.x + " y:" + _playerInstance.entityLocation.y + " z:" + _playerInstance.entityLocation.z);

            GameObject _currentPlayerReference = GameObject.Find("Player");
            _currentPlayerReference.transform.localPosition = new Vector3(save.playerPosition[0], save.playerPosition[1], save.playerPosition[2]);

            GameObject[] _currentListOfEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            for (int i = 0; i < _currentListOfEnemies.Length; i++)
            {
                _currentListOfEnemies[i].transform.localPosition = new Vector3(save.enemyPositions[i,0], save.enemyPositions[i, 1], save.enemyPositions[i, 2]);
            }
            //Debug.Log("Player position loaded at x:" + _playerInstance.entityLocation.x + " y:" + _playerInstance.entityLocation.y + " z:" + _playerInstance.entityLocation.z);
            // 4. Update UI

            Debug.Log("Game Loaded");
        }
    }

}

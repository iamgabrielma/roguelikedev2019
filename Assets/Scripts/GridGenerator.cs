using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// new dependencies:
using UnityEngine.Tilemaps;
using UnityEditor;
// Diagnostics and debugging
//using System.Diagnostics; // enables System.Diagnostics.StopWatch
//using System; // enables System.TimeSPan
//using System; // Use of Next() in Random.Next() If we declare it, confuses Random with Unity.Random or System.Random

public class GridGenerator : MonoBehaviour
{
    // FOV variables
    public bool isFOVrecompute;
    private Transform playerReference; // I can calculate the FOV based on player position

    /* Debug checks for adding variety to the ProcGen map */
    //private bool _debug_procgen_tileDensity;
    private bool _debug_z_block_of_code = false;
    private bool _debug_show_level_no_FOV = false;

    // The tile map stores sprites in a layout marked by a Grid component. Ref: https://docs.unity3d.com/2017.2/Documentation/ScriptReference/Tilemaps.Tilemap.html
    public Tilemap floorMap; // TODO: change to Tilemap highlightMap = GetComponent<TileMap>();
    public Tilemap wallMap;
    public Tile floorTile; // The Tile class is a simple class that allows a sprite to be rendered on the Tilemap.. Ref: https://docs.unity3d.com/2017.2/Documentation/Manual/Tilemap-ScriptableTiles-TileBase.html
    public Tile wallTile;

    public int seed;
    public bool useRandomSeed;

    private int mapWidthX = 106;
    private int mapHeightY = 30;

    // ProcGen variables:
    enum gridSpaces { empty, floor, wall };
    enum walkerDirection{ up, down, left, right};
    class Walker
    {
        public Vector2 walkerPosition;
        public Vector2 walkerDirection;
    }
    private int maxNumberOfWalkers;
    List<Walker> listOfWalkers = new List<Walker>();

    List<Vector2> listOfFloorTiles = new List<Vector2>(); // Will be a list of positions, not of tiles.
    List<Vector2> listOfWallTiles = new List<Vector2>();


    //public TileBase tileBase; // Asigned to our floor tilebase in the editor.

    // Start is called before the first frame update
    void Start()
    {
        playerReference = GameObject.FindWithTag("Player").transform;
        //Debug.Log("playerref vector" + playerReference.localPosition.x.ToString()); // 0.5

        isFOVrecompute = InputHandler.isFOVrecompute; // We set this to the Static bool
        //Debug.Log("isFOVrecompute" + isFOVrecompute);
        /* WIP: SEEDS */
        //if (useRandomSeed)
        //{
        //    // TODO : Fix, possibly via hashcode, for now random int is ok
        //    seed = Random.Range(1,100);



        //}
        //Debug.Log("Using seed: " + seed);
        //Random.InitState(seed);
        //Random.State oldState = Random.state;
        //Debug.Log(oldState);

        // We assigned the child TileMap from the Grid object for highlightMap directly in the inspector

        /* __This works__ */
        // https://docs.unity3d.com/ScriptReference/Vector3Int.html
        //highlightMap.SetTile(new Vector3Int(0,0,0), tileBase);
        //Debug.Log (highlightMap.GetTile(Vector3Int.zero)); // null if there's no tile, TileBase.tileBase if there's tile.
        /* __end__This works__end__ */

        //Stopwatch stopwatch = new Stopwatch();
        //stopwatch.Start();
        //FloorSetup(106, 30);
        //WallsSetup(106, 30);
        //stopwatch.Stop();
        //TimeSpan ts = stopwatch.Elapsed;
        //int _ms = ts.Milliseconds; // Calculated 4ms vs 14ms for a 20x20 via Prefabs. 27ms in tilemaps vs 170 in prefabs for 80x80 map. Staying with TileMaps!
        // 16ms for generating both floors and walls on a 106x30 tileset


        ProcGenFloorSetup();

        // TODO: Fix and implement this debugger, for now full map can be seen on "Scene" mode.
        if (_debug_show_level_no_FOV)
        {
            foreach (var item in listOfFloorTiles)
            {
                wallMap.SetColor(new Vector3Int((int)item.x, (int)item.y, 0), Color.white);
                floorMap.SetColor(new Vector3Int((int)item.x, (int)item.y, 0), Color.white);
            }
        }

    }


    // Update is called once per frame
    void Update()
    {
        isFOVrecompute = InputHandler.isFOVrecompute; // We set this to the Static bool
        //Debug.Log("isFOVrecompute" + isFOVrecompute);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ClearReferences();

            if (useRandomSeed)
            {
                // TODO : Fix, possibly via hashcode, for now random int is ok
                seed = Random.Range(1, 100);
            }

            Random.InitState(seed);
            Debug.Log("Using seed: " + seed);


            ProcGenFloorSetup();
            for (int i = 0; i < 1000; i++) // 1000 because ¯\_(ツ)_/¯
            {
                WalkersPopulationControl(); // regenerates all tiles
            }
            ProcGenWallSetup(); // Once floors are generated, we can generate the walls
            GetMapDensityData(); // TODO: For now is just data, later I can change it from void to something else to perform more ProcGen
                                 //OnDrawGizmos();
                                 //listOfWalkers.Clear();

            // TODO: This needs to be a check that is coming from the player entity when moves.
            //isFOVrecompute = true;
        }

        // WIP FOV
        if (isFOVrecompute)
        {
            // TODO: Most likely I can take this outside of Update() for performance. 
            List<Vector2> listOfDiscovered_vectors = new List<Vector2>();
            List<Vector2> listOfClosestFOV_vectors = new List<Vector2>();
            //List<Vector2> _test_list_intersected_items = new List<Vector2>();

            //_test_list_intersected_items = listOfClosestFOV_vectors.FindAll(elem => listOfDiscovered_vectors.Contains(elem));


            // 2 FUNCTIONALITIES: 1) List of already discovered vectors 2) Current FOV

            // 1: TESTING WORKS! 
            //TileBase _testing_fov_recop =  floorMap.GetTile(new Vector3Int(0, 2, 0));
            //wallMap.SetTileFlags(new Vector3Int(0, 2, 0), TileFlags.None); // by default color is blocked  so we use tileflags.none ? To change this go to your tile asset under your tile pallet, look at the inspector, right click it and look at the debug inspector, and change the flag from 'color locked' to 'none'.
            //wallMap.SetColor(new Vector3Int(0, 2, 0), Color.green);

            // 1.1 Locate the NEW player vector: TODO: Change the +5/-5 to a visibility variable . TODO: Fix that there's the same visibility all around, not 5 in one direction and 4 in the other because counts the playerPos as one tile.
            // TODO: Most likely I can take this outside of Update() for performance. 
            int _playerPosX = (int)playerReference.localPosition.x;
            int _playerPosY = (int)playerReference.localPosition.y;
            int _tileDifferenceX = (int)playerReference.localPosition.x + 5;
            int _tileDifferenceY = (int)playerReference.localPosition.y + 5;

            int _tileOffsetFromPlayerX = (int)playerReference.localPosition.x -5;
            int _tileOffsetFromPlayerY = (int)playerReference.localPosition.y - 5;


            /* Logic for discovered terrain */
            // 1.3 Set a radius around the player. 
 

            for (int x = _tileOffsetFromPlayerX; x < _tileDifferenceX; x++)
            {
                for (int y = _tileOffsetFromPlayerY; y < _tileDifferenceY; y++)
                {
                    listOfDiscovered_vectors.Add(new Vector2(x, y));
                    listOfClosestFOV_vectors.Add(new Vector2(x, y)); // Will be used to recalculate the color of the nearest tiles.

                }
            }
            //Debug.Log("listOfDiscovered_vectors" + listOfDiscovered_vectors.Count.ToString()); //100 vectors
            //Debug.Log("listOfClosestFOV_vectors" + listOfClosestFOV_vectors.Count.ToString()); //100 vectors
            // This list will contain all discovered vectors around the player
            foreach (var item in listOfDiscovered_vectors)
            {
                wallMap.SetColor(new Vector3Int((int)item.x, (int)item.y, 0), Color.grey);
                floorMap.SetColor(new Vector3Int((int)item.x, (int)item.y, 0), Color.grey);

            }
            // This list can only contain new vectors each time is run. AFTER these have been painted to grey. List must be empty on new FOV recomputes.
            // PROBLEM: Doesn't work because both 
            //foreach (var item in listOfClosestFOV_vectors)
            //{
            //    wallMap.SetColor(new Vector3Int((int)item.x, (int)item.y, 0), Color.blue);
            //    floorMap.SetColor(new Vector3Int((int)item.x, (int)item.y, 0), Color.blue);
            //}
            //foreach (var item in _test_list_intersected_items)
            //{
            //        wallMap.SetColor(new Vector3Int((int)item.x, (int)item.y, 0), Color.blue);
            //        floorMap.SetColor(new Vector3Int((int)item.x, (int)item.y, 0), Color.blue);
            //}
            //_test_list_intersected_items.Clear();
            //Debug.Log("_test_list_intersected_items" + _test_list_intersected_items.Count.ToString()); // 0 vectors

            // 2: Now that we have a list of discovered vectors, I need to validate again which ones are within FOV, for the actual player view.
            isFOVrecompute = false;



        }

        if (_debug_z_block_of_code && Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("########### --- DEBUG --- ###########");
        }
    }

    void ClearReferences() {

        floorMap.ClearAllTiles(); // clears all floor tiles
        listOfFloorTiles.Clear(); // clear this count list as well
        listOfWallTiles.Clear(); // clear this count list as well
        listOfWalkers.Clear(); // clears all walkers for the new iteration this may need to go at the end of the script, or will clear them before generates the level.

    }

    // Procedural generation version of FloorSetup() . Will be using the Drunken Walker algorithm
    public void ProcGenFloorSetup()
    {

        // WIP: _temporary_ initial (0,0) and final walkers (max,max) to play with connecting corrindors
        //Walker _temp_initialWalker = new Walker();
        //_temp_initialWalker.walkerPosition = new Vector2(0,0);
        //listOfWalkers.Add(_temp_initialWalker);
        //Walker _temp_finalWalker = new Walker();
        //_temp_finalWalker.walkerPosition = new Vector2(mapWidthX, mapHeightY);
        //listOfWalkers.Add(_temp_finalWalker);


        // TODO: Note that maxNumberOfWalkers is actually maxNumberOf_INITIAL_Walkers , as new ones are born in PopulationControl function.
        maxNumberOfWalkers = 10;
        // 0: Iterations
        // Done -> 1: Create walkers:
        for (int i = 0; i < maxNumberOfWalkers; i++)
        {
            Walker _walker = new Walker(); // No vector here assumes that all of them are created at (0,0)
            // Problem: If walkers are created at (0,0), they easily grow the map in X and Y negative. Will use the center of the map as a starting location instead.
            _walker.walkerPosition = new Vector2((int)mapWidthX/2, (int)mapHeightY/2);
            listOfWalkers.Add(_walker);
        }

        // Done -> 2: WalkersPopulationControl() -> Walkers gonna walk
        // TODO: 3: WalkersPopulationControl() -> Chance of dying . Initial tests look pretty bad, re-check in the future.
        // Done -> 4: WalkersPopulationControl() -> Chance of spawning a new one
        // TODO: 5: Set limit where the level can grow. Clamp walker values to the map limits

    }

    public void ProcGenWallSetup() {

        // There's no need for all of this if we already have a list of tile locations listOfFloorTiles
        // Update: There's actually a need: https://forum.unity.com/threads/tilemap-tile-positions-assistance.485867/#post-3165299
        int width = 106;
        int height = 30;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // If the tile is not null, create a floor.
                if(floorMap.GetTile(new Vector3Int(x,y,0)) != null) {

                    //Debug.Log("x: " + x + "| y: "+ y); // We get the coordinates from looping through the game, not through the tilemap itself.
                    Vector2 _floorTileLocation = new Vector2(x,y);
                    listOfFloorTiles.Add(_floorTileLocation);

                }
                else
                {
                    // Else: 0.1 Approach to wall filling
                    Vector2 _wallTileLocation = new Vector2(x, y);
                    wallMap.SetTile(new Vector3Int((int)_wallTileLocation.x, (int)_wallTileLocation.y, 0), wallTile);
                    listOfWallTiles.Add(_wallTileLocation);
                    // TODO: 1) Clear tiles when reseting the map.
                    // TODO: Set map widths and height correctly, seems to be 3 different references at the moment. Walkers will go X-- and Y-- from (0,0) but the map doesn't, that's why I see this error.
                }
            }
        }

        PlaceEntities();

        //Debug.Log("Number of Tiles:" + listOfFloorTiles.Count.ToString());

        //foreach (var eachFloorTile in listOfFloorTiles)
        //{
        //    // TODO: Check the tiles around, if empty => set wall. Problem: TileBase does not have a "position" : https://forum.unity.com/threads/tilemap-tile-positions-assistance.485867/#post-3165299
        //    // iterate through all the coordinates on the grid beginning from the tilemap.origin up to tilemap.origin + tilemap.size. At each coordinate you can retrieve the TileBase that's associated with it, and do whatever you need to do.
        //    // So YES, it is necessary to go through the whole map again, not only the list of tiles.

        //}


    }

    void WalkersPopulationControl() {

        foreach (var _walker in listOfWalkers)
        {
            //Random.InitState(seed);
            /* This initially did not work because https://stackoverflow.com/questions/5650705/in-c-why-cant-i-modify-the-member-of-a-value-type-instance-in-a-foreach-loop
             * Because foreach uses an enumerator, and enumerators can't change the underlying collection, but can, however, change any objects referenced by an object 
             * in the collection. This is where Value and Reference-type semantics come into play.
             Solution: Change the current struct to a class? Now it works, but may have other problems in the future.
             */
            /* Works */
            //_walker.walkerPosition = new Vector2(0, _walker.walkerPosition.y + RandomDirection());
            //floorMap.SetTile(new Vector3Int(0, (int)_walker.walkerPosition.y, 0), floorTile);

            /* New solution: */
            _walker.walkerPosition = RandomDirection(_walker.walkerPosition);
            floorMap.SetTile(new Vector3Int((int)_walker.walkerPosition.x, (int)_walker.walkerPosition.y, 0), floorTile);

            // Walker population control
            int _rand = Random.Range(0, 100);
            if (_rand <= 50) // 50% chance of new walker being born
            {
                //Debug.Log("A new walker is created!");
                Walker _newWalker = new Walker();
                _newWalker.walkerPosition = _walker.walkerPosition;
                listOfWalkers.Add(_newWalker); // Problem this will not execute here because we're still within the loop, and cannot be modified by adding a new item while is running. That's why we add break to get out of the loop.
                break;
            }
            //else if (_rand > 95) // 10% Chance of walker dying
            //{
            //    listOfWalkers.Remove(_walker);
            //    break;
            //}

            //Debug.Log("pos: " + _walker.walkerPosition.ToString() + " dir: " + _walker.walkerDirection.ToString());
            //Debug.Log("number of walkers: "+ listOfWalkers.Count.ToString());
        }
    }

    Vector2 RandomDirection(Vector2 currentWalkerPosition) {

        // 1: Selects an initial direction:
        //Random.InitState(seed);
        float _rand = (int)Random.Range(0, 3.99f); // 0, 1, 2, 3
        switch (_rand)
        {
            case 0: //up x=0 y++
                currentWalkerPosition.y += 1.0f;
                break;
            case 1: //down x=0 y--
                currentWalkerPosition.y -= 1.0f;
                break;
            case 2: //left x-- y=0
                currentWalkerPosition.x -= 1.0f;
                break;
            case 3://right x++ y=0
                currentWalkerPosition.x += 1.0f;
                break;
            default:
                break;

        }

        // TODO 2:If this initial direction is outside of boundaries, roll again:
        /* Boundaries:
        BottomLeft: 0,0
        BottomRight: mapWidthX, 0
        TopLeft: 0, mapHeightY
        TopRight: mapWidthX, mapHeightY
        */
        if (currentWalkerPosition.x < 0) // Checks X-- and turns the walker around
        {
            currentWalkerPosition.x = currentWalkerPosition.x * (-1);

        } else if (currentWalkerPosition.y < 0) // Checks Y-- and turns the walker around
        {
            currentWalkerPosition.y = currentWalkerPosition.y * (-1);

        } else if (currentWalkerPosition.x > mapWidthX) // Checks if x wants to go outside boundaries
        {
            //currentWalkerPosition.x = currentWalkerPosition.x * (-1); // Error: Will put us in Y-- , which we don't want.
            //currentWalkerPosition.x -= 1.0f; // Approach 1: Go back
            //currentWalkerPosition.x = mapWidthX; // Approach 2: Stop
            // This would fill up the (0,0) corner
            currentWalkerPosition.x = 0; //Approach 3: Teleport to a corner, but map feels a blob as is filling up only in the middle , so we teleport the walker entirely, not only one axis.
            currentWalkerPosition.y = 0;

        }
        else if (currentWalkerPosition.y > mapHeightY) // Checks if y wants to go outside boundaries
        {
            //currentWalkerPosition.y = currentWalkerPosition.y * (-1); // Error: Will put us in Y-- , which we don't want.
            //currentWalkerPosition.y -= 1.0f; // Go back
            // This would fill up the (max,max) corner , see approach 3 above
            currentWalkerPosition.x = mapWidthX;
            currentWalkerPosition.y = mapHeightY;
        }

        return new Vector2(currentWalkerPosition.x, currentWalkerPosition.y);

    }


    private void GetMapDensityData() {

        //float _mapDensity = (listOfWallTiles.Count / listOfFloorTiles.Count) * 100;
        float _mapDensity = (float)listOfFloorTiles.Count / (mapWidthX * mapHeightY) * 100;
        //Debug.Log(listOfFloorTiles.Count);
        //Debug.Log(mapWidthX);
        //Debug.Log(mapHeightY);
        Debug.Log("Map Density " + _mapDensity + "% | Floor: " + listOfFloorTiles.Count.ToString() + " | Walls: " + listOfWallTiles.Count.ToString());
    }

    private void OnDrawGizmos()
    {

        Gizmos.color = (new Vector2(0,0) !=null)?Color.green:Color.white;
        foreach (var item in listOfFloorTiles)
        {
            Gizmos.DrawCube(new Vector2(item.x + 0.5f, item.y + 0.5f), Vector3.one);
        }
        //Gizmos.DrawCube(new Vector2(0+0.5f, 0+0.5f),Vector3.one);
    }

    private void PlaceEntities()
    {
        // Random number of enemies:
        int _numberOfEnemyEntities = Random.Range(1,10);
        //Debug.Log("Monsters: " + _numberOfEnemyEntities);

        // Reference to which Prefab to use:
        GameObject _test_npc = Resources.Load<GameObject>("Prefabs/Enemy");

        // For each entity to be created, find a suitable spawning place and Instantiate an enemy
        for (int i = 0; i < _numberOfEnemyEntities; i++)
        {
            Debug.Log("listOfFloorTiles count: " + listOfFloorTiles.Count.ToString());
            int _randomIndex = Random.Range(1, listOfFloorTiles.Count);
            Vector2 _randomVector = listOfFloorTiles[_randomIndex];

            Entity npcInstance = new Entity((int)_randomVector.x, (int)_randomVector.y, "Enemy", _test_npc, new Vector3(_randomVector.x, _randomVector.y, 0));
            Instantiate(npcInstance.entityGameObject, npcInstance.entityLocation, Quaternion.identity);
            // list[index]
            //Debug.Log("Available vector: " + _randomVector.ToString());
        }
        //listOfFloorTiles
        //Entity npcInstance = new Entity(1, 1, "Enemy", _test_npc, new Vector3(1,1,0));

        // Track entities
        //List<Entity> listOfEntities = new List<Entity>();
    }
}

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
    public static bool __isMapReady; // Once this is ready , I should move the map into int[,] map
    public static bool __generateNextFloor;
    int[,] map; // Creating this for the flood generation. We'll have to store our map here with 0's and 1's

    // FOV variables
    public bool isFOVrecompute;
    private Transform playerReference; // I can calculate the FOV based on player position
    public int playerVisibilityRadius;

    /* Debug checks for adding variety to the ProcGen map */
    //private bool _debug_procgen_tileDensity;
    private bool _debug_z_block_of_code = false;
    //private bool _debug_show_level_no_FOV = false;
    private bool _debug_redo_map = false;

    // The tile map stores sprites in a layout marked by a Grid component. Ref: https://docs.unity3d.com/2017.2/Documentation/ScriptReference/Tilemaps.Tilemap.html
    public Tilemap floorMap; // TODO: change to Tilemap highlightMap = GetComponent<TileMap>();
    public Tilemap wallMap;
    public Tile floorTile; // The Tile class is a simple class that allows a sprite to be rendered on the Tilemap.. Ref: https://docs.unity3d.com/2017.2/Documentation/Manual/Tilemap-ScriptableTiles-TileBase.html
    public Tile wallTile;

    public Tilemap _testing_map;
    public Tilemap _testing_closestFovMap;

    public int seed;
    public bool useRandomSeed;
    private int currentFloor;

    private int mapWidthX = 106;
    private int mapHeightY = 106;

    // ProcGen variables:
    enum gridSpaces { empty, floor, wall };
    enum walkerDirection{ up, down, left, right};

    class Walker
    {
        public Vector2 walkerPosition;
        public Vector2 walkerDirection;
    }

    class FloodRegion
    {
        public List<Vector2> floodRegion;
        public int group;
    }

    private int initialNumberOfWalkers;
    private int initialNumberOfWalkeriterations;

    List<Walker> listOfWalkers = new List<Walker>();
    List<Walker> listOfNonRandomWalkers = new List<Walker>();

    public static List<Vector2> listOfFloorTiles = new List<Vector2>(); // Will be a list of positions, not of tiles.
    List<Vector2> listOfWallTiles = new List<Vector2>();

    //List<Vector2> listOfRegion1FloorTiles = new List<Vector2>(); // Region 1 massive area
    List<Vector2> listOfRegion2FloorTiles = new List<Vector2>(); // Region 2, the rest

    List<Vector2> borders = new List<Vector2>();

    public List<Entity> listOfEnemyEntities = new List<Entity>();
    public List<Entity> listOfItems = new List<Entity>();


    //public TileBase tileBase; // Asigned to our floor tilebase in the editor.

    // Start is called before the first frame update
    void Start()
    {
        currentFloor = 1;
        __isMapReady = false;
        __generateNextFloor = false;
        playerReference = GameObject.FindWithTag("Player").transform;
        playerVisibilityRadius = 4;
        isFOVrecompute = InputHandler.isFOVrecompute; // We set this to the Static bool

        // Check if we're using a seed for the map
        if (useRandomSeed)
        {
            seed = Random.Range(1, 100); // TODO : Fix, possibly via hashcode, for now random int is ok
        }
        Random.InitState(seed);
        Debug.Log("Generating Map: Using seed " + seed);

        // Floor setup
        ProcGenFloorWalkersSetup();
        // WIP: Testing new Walkers: TODO: Fix.
        TEST_ProcGenNewWalkers();
        // Walls setup
        ProcGenWallSetup();
        // Close gaps in the map: Finds floorTiles in the borders and changes them for wallTiles
        ProcGenWallFixtures(mapWidthX, mapHeightY);
        // Get floor borders graphically
        //GetProcGenMapBorders();
        // 
        //ProcGenAdditionalWalkers();
        //ProcGenWallFixtures(mapWidthX, mapHeightY);


        PlaceEntities();
        PlaceItems();
        PlaceExit();
        __isMapReady = true; // Use this for enemyAI and placing items, Once this is ready , I should move the map into int[,] map . Also as well AFTER entities have been placed.
        //ProcessMap();
        GetMapData();
        // Testing filling map generation for more density - SUCCESS
        //TEST_ProcGenNewWalkers();

        // Testing region filling

        //GetRegionTiles(mapWidthX, mapHeightY);
        //List<Coord> _test = new List<Coord>();
        //_test = GetRegionTiles(mapWidthX, mapHeightY);
        //Debug.Log("test is:" + _test.Count.ToString());

        //GetNeighbors();



        // Lague application:
        //SetIntegerMap();
        //GetRegionTiles(1, 1);//4 tiles
        //GetRegionTiles(2, 2);//8? tiles
        //GetRegionTiles(mapWidthX-1, mapHeightY-1); // Finally seems to work, next step list of lists.
        //List<Coord> _test = new List<Coord>();
        //_test = GetRegionTiles(mapWidthX, mapHeightY);
        //Debug.Log("test is:" + _test.Count.ToString());


        // Testing - SUCESS
        //GetBorderVolumes();
        // Testing... WIP
        //GetRegions();
        // Testing ... Floodtilemaps
        //Tilemap _tilemapregion1 = FloodRegions(floorMap);
        //Tilemap _tilemapregion2 = FloodRegions(floorMap);
        //Debug.Log("tilemap region 2 size:" + _tilemapregion2.size.ToString()); // 107,31,1 ¯\_(ツ)_/¯
        //Debug.Log("tilemap region 1 magnitude:" + _tilemapregion1.size.magnitude.ToString());
        //Debug.Log("tilemap region 2 size:" + _tilemapregion2.size.x.ToString());
        //Debug.Log("tilemap region 2 size:" + _tilemapregion2.AddTileFlags);
        //ProcGenBorderFixtures();



        //GetRegions(0); // 0 -> floors, -> walls
        // More WIP
        //FloodingTilesWIP();
        //foreach (var item in listOfFloorTiles)
        //{
        //    _tilemapregion2.SetTileFlags(new Vector3Int((int)item.x, (int)item.y, 0), TileFlags.None);
        //}


        // Place Entities

        // Helper function to get level information



        //GetRegionTiles(1,1);


    }

    public int CurrentFloor { get { return currentFloor; } }
    
    void Update()
    {
        if (__generateNextFloor)
        {
            NextFloor();
        }

        isFOVrecompute = InputHandler.isFOVrecompute; // We set this to the Static bool
        //Debug.Log("isFOVrecompute" + isFOVrecompute);

        // If _debug_redo_map = True, we can create a new map . TODO: This is repeated code except ClearReferences() , good chance to put all in the same method
        if ( _debug_redo_map && Input.GetKeyDown(KeyCode.Space))
        {
            ClearReferences();

            if (useRandomSeed)
            {
                seed = Random.Range(1, 100);
            }

            Random.InitState(seed);
            Debug.Log("Generating Map: Using seed " + seed);

            // Floor setup
            ProcGenFloorWalkersSetup();
            // Walls setup
            ProcGenWallSetup();
            // Close gaps in the map
            ProcGenWallFixtures(mapWidthX, mapHeightY);
            // Place Entities
            PlaceEntities();
            // Helper function to get level information
            GetMapData();
        }

        // WIP FOV
        if (isFOVrecompute)
        {
            // Calculates Enemy FOV
            CalculateEntityClosestFOV(listOfEnemyEntities);

            // TODO: Most likely I can take this outside of Update() for performance. 
            // TODO: I think I don't need these and can be deleted. Review.
            List<Vector2> listOfDiscovered_vectors = new List<Vector2>();
            List<Vector2> listOfClosestFOV_vectors = new List<Vector2>();


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

            // This list will contain all discovered vectors around the player
            foreach (var item in listOfDiscovered_vectors)
            {
                wallMap.SetColor(new Vector3Int((int)item.x, (int)item.y, 0), Color.grey);
                floorMap.SetColor(new Vector3Int((int)item.x, (int)item.y, 0), Color.grey);

            }

            // Calculates Player FOV
            CalculatePlayerClosestFOV();



            // This sets to null these tiles outside the player area, so are repainted to grey once the player moves to the next place.
            // TODO: Implement into CalculatePlayerClosestFOV() and use visibility variables instead of harcoded values.
            for (int i = 0; i < 2; i++)
            {

                for (int a = 0; a < 6; a++) // 6 because must be double than -2 -1 and 2+1
                {
                    _testing_closestFovMap.SetTile(new Vector3Int(_playerPosX - 2 - 1 + a, _playerPosY + 2 + 1, 0), null);
                }
                // bottom
                for (int a = 0; a < 6; a++) // 6 because must be double than -2 -1 and 2+1
                {
                    _testing_closestFovMap.SetTile(new Vector3Int(_playerPosX - 2 - 1 + a, _playerPosY - 2 - 1, 0), null);
                }
                // left
                for (int a = 0; a < 6; a++) // 6 because must be double than -2 -1 and 2+1
                {
                    _testing_closestFovMap.SetTile(new Vector3Int(_playerPosX - 2 - 1, _playerPosY - 2 - 1 + a, 0), null);
                }
                // right
                for (int a = 0; a < 6; a++) // 6 because must be double than -2 -1 and 2+1
                {
                    _testing_closestFovMap.SetTile(new Vector3Int(_playerPosX + 2 + 1, _playerPosY - 2 - 1 + a, 0), null);
                }

            }


            isFOVrecompute = false;


        }

        if (_debug_z_block_of_code && Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("########### --- DEBUG --- ###########");
        }
    }

    void ClearReferences() {

        // Map:
        floorMap.ClearAllTiles(); // clears all floor tiles
        listOfFloorTiles.Clear(); // clear this count list as well
        listOfWallTiles.Clear(); // clear this count list as well
        listOfWalkers.Clear(); // clears all walkers for the new iteration this may need to go at the end of the script, or will clear them before generates the level.

        // Enemies and Items
        /* TIL: 
        Attempting to destroy an asset loaded through Resource.Load is like attempting to delete an asset (be it a prefab or a texture or what have you) from the Project view. 
        It will delete that asset permanently if it were allowed
        */
        GameObject[] _temp_listOfCurrentEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] _temp_listOfCurrentItems = GameObject.FindGameObjectsWithTag("Item");
        foreach (var item in _temp_listOfCurrentEnemies)
        {
            Destroy(item);
        }
        foreach (var item in _temp_listOfCurrentItems)
        {
            Destroy(item);
        }
        listOfEnemyEntities.Clear();
        listOfItems.Clear();

    }

    /* Sets up the Walkers that will construct the floor grid later on */
    public void ProcGenFloorWalkersSetup()
    {

        initialNumberOfWalkers = 10; // How many walkers we'll initially create
        initialNumberOfWalkeriterations = 1000;

        // Create walkers
        for (int i = 0; i < initialNumberOfWalkers; i++)
        {
            Walker _walker = new Walker(); // No vector here assumes that all of them are created at (0,0)
            _walker.walkerPosition = new Vector2((int)mapWidthX/2, (int)mapHeightY/2); // Spawn them at the center of the map instead
            listOfWalkers.Add(_walker);
        }

        // Runs WalkerBehaviour() a fixed number of times. Each time this functions run, each walker of the list moves, replicates, or die, building a path of floors in each movement
        for (int i = 0; i < initialNumberOfWalkeriterations; i++) 
        {
            WalkerBehaviour(listOfWalkers);
        }

    }

    public void TEST_ProcGenNewWalkers() {


        for (int i = 0; i < 50; i++) // 50 because why not.
        {
            float _density = GetMapData();

            if (_density < 0.3f)
            {
                Walker _nonRandomWalker = new Walker();

                int _rand = Random.Range(0, listOfWallTiles.Count); // TODO: Change this to borders
                //Debug.Log(_rand);
                Vector2 _randpos = listOfWallTiles[_rand]; // Index out of range, wut. ISSUE: At this point we still have no walls, so index will be zero.
                                                           //Debug.Log(listOfWallTiles[_rand].ToString());
                _nonRandomWalker.walkerPosition = new Vector2(_randpos.x, _randpos.y);
                //Debug.Log("Created new walker at: " + _nonRandomWalker.walkerPosition.ToString());

                // Fix lists: 
                listOfWallTiles.Remove(_nonRandomWalker.walkerPosition);//removes this vector from the wall list, as is a floor now
                listOfFloorTiles.Add(_nonRandomWalker.walkerPosition);//adds a new floor tile
                listOfNonRandomWalkers.Add(_nonRandomWalker);//adds nonrandomwalker

                TEST_Moving_non_random_walkers();

            }
            else
            {
                Debug.Log("No walkers to add, density: " + GetMapData().ToString());
                break; // get out the loop
            }
        }

    }

    public void TEST_Moving_non_random_walkers() {

        foreach (var _nonRandomWalker in listOfNonRandomWalkers)
        {
        
            _nonRandomWalker.walkerPosition = RandomDirection(_nonRandomWalker.walkerPosition);
            floorMap.SetTile(new Vector3Int((int)_nonRandomWalker.walkerPosition.x, (int)_nonRandomWalker.walkerPosition.y, 0), floorTile); // set new floor
            wallMap.SetTile(new Vector3Int((int)_nonRandomWalker.walkerPosition.x, (int)_nonRandomWalker.walkerPosition.y, 0), null); // unset old wall
            floorMap.SetColor(new Vector3Int((int)_nonRandomWalker.walkerPosition.x, (int)_nonRandomWalker.walkerPosition.y, 0), Color.green); // testing visibility

            // Fixing lists:
            Vector2 _floorTileLocation = new Vector2((int)_nonRandomWalker.walkerPosition.x, (int)_nonRandomWalker.walkerPosition.y);
            listOfFloorTiles.Add(_floorTileLocation); // Add to floor list
            listOfWallTiles.Remove(_floorTileLocation); // remove from wall list

        }

    }

    public void ProcGenWallSetup() {
    
        int width = mapWidthX;
        int height = mapHeightY;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // refactores
                if (floorMap.GetTile(new Vector3Int(x, y, 0)) == null)
                {
                        Vector2 _wallTileLocation = new Vector2(x, y);
                        wallMap.SetTile(new Vector3Int((int)_wallTileLocation.x, (int)_wallTileLocation.y, 0), wallTile);
                        listOfWallTiles.Add(_wallTileLocation);
                }
                // If the tile is not null, create a floor.
                //if(floorMap.GetTile(new Vector3Int(x,y,0)) != null) {

                //    //Vector2 _floorTileLocation = new Vector2(x,y); // TODO : ? Why we're setting this again? I've removed it and the map looks the same.. maybe was coming from some previous iteration
                //    //listOfFloorTiles.Add(_floorTileLocation);
                //    //continue;

                //}
                //else
                //{
                //    // If tile is null, create a wall
                //    Vector2 _wallTileLocation = new Vector2(x, y);
                //    wallMap.SetTile(new Vector3Int((int)_wallTileLocation.x, (int)_wallTileLocation.y, 0), wallTile);
                //    listOfWallTiles.Add(_wallTileLocation);
                //    // TODO: 1) Clear tiles when reseting the map.
                //    // TODO: Set map widths and height correctly, seems to be 3 different references at the moment. Walkers will go X-- and Y-- from (0,0) but the map doesn't, that's why I see this error.
                //}
            }
        }
    }
    // TODO: Bug Fix: Tiles added via this process don't go through FOV and are always visible. 
    void ProcGenWallFixtures(int width, int height)
    {
        // TODO: Seems that this may be the cause why shorter FOV was not working. If I generate a new tile in Floor or Walls, it inherits the grey color and is not blue. I need to use a new tilemap for setting a new color because is set by TILEMAP, not by TILE!!!

        // Fills with walls the bottom and ceiling map openings from the walkers
        for (int i = 0; i < width; i++)
        {
            if (floorMap.GetTile(new Vector3Int(i, 0, 0)) != null)
            {
                floorMap.SetTile(new Vector3Int(i, 0, 0), null); // Clear potential previous floors
                wallMap.SetTile(new Vector3Int(i, 0, 0), wallTile);
                wallMap.SetColor(new Vector3Int(i, 0, 0), Color.black);


                // Add to our list of walls:
                Vector2 _wallTileLocation = new Vector2(i, 0);
                listOfWallTiles.Add(_wallTileLocation); // + list
                listOfFloorTiles.Remove(_wallTileLocation); // - list
            }
            if (floorMap.GetTile(new Vector3Int(i, height-1, 0)) != null)
            {
                floorMap.SetTile(new Vector3Int(i, height - 1, 0), null); // Clear potential previous floors
                wallMap.SetTile(new Vector3Int(i, height-1, 0), wallTile);
                wallMap.SetColor(new Vector3Int(i, height-1, 0), Color.black);

                // Add to our list of walls:
                Vector2 _wallTileLocation = new Vector2(i, height-1);
                listOfWallTiles.Add(_wallTileLocation);// + list
                listOfFloorTiles.Remove(_wallTileLocation); // - list
            }
        }
        // Fills with walls the right and left map openings from the walkers
        for (int i = 0; i < height; i++)
        {
            if (floorMap.GetTile(new Vector3Int(0, i, 0)) != null)
            {
                wallMap.SetTile(new Vector3Int(0, i, 0), wallTile);
                wallMap.SetColor(new Vector3Int(0, i, 0), Color.black);
                // Add to our list of walls:
                Vector2 _wallTileLocation = new Vector2(0, i);


                floorMap.SetTile(new Vector3Int(0, i, 0), null); // Clear potential previous floors
                listOfWallTiles.Add(_wallTileLocation);
                listOfFloorTiles.Remove(_wallTileLocation); // Remove them for the list as well so we can use flood algo
            }
            if (floorMap.GetTile(new Vector3Int(i, height - 1, 0)) != null)
            {
                wallMap.SetTile(new Vector3Int(height, i, 0), wallTile);
                wallMap.SetColor(new Vector3Int(height, i, 0), Color.black);
                // Add to our list of walls:
                Vector2 _wallTileLocation = new Vector2(height, i);

                floorMap.SetTile(new Vector3Int(height, i, 0), null); // Clear potential previous floors
                listOfWallTiles.Add(_wallTileLocation);
                listOfFloorTiles.Remove(_wallTileLocation); // Remove them for the list as well so we can use flood algo
            }
        }
    }

    // Each time this functions run, each walker of the list moves, replicates, or die, building a path of floors in each movement
    void WalkerBehaviour(List<Walker> _inputListOfWalkers) {


        // TODO: 3: WalkersPopulationControl() -> Chance of dying . Initial tests look pretty bad, re-check in the future.
        // TODO: 5: Set limit where the level can grow. Clamp walker values to the map limits

        foreach (var _walker in _inputListOfWalkers)
        {
            /* Comment:
             * This initially did not work because https://stackoverflow.com/questions/5650705/in-c-why-cant-i-modify-the-member-of-a-value-type-instance-in-a-foreach-loop
             * Because foreach uses an enumerator, and enumerators can't change the underlying collection, but can, however, change any objects referenced by an object 
             * in the collection. This is where Value and Reference-type semantics come into play.
             Solution: Change the current struct to a class? Now it works, but may have other problems in the future.
             */

            _walker.walkerPosition = RandomDirection(_walker.walkerPosition); // Walker new position will be a new position in a random direction RandomDirection()
            wallMap.SetTile(new Vector3Int((int)_walker.walkerPosition.x, (int)_walker.walkerPosition.y, 0), null); // This part is done as once we run the 2nd or more iterations of this functions, there may be walls in place so even if we generate the floors in its tilemap we won't see them.
            floorMap.SetTile(new Vector3Int((int)_walker.walkerPosition.x, (int)_walker.walkerPosition.y, 0), floorTile);
            listOfFloorTiles.Add(_walker.walkerPosition);

            // Walker population control

            int _rand = Random.Range(0, 100);
            if (_rand <= 30) // 30% chance of new walker being born
            {
                //Debug.Log("A new walker is created!");
                Walker _newWalker = new Walker();
                _newWalker.walkerPosition = _walker.walkerPosition; // new walker position is the same as the old walker position
                listOfFloorTiles.Add(_walker.walkerPosition);
                _inputListOfWalkers.Add(_newWalker); // Problem this will not execute here because we're still within the loop, and the LIST listOfWalkers cannot be modified by adding a new item while is running. That's why we add break to get out of the loop.
                break;
            }
        }
    }

    Vector2 RandomDirection(Vector2 currentWalkerPosition) {

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

        } 

        return new Vector2(currentWalkerPosition.x, currentWalkerPosition.y);

    }


    private float GetMapData() {

        //float _mapDensity = (listOfWallTiles.Count / listOfFloorTiles.Count) * 100;
        float _mapDensity = (float)listOfFloorTiles.Count / (mapWidthX * mapHeightY);
        // TODO this is badly counted because walkers add this multiple times for the same coordinate

        //float _region1MapDensity = (float)listOfRegion1FloorTiles.Count / (listOfFloorTiles.Count);
        //float _region2MapDensity = (float)listOfRegion2FloorTiles.Count / (listOfFloorTiles.Count); // Right now 0%.
        //Debug.Log(listOfFloorTiles.Count);
        //Debug.Log(mapWidthX);
        //Debug.Log(mapHeightY);
        //Debug.Log("Map Density " + (_mapDensity * 100) + "% | Floor: " + listOfFloorTiles.Count.ToString() + " | Walls: " + listOfWallTiles.Count.ToString());
        //Debug.Log("Region 1 density: " + (_region1MapDensity * 100 ) + "%");
        //Debug.Log("Region 2 density: " + (_region2MapDensity * 100) + "%");

        return _mapDensity;
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

    // TODO: Most likely we can place this in a different place, for example on GameStateManager to generate enemies as new events or states happen.
    private void PlaceEntities()
    {
        int _numberOfEnemyEntities = 10; // TODO: Move this somewhere differently, Game Manager?

        GameObject enemyObjectPrefab = Resources.Load("Prefabs/Enemy") as GameObject;
         
        // For each entity to be created, find a suitable spawning place and Instantiate an enemy
        for (int i = 0; i < _numberOfEnemyEntities; i++)
        {
            int _randomIndex = Random.Range(1, listOfFloorTiles.Count);
            Vector2 _randomVector = listOfFloorTiles[_randomIndex];
            Entity npcInstance = new Entity((int)_randomVector.x, (int)_randomVector.y, "Enemy", enemyObjectPrefab, new Vector3(_randomVector.x, _randomVector.y, 0));
            Instantiate(npcInstance.entityGameObject, npcInstance.entityLocation, Quaternion.identity);
            listOfEnemyEntities.Add(npcInstance); // We use this later on to calculate closes FOV at CalculateEntityClosestFOV(listOfEnemyEntities);
            //listOfEnemyEntitiesGO.Add(enemyObjectPrefab);


        }
    }

    void PlaceItems() {

        ItemGenerator itemGenerator = new ItemGenerator();
        itemGenerator.GenerateItems(listOfFloorTiles);
    }

    void PlaceExit()
    {
        GameObject _exit = Resources.Load("Prefabs/ItemExit") as GameObject;
        int _randomIndex = Random.Range(1, listOfFloorTiles.Count);
        Vector2 _randomVector = listOfFloorTiles[_randomIndex];
        Instantiate(_exit, new Vector3(_randomVector.x + 0.5f, _randomVector.y + 0.5f, 0), Quaternion.identity);
        //TESTING:
        //Instantiate(_exit, new Vector3(56 + 0.5f, 56 + 0.5f, 0), Quaternion.identity);
    }

    // Is inside our GridGenerator because regenerates / re-paints our grid constantly
    void CalculateEntityClosestFOV(List<Entity> _listOfEntities) {

        foreach (var enemy in _listOfEntities)
        {
            if (enemy != null)
            {
                Vector3 _entityLocation = new Vector3(enemy.entityLocation.x, enemy.entityLocation.y, 0); // Gets the position vector of each entity
                //Debug.Log("Enemy at: " + _entityLocation.ToString());
                int _entityVisibilityRadius = 5; // 
                                                 //int _entityVisibilityDiameter = 6; // _entityVisibilityRadius * 2
                int _offsetQuadrant4 = (int)_entityLocation.x - _entityVisibilityRadius; // Top left corner of the enemy quadrant (4). Position -3
                int _offsetQuadrant1 = (int)_entityLocation.y - _entityVisibilityRadius; // Top left corner of the enemy quadrant (4). Position -3
                                                                                         //int _offsetQuadrant1 = (int)_entityLocation.x + _entityVisibilityRadius; // Top right corner of the enemy quadrant (1) 
                                                                                         //int _offsetQuadrant3 = (int)_entityLocation.x - _entityVisibilityRadius;

                //Debug.Log("DEBUG: VALID QUADRANT");
                for (int x = _offsetQuadrant4; x < _entityLocation.x + _entityVisibilityRadius; x++) // From offset x -- (4) to offset x ++ (1). From X=-3 to X=6
                {
                    for (int y = _offsetQuadrant1; y < _entityLocation.y + _entityVisibilityRadius; y++) // From offset y -- (3)to offset y ++ (2)
                    {
                        if (floorMap.GetTile(new Vector3Int(x, y, 0)) != null && floorMap.tag == "Floor")
                        {
                            //Debug.Log("DEBUG: " + new Vector3(x, y, 0));
                            _testing_closestFovMap.SetTile(new Vector3Int(x, y, 0), floorTile);
                            _testing_closestFovMap.SetColor(new Vector3Int(x, y, 0), Color.red);
                        }
                        else if (wallMap.GetTile(new Vector3Int(x, y, 0)) != null && wallMap.tag == "Wall")
                        {
                            //Debug.Log("DEBUG: " + new Vector3(x, y, 0));
                            _testing_closestFovMap.SetTile(new Vector3Int(x, y, 0), wallTile);
                            _testing_closestFovMap.SetColor(new Vector3Int(x, y, 0), Color.red);
                        }
                    }
                }
            }
        }
    }

    // Adapted from CalculateEntityClosestFOV , as we can't pass the Player as an argument because is not generated as an Entity, once we change this we can get rid of the function as well.
    // Is inside our GridGenerator because regenerates / re-pain our grid constantly
    void CalculatePlayerClosestFOV()
    {

            if (playerReference != null)
            {
                Vector3 _entityLocation = new Vector3(playerReference.localPosition.x, playerReference.localPosition.y, 0); // Gets the position vector of each entity
                //Debug.Log("Enemy at: " + _entityLocation.ToString());
                int _entityVisibilityRadius = playerVisibilityRadius; // 
                int _entityVisibilityDiameter = _entityVisibilityRadius * 2; // _entityVisibilityRadius * 2
                int _offsetQuadrant4 = (int)_entityLocation.x - _entityVisibilityRadius; // Top left corner of the enemy quadrant (4). Position -3
                int _offsetQuadrant1 = (int)_entityLocation.y - _entityVisibilityRadius; // Top left corner of the enemy quadrant (4). Position -3
                                                                                         //int _offsetQuadrant1 = (int)_entityLocation.x + _entityVisibilityRadius; // Top right corner of the enemy quadrant (1) 
                                                                                         //int _offsetQuadrant3 = (int)_entityLocation.x - _entityVisibilityRadius;

                //Debug.Log("DEBUG: VALID QUADRANT");
                for (int x = _offsetQuadrant4; x < _entityLocation.x + _entityVisibilityRadius; x++) // From offset x -- (4) to offset x ++ (1). From X=-3 to X=6
                {
                    for (int y = _offsetQuadrant1; y < _entityLocation.y + _entityVisibilityRadius; y++) // From offset y -- (3)to offset y ++ (2)
                    {
                        if (floorMap.GetTile(new Vector3Int(x, y, 0)) != null && floorMap.tag == "Floor")
                        {
                            //Debug.Log("DEBUG: " + new Vector3(x, y, 0));
                            _testing_closestFovMap.SetTile(new Vector3Int(x, y, 0), floorTile);
                            _testing_closestFovMap.SetColor(new Vector3Int(x, y, 0), Color.white);
                        }
                        else if (wallMap.GetTile(new Vector3Int(x, y, 0)) != null && wallMap.tag == "Wall")
                        {
                            //Debug.Log("DEBUG: " + new Vector3(x, y, 0));
                            _testing_closestFovMap.SetTile(new Vector3Int(x, y, 0), wallTile);
                            _testing_closestFovMap.SetColor(new Vector3Int(x, y, 0), Color.white);
                        }
                    }
                }

            // Re-drawns these tiles that are outside CalculatePlayerClosestFOV()
            ResetTilesOutsideFOV(_entityVisibilityDiameter, _entityVisibilityRadius, _offsetQuadrant4, _entityLocation);

        }

    }

    void ResetTilesOutsideFOV(int _entityVisibilityDiameter, int _entityVisibilityRadius, int _offsetQuadrant4, Vector3 _entityLocation)
    {
        for (int i = 0; i < _entityVisibilityDiameter + 1; i++)
        {
            _testing_closestFovMap.SetTile(new Vector3Int(_offsetQuadrant4 + i, (int)_entityLocation.y + _entityVisibilityRadius, 0), null);
            //Leave this for debugging: 
            //_testing_closestFovMap.SetColor(new Vector3Int(_offsetQuadrant4 + i, (int)_entityLocation.y + _entityVisibilityRadius, 0), Color.yellow);
        }
        // Bottom
        for (int i = 0; i < _entityVisibilityDiameter; i++)
        {
            _testing_closestFovMap.SetTile(new Vector3Int(_offsetQuadrant4 + i, (int)_entityLocation.y - _entityVisibilityRadius, 0), null);
            //Leave this for debugging: 
            //_testing_closestFovMap.SetColor(new Vector3Int(_offsetQuadrant4 + i, (int)_entityLocation.y - _entityVisibilityRadius , 0), Color.yellow);
        }
        // Left
        for (int i = 0; i < _entityVisibilityDiameter; i++)
        {
            _testing_closestFovMap.SetTile(new Vector3Int(_offsetQuadrant4, (int)_entityLocation.y - _entityVisibilityRadius + i, 0), null);
            //Leave this for debugging: 
            //_testing_closestFovMap.SetColor(new Vector3Int(_offsetQuadrant4 , (int)_entityLocation.y - _entityVisibilityRadius + i, 0), Color.yellow);
        }
        // Right
        for (int i = 0; i < _entityVisibilityDiameter + 1; i++)
        {
            _testing_closestFovMap.SetTile(new Vector3Int((int)_entityLocation.x + _entityVisibilityRadius, (int)_entityLocation.y - _entityVisibilityRadius + i, 0), null);
            //Leave this for debugging: 
            //_testing_closestFovMap.SetColor(new Vector3Int((int)_entityLocation.x + _entityVisibilityRadius, (int)_entityLocation.y - _entityVisibilityRadius + i , 0), Color.yellow);
        }
    }

    // ORIGINAL
    struct Coord
    {
        public int tileX;
        public int tileY;

        public Coord(int x, int y) {

            tileX = x;
            tileY = y;
        }
    }



    //void ProcessMap() {
    //    // no modifications = o in both threesholds

    //    List<List<Coord>> wallRegions = GetRegions(1); // Get regions of walls (1)


    //    int wallThresholdSize = 0; // 100 map is almost empty, 10 opens up map, 3 no sucede casi nada pero veo bordes desaparecer.
    //    foreach (List<Coord> wallRegion in wallRegions)
    //    {
    //        if (wallRegion.Count < wallThresholdSize)
    //        {
    //            foreach (Coord tile in wallRegion)
    //            {
    //                // This is getting holes in the map and filling them up with walls:
    //                map[tile.tileX, tile.tileY] = 0; // setting it to an empty space.
    //                wallMap.SetTile(new Vector3Int((int)tile.tileX, (int)tile.tileY, 0), null);
    //                //floorMap.SetTile(new Vector3Int((int)tile.tileX, (int)tile.tileY, 0), floorTile);
    //                //floorMap.SetColor(new Vector3Int((int)tile.tileX, (int)tile.tileY, 0), Color.red);
    //                wallMap.SetTile(new Vector3Int((int)tile.tileX, (int)tile.tileY, 0), wallTile);
    //                wallMap.SetColor(new Vector3Int((int)tile.tileX, (int)tile.tileY, 0), Color.red);

    //            }
    //        }
    //    }
    //    Debug.Log("How many wallRegions?" + wallRegions.Count.ToString()); // 932 wrong

    //    // TODO este no funciona para nada, está cogiendo todas las walls del escenario una por una
    //    // We set a threeshold for Floors, if our room size is smaller (<) than our thresshold, 
    //    List<List<Coord>> roomRegions = GetRegions(0); // Get regions of floors (0)

    //    int roomThresholdSize = 0; // 100 nothing happened, 10 nothing, 3 nothing.
    //    List<Room> survivingRooms = new List<Room>(); // Added later on: List of rooms which survive the culling process:

    //    foreach (List<Coord> roomRegion in roomRegions)
    //    {
    //        if (roomRegion.Count < roomThresholdSize) // If our tile count for the rooms is < than our threeshold, then remove athe floor?
    //        {
    //            foreach (Coord tile in roomRegion)
    //            {
    //                map[tile.tileX, tile.tileY] = 1; // setting it to null floor, and then adding a walled space
    //                wallMap.SetTile(new Vector3Int((int)tile.tileX, (int)tile.tileY, 0), null);
    //                //wallMap.SetTile(new Vector3Int((int)tile.tileX, (int)tile.tileY, 0), wallTile);
    //                //wallMap.SetColor(new Vector3Int((int)tile.tileX, (int)tile.tileY, 0), Color.red);
    //                floorMap.SetTile(new Vector3Int((int)tile.tileX, (int)tile.tileY, 0), floorTile);
    //                floorMap.SetColor(new Vector3Int((int)tile.tileX, (int)tile.tileY, 0), Color.red);

    //            }
    //        }
    //        else
    //        {
    //            survivingRooms.Add(new Room(roomRegion, map));
    //        }
    //    }
    //    Debug.Log("How many roomRegions?" + roomRegions.Count.ToString()); // Doesn't even trigger. Code does not reach.

    //    ConnectClosestRooms(survivingRooms); // At the end of the process we'll pass our list of surviving rooms to connect them to eachother
    //}

    // Method that given a certain tile type can return all of the regions of tha type, a list of regions instead of coordinates. A list of a list of coordinates
    //List<List<Coord>> GetRegions(int tileType)
    //{

    //    // TODO this seems to fail and is teturning the initial list.
    //    List<List<Coord>> regions = new List<List<Coord>>();
    //    int[,] mapFlags = new int[mapWidthX, mapHeightY]; // which regions we've already look at

    //    // Then we want to look into all the adjacent tiles:
    //    for (int x = 0; x < mapWidthX; x++)
    //    {
    //        for (int y = 0; y < mapHeightY; y++)
    //        {
    //            // We go through all map and check if these flags are equal to 0 (Not checked) && tile at x and y is the right type (0, floor, 1 wall)
    //            if (mapFlags[x, y] == 0 && map[x, y] == tileType)
    //            {
    //                // then we want to create a new list of coordinates:
    //                List<Coord> newRegion = GetRegionTiles(x, y);// We'll create a new list of coordinates via GetRegionTiles() and passing x and y as starting values
    //                regions.Add(newRegion);

    //                // mark all tiles of the new region as "looked at"
    //                foreach (Coord tile in newRegion)
    //                {
    //                    mapFlags[tile.tileX, tile.tileY] = 1; // 1= looked at
    //                }
    //            }
    //        }
    //    }

    //    Debug.Log("regions" + regions.Count.ToString());
    //    return regions; // return a list of regions // TODO: Problem, regions return 900+when should be 2 or 3.
    //}

    // This gets if the tiles are set to 0: floor or 1: wall
    //List<Coord> GetRegionTiles(int startX, int startY)
    //{

    //    List<Coord> tiles = new List<Coord>(); // To store tiles, either stores the floors or the walls.
    //    int[,] mapFlags = new int[mapWidthX, mapHeightY]; // System.Int32[106,30] which tiles we've already look at, 1 vs 0 
    //    ////TODO this seems to be broken here: in tileType
    //    int tileType = map[startX, startY]; // 0 // Which type of Tile are we looking for

    //    Queue<Coord> _queue = new Queue<Coord>(); // create a new queue of coordinates
    //    _queue.Enqueue(new Coord(startX, startY)); // enqueue our first coordinate

    //    mapFlags[startX, startY] = 1; // Set this to one to mark that we have already looked at this tile (set to 1)
    //    // Last element [105,29] so this was enqueued correctly

    //    while (_queue.Count > 0) // While there's still stuff in the queue...
    //    {

    //        //    //Debug.Log("Queue: " + _queue.Count.ToString());

    //        Coord tile = _queue.Dequeue(); // returns the first item in the queue, AND removed the item from the queue
    //        tiles.Add(tile); // We add this tile to our list
    //                         //    Debug.Log("added" + tile.ToString());


    //        // Then we want to look into all the adjacent tiles:
    //        for (int x = tile.tileX - 1; x < tile.tileX + 1; x++)
    //        {
    //            for (int y = tile.tileY - 1; y < tile.tileY + 1; y++)
    //            {
    //                //            // Check if tile is in range and NOT diagonal
    //                if (H_IsInMapRange(x, y) && (y == tile.tileY || x == tile.tileX))
    //                {
    //                    //                // We make sure we haven't checked this vector yet && we want to make sure is the right tile of tile
    //                    if (mapFlags[x, y] == 0 && map[x, y] == tileType)
    //                    {
    //                        mapFlags[x, y] = 1; // We set the tile as checked
    //                        _queue.Enqueue(new Coord(x, y)); // And finally we add the new coordinate to the queue to be checked in the next iteration

    //                        //                    // Testing visible:
    //                        //                    map.SetTile(new Vector3Int(x, y, 0), floorTile);
    //                        //                    map.SetColor(new Vector3Int(x, y, 0), Color.red);
    //                    }
    //                }
    //            }
    //        }
    //    }


    //    return tiles;
    //    //Count = 3180 seems to sum both walls and floors
    //    //Count = 714 with the floortile = 1 check which is more similar to 932 via floorlist check
    //}

    //void ConnectClosestRooms(List<Room> allRooms)
    //{
    //    int bestDistance = 0;
    //    Coord bestTileA = new Coord(); // We'll store here which tiles resulted as "best distance"
    //    Coord bestTileB = new Coord(); // We'll store here which tiles resulted as "best distance"
    //    Room bestRoomA = new Room(); // We'll want to know from which room the best tiles come from. This is why we make the second empty constructor for room
    //    Room bestRoomB = new Room();
    //    bool possibleConnectionFound = false;

    //    // we'll go through all rooms and compare them to every other room to find the closest one
    //    foreach (Room roomA in allRooms)
    //    {
    //        possibleConnectionFound = false; // Once best connection is found, its going to move on to the next room, so we want to reset this.

    //        foreach (Room roomB in allRooms)
    //        {
    //            // case1: At some point roomA = roomB and we don't want to try to find a connecting to the same room, we'll skip ahead to the next
    //            if (roomA == roomB)
    //            {
    //                continue;
    //            }
    //            // case2: roomA is actually connected to roomB, as already has a connection we'll break, skip all of this and go to next room A
    //            if (roomA.isConnected(roomB))
    //            {
    //                possibleConnectionFound = false; // so it does not create the pagae anyway when exiting the loop and hitting "if (possibleConnectionFound)" check
    //                break;
    //            }
    //            // Look at the distance between all the edge tiles in both rooms:
    //            for (int tileIndexA = 0; tileIndexA < roomA.edgeTiles.Count; tileIndexA++)
    //            {
    //                for (int tileIndexB = 0; tileIndexB < roomB.edgeTiles.Count; tileIndexB++)
    //                {
    //                    // create a coordinate:
    //                    Coord tileA = roomA.edgeTiles[tileIndexA];
    //                    Coord tileB = roomB.edgeTiles[tileIndexB];
    //                    int distanceBetweenRooms = (int)Mathf.Pow(tileA.tileX - tileB.tileX, 2) + (int)Mathf.Pow(tileA.tileY - tileB.tileY, 2);

    //                    if (distanceBetweenRooms < bestDistance || !possibleConnectionFound)
    //                    {
    //                        bestDistance = distanceBetweenRooms;
    //                        possibleConnectionFound = true;
    //                        bestTileA = tileA;
    //                        bestTileB = tileB;
    //                        bestRoomA = roomA;
    //                        bestRoomB = roomB;
    //                    }
    //                }
    //            }
    //        }

    //        // Once we finish the foreach() and found out our connection between A and B
    //        if (possibleConnectionFound)
    //        {
    //            CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
    //        }
    //    }
    //}

    //void CreatePassage(Room roomA, Room roomB, Coord tileA, Coord tileB) {

    //    // We tell them that they're now connected to eachother
    //    Room.ConnectRooms(roomA,roomB);
    //    Debug.DrawLine(CoordToWorldPoint(tileA), CoordToWorldPoint(tileB), Color.green, 100);
    //}

    // A helper method that transforms a coordinate in an actual world position
    //Vector3 CoordToWorldPoint(Coord tile) {

    //    return new Vector3(-mapWidthX/ 2 + 0.5f + tile.tileX, 2, -mapHeightY / 2 + 0.5f + tile.tileY);
    //}

    //void GetRegions() {

    //    Debug.Log("Getting regions ... ");

    //    // At this point we already can calculate listOfRegion1FloorTiles, so listOfRegion2FloorTiles could be the rest of tiles

    //    foreach (var item in listOfFloorTiles)
    //    {
    //        if (!listOfRegion1FloorTiles.Contains(item))
    //        {
    //            listOfRegion2FloorTiles.Add(item);
    //        }
    //    }


    //    // Get all floor tiles that don't have wall neighbor?
    //    //1) GET ALL FLOOR TILES
    //    //2) SET THEM TO GROUP 0
    //    //3) GO OVER THE GRID TILE, EVERYTIME A GROUP 0 IS MET, MARK IT AS NEXT REGION (1,2,...)
    //    //4)

    //    // TESTING QUEUES:
    //    //Queue<Vector2> _queue = new Queue<Vector2>();

    //    //_queue.Enqueue(new Vector2(0, 0)); // adds element to the END of the queue
    //    //_queue.Enqueue(new Vector2(1, 0));
    //    //_queue.Enqueue(new Vector2(2, 0));
    //    //_queue.Enqueue(new Vector2(3, 0));

    //    //foreach (var item in _queue)
    //    //{
    //    //    Debug.Log(item.ToString());
    //    //}

    //    //_queue.Dequeue(); // removes oldest element from the start of the queue . In this case (0,0)

    //    //foreach (var item in _queue)
    //    //{
    //    //    Debug.Log(item.ToString());
    //    //}


    //}


    void GetBorderVolumes() {

        foreach (var item in listOfFloorTiles) {

            Vector2 neighborUp = new Vector2(item.x, item.y + 1);
            Vector2 neighbordown = new Vector2(item.x, item.y - 1);
            Vector2 neighborLeft = new Vector2(item.x - 1, item.y);
            Vector2 neighborRight = new Vector2(item.x + 1, item.y);

            if (!listOfFloorTiles.Contains(neighborUp)) //|| listOfFloorTiles.Contains(neighbordown) || listOfFloorTiles.Contains(neighborLeft) || listOfFloorTiles.Contains(neighborRight))
            {
                floorMap.SetColor(new Vector3Int((int)item.x, (int)item.y, 0), Color.magenta);

            }
            if (!listOfFloorTiles.Contains(neighbordown))
            {
                floorMap.SetColor(new Vector3Int((int)item.x, (int)item.y, 0), Color.magenta);

            }
            if (!listOfFloorTiles.Contains(neighborLeft))
            {
                floorMap.SetColor(new Vector3Int((int)item.x, (int)item.y, 0), Color.magenta);

            }
            if (!listOfFloorTiles.Contains(neighborRight))
            {
                floorMap.SetColor(new Vector3Int((int)item.x, (int)item.y, 0), Color.magenta);

            }
        }
    }

    //  Checks for borders in the level and paint them red graphically.
    void GetProcGenMapBorders() {


        //1- Use listOfFloorTiles
        // TODO : Using SetcColor works without set tile, most likely this gets broken from this somwhere along the line of generating the level
        foreach (var item in listOfFloorTiles)
        {
            Vector2 neighborUp = new Vector2(item.x, item.y+1);
            Vector2 neighbordown = new Vector2(item.x, item.y - 1);
            Vector2 neighborLeft = new Vector2(item.x-1, item.y);
            Vector2 neighborRight = new Vector2(item.x + 1, item.y);

            // If the up neighbor is in the list as well, means is a floor, do nothing
            if (!listOfFloorTiles.Contains(neighborUp)) //|| listOfFloorTiles.Contains(neighbordown) || listOfFloorTiles.Contains(neighborLeft) || listOfFloorTiles.Contains(neighborRight))
            {
                wallMap.SetColor(new Vector3Int((int)neighborUp.x, (int)neighborUp.y, 0), Color.red);
                borders.Add(new Vector2(neighborUp.x, neighborUp.y));
                //continue; //skips to the next part, breaking the loop will get us entirely out

            }
            if (!listOfFloorTiles.Contains(neighbordown)) //|| listOfFloorTiles.Contains(neighbordown) || listOfFloorTiles.Contains(neighborLeft) || listOfFloorTiles.Contains(neighborRight))
            {
                wallMap.SetColor(new Vector3Int((int)neighbordown.x, (int)neighbordown.y, 0), Color.red);
                borders.Add(new Vector2(neighbordown.x, neighbordown.y));
                //continue; //skips to the next part, breaking the loop will get us entirely out

            }
            if (!listOfFloorTiles.Contains(neighborLeft)) //|| listOfFloorTiles.Contains(neighbordown) || listOfFloorTiles.Contains(neighborLeft) || listOfFloorTiles.Contains(neighborRight))
            {
                wallMap.SetColor(new Vector3Int((int)neighborLeft.x, (int)neighborLeft.y, 0), Color.red);
                borders.Add(new Vector2(neighborLeft.x, neighborLeft.y));
                //continue; //skips to the next part, breaking the loop will get us entirely out

            }
            if (!listOfFloorTiles.Contains(neighborRight)) //|| listOfFloorTiles.Contains(neighbordown) || listOfFloorTiles.Contains(neighborLeft) || listOfFloorTiles.Contains(neighborRight))
            {
                wallMap.SetColor(new Vector3Int((int)neighborRight.x, (int)neighborRight.y, 0), Color.red);
                borders.Add(new Vector2(neighborLeft.x, neighborLeft.y));
                //continue; //skips to the next part, breaking the loop will get us entirely out

            }
        }

    }

    void ProcGenAdditionalWalkers() {
        /* TODO Needs GetProcGenMapBorders(); to be running, in order to work, move into a different method */

        // Get border locations from GetProcGenMapBorders, this exists already in List borders
        //  use this data to create more walkers
        //int _fit = 0;
        // Get various offsets:
        // Map center limits
        // center 53,53
        // x: 0,53  , 106,53
        // y: 53,0 , 53,106
        // 25 - 75 looks good for offsetting

        // 1) Set area borders
        //Vector2 _upleft = new Vector2(25,75);
        //Vector2 _upright = new Vector2(75, 75);
        //Vector2 _downleft = new Vector2(25, 25);
        //Vector2 _downright = new Vector2(75, 25);

        List<Walker> _additionalWalkers = new List<Walker>();

        Debug.Log(" border tiles: " + borders.Count.ToString());

        foreach (var item in borders)
        {   
            // Creates level on the upper part of the map
            if (item.y > 75)
            {
                Walker _newWalker = new Walker();
                _newWalker.walkerPosition = new Vector2(item.x, item.y);
                //_newWalker.walkerPosition = RandomDirection(_newWalker.walkerPosition);
                _additionalWalkers.Add(_newWalker);
            }
            // Creates level on the bottom part of the map
            //else if (item.y < 50)
            //{
            //    Walker _newWalker = new Walker();
            //    _newWalker.walkerPosition = new Vector2(item.x, item.y);
            //    //_newWalker.walkerPosition = RandomDirection(_newWalker.walkerPosition);
            //    _additionalWalkers.Add(_newWalker);
            //}
        }

        // Once our new list of walkers for extending the level is created, we run the script to make them run.
        for (int i = 0; i < 1000; i++) // 1000 because ¯\_(ツ)_/¯
        {
            WalkerBehaviour(_additionalWalkers); // regenerates all tiles
        }

        // 2) Get location of tiles
        //for (int x = (int)_upleft.x; x < (int)_upright.x; x++)
        //{
        //    for (int y = (int)_downleft.x; y < (int)_downright.x; y++)
        //    {
        //        // This creates an area
        //        _centerGameArea.Add(new Vector2(x,y));

        //    }
        //}

        //foreach (var item in _centerGameArea)
        //{
        //    if (borders[item] > item)
        //    {
        //        //wallMap.SetColor(new Vector3Int((int)item.x, (int)item.y, 0), Color.red);
        //        _fit++;
        //    }
        //}

        //Debug.Log("fit++: " + _fit);
    }

    bool H_IsInMapRange(int x, int y) {

        return x >= 0 && x < mapWidthX && y >= 0 && y < mapHeightY;
    }

    //Tilemap FloodRegions(Tilemap tilemap) {

    //    //Tilemap tilemap = new Tilemap();

    //    //region1.FloodFill(new Vector3Int(1, 1, 0), wallTile);
    //    //tilemap.FloodFill(new Vector3Int(1, 1, 0), wallTile);
    //    tilemap.FloodFill(new Vector3Int(3, 27, 0), wallTile);

    //    return tilemap;
    //    //FloodRegion _region1 = new FloodRegion();
    //    //Queue<Vector2> _queue = new Queue<Vector2>();

    //    // _region1.floodRegion = // list of v2
    //    //foreach (var item in listOfFloorTiles)
    //    //{
    //    //    if (floorMap.GetTile(new Vector3Int((int)item.x, (int)item.y, 0)) == floorTile)
    //    //    {

    //    //    }
    //    //}

    //    //for (int x = 0; x < mapWidthX; x++)
    //    //{
    //    //    for (int y = 0; y < mapHeightY; y++)
    //    //    {
    //    //            if (floorMap.GetTile(new Vector3Int(x, y, 0)) == floorTile)
    //    //            {

    //    //            }
    //    //    }
    //    //}
    //}

    //void FloodingTilesWIP() { 
    
    //    List<Vector2> _region1 = new List<Vector2>();

    //    Vector2 _initialTile = new Vector2(1,1);
    //    Queue<Vector2> _queue = new Queue<Vector2>(); // create a new queue of coordinates

    //    _queue.Enqueue(_initialTile); // enqueue our first coordinate

    //    for (int x = 0; x < mapWidthX; x++)
    //    {
    //        for (int y = 0; y < mapHeightY; y++)
    //        {
    //            Vector2 _s = new Vector2(x + x, y + y); // check tiles around

    //            if (listOfFloorTiles.Contains(_s))
    //            {
    //                Vector2 _ok = new Vector2(x, y);
    //                Debug.Log("valid floor tiles: " + _s.ToString());
    //                _region1.Add(new Vector2(x,y)); // add current tile
    //                Debug.Log("adding _ok floor tiles: " + _ok.ToString());

    //            } else {
    //                Debug.Log("skipping tile: " + _s.ToString());
    //                continue;
    //            }
    //        }
    //    }
    //    //if (listOfFloorTiles.Contains(new Vector2(oneone.x, oneone.y-1)))
    //    //{
    //    //    Debug.Log("yes: y-1");
    //    //}
    //    //else
    //    //{
    //    //    Debug.Log("nope: y-1");
    //    //}
    //    //if (listOfFloorTiles.Contains(new Vector2(oneone.x+1, oneone.y)))
    //    //{
    //    //    Debug.Log("yes: x+1");
    //    //}
    //    //else
    //    //{
    //    //    Debug.Log("nope: x+1");
    //    //}

    //    // Add all x= 0 y = 1 vectors to _listOfFloodedFloors
    //    //for (int x = 0; x < mapWidthX; x++)
    //    //{
    //    //    _listOfFirstLine.Add(new Vector2(x, 1));
    //    //}

    //    //// for each item in _listOfFloodedFloors , is the same coincides with listOfFloorTiles then == floor
    //    //foreach (var item in _listOfFirstLine)
    //    //{
    //    //    if (listOfFloorTiles.Contains(item))
    //    //    {
    //    //        _region1.Add(item);
    //    //    } else {
    //    //        break; // stop when a wall is found
    //    //    }
    //    //}

    //    Debug.Log("region 1 contains: " + _region1.Count.ToString()); // 21 --> 14 when added the break; statement, so only gets the first region.
    //    // TODO: Improve to check the tiles around.



    //}

    void ProcGenBorderFixtures() {

        Debug.Log("border tiles: " + borders.Count.ToString());
    }

    /// <summary>
    ///  Creates a map of 0's and 1's for map[,]
    /// </summary>
    void SetIntegerMap() {

        map = new int[mapWidthX, mapHeightY];
        //Debug.Log(map.Length); //System.Int32[106,30]

        int _floors = 0;
        int _walls = 0;


        for (int x = 0; x < mapWidthX; x++)
        {
            for (int y = 0; y < mapHeightY; y++)
            {
                Vector2 _v = new Vector2(x,y);

                if (listOfFloorTiles.Contains(_v))
                {
                    map[x,y] = 1;
                    _floors++;
                }
                else
                {
                    map[x, y] = 0;
                    _walls++;

                }
            }
        }

        Debug.Log("Check FLOOR validation: " + listOfFloorTiles.Count.ToString() + "floor tiles && " + _floors.ToString());
        Debug.Log("Check WALLS validation: " + listOfWallTiles.Count.ToString() + "wall tiles && " + _walls.ToString());
    }

    public void NextFloor()
    {
        currentFloor++;
        __generateNextFloor = false;
        Debug.Log("Newfloor");

        // Clear the current floor
        __isMapReady = false;
        ClearReferences();

        // Generate a new floor
        ProcGenFloorWalkersSetup();
        TEST_ProcGenNewWalkers();
        ProcGenWallSetup();
        ProcGenWallFixtures(mapWidthX, mapHeightY);
        PlaceEntities();
        PlaceItems();
        PlaceExit();
        __isMapReady = true;
    }

}

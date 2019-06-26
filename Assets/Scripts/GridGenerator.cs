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
    public int playerVisibilityRadius;

    /* Debug checks for adding variety to the ProcGen map */
    //private bool _debug_procgen_tileDensity;
    private bool _debug_z_block_of_code = false;
    private bool _debug_show_level_no_FOV = false;
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
    List<Walker> listOfNonRandomWalkers = new List<Walker>();

    List<Vector2> listOfFloorTiles = new List<Vector2>(); // Will be a list of positions, not of tiles.
    List<Vector2> listOfWallTiles = new List<Vector2>();

    public List<Entity> listOfEnemyEntities = new List<Entity>();


    //public TileBase tileBase; // Asigned to our floor tilebase in the editor.

    // Start is called before the first frame update
    void Start()
    {
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
        // WIP: Testing new Walkers:
        //TEST_ProcGenNewWalkers();
        // Walls setup
        ProcGenWallSetup();
        // Testing filling map generation:
        TEST_ProcGenNewWalkers();
        // Close gaps in the map: Finds floorTiles in the borders and changes them for wallTiles
        ProcGenFixtures(mapWidthX, mapHeightY);
        // Place Entities
        PlaceEntities();
        // Helper function to get level information
        GetMapData();


    }
    
    void Update()
    {
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
            ProcGenFixtures(mapWidthX, mapHeightY);
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

        floorMap.ClearAllTiles(); // clears all floor tiles
        listOfFloorTiles.Clear(); // clear this count list as well
        listOfWallTiles.Clear(); // clear this count list as well
        listOfWalkers.Clear(); // clears all walkers for the new iteration this may need to go at the end of the script, or will clear them before generates the level.

    }

    // Procedural generation version of FloorSetup() . Will be using the Drunken Walker algorithm
    public void ProcGenFloorWalkersSetup()
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

        for (int i = 0; i < 1000; i++) // 1000 because ¯\_(ツ)_/¯
        {
            WalkersPopulationControl(); // regenerates all tiles
        }

        // Done -> 2: WalkersPopulationControl() -> Walkers gonna walk
        // TODO: 3: WalkersPopulationControl() -> Chance of dying . Initial tests look pretty bad, re-check in the future.
        // Done -> 4: WalkersPopulationControl() -> Chance of spawning a new one
        // TODO: 5: Set limit where the level can grow. Clamp walker values to the map limits

    }

    public void TEST_ProcGenNewWalkers() {


        for (int i = 0; i < 50; i++) // 50 because why not.
        {
            float _density = GetMapData();

            if (_density < 0.3f)
            {
                Walker _nonRandomWalker = new Walker();

                int _rand = Random.Range(0, listOfWallTiles.Count);
                //Debug.Log(_rand);
                Vector2 _randpos = listOfWallTiles[_rand]; // Index out of range, wut. ISSUE: At this point we still have no walls, so index will be zero.
                                                           //Debug.Log(listOfWallTiles[_rand].ToString());
                _nonRandomWalker.walkerPosition = new Vector2(_randpos.x, _randpos.y);
                Debug.Log("Created new walker at: " + _nonRandomWalker.walkerPosition.ToString());

                // Fix lists: 
                listOfWallTiles.Remove(_nonRandomWalker.walkerPosition);//removes this vector from the wall list, as is a floor now
                listOfFloorTiles.Add(_nonRandomWalker.walkerPosition);//adds a new floor tile
                listOfNonRandomWalkers.Add(_nonRandomWalker);//adds nonrandomwalker

                TEST_Moving_non_random_walkers();



            }
        }



        // top right corner
        //int dx = 106;
        //int dy = 30;
        //Walker _nonRandomWalker = new Walker();
        //// Initial position:
        //_nonRandomWalker.walkerPosition = new Vector2(dx, dy);
        //listOfNonRandomWalkers.Add(_nonRandomWalker);

        //for (int i = 0; i < 1000; i++) // 1000 because ¯\_(ツ)_/¯
        //{
        //    TEST_Moving_non_random_walkers();
        //}

    }

    public void TEST_Moving_non_random_walkers() {

        foreach (var _nonRandomWalker in listOfNonRandomWalkers)
        {
                _nonRandomWalker.walkerPosition = RandomDirection(_nonRandomWalker.walkerPosition);
                floorMap.SetTile(new Vector3Int((int)_nonRandomWalker.walkerPosition.x, (int)_nonRandomWalker.walkerPosition.y, 0), floorTile); // set new floor
                wallMap.SetTile(new Vector3Int((int)_nonRandomWalker.walkerPosition.x, (int)_nonRandomWalker.walkerPosition.y, 0), null); // unset old wall
            floorMap.SetColor(new Vector3Int((int)_nonRandomWalker.walkerPosition.x, (int)_nonRandomWalker.walkerPosition.y, 0), Color.green);

        }

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



        //PlaceEntities();

        //Debug.Log("Number of Tiles:" + listOfFloorTiles.Count.ToString());

        //foreach (var eachFloorTile in listOfFloorTiles)
        //{
        //    // TODO: Check the tiles around, if empty => set wall. Problem: TileBase does not have a "position" : https://forum.unity.com/threads/tilemap-tile-positions-assistance.485867/#post-3165299
        //    // iterate through all the coordinates on the grid beginning from the tilemap.origin up to tilemap.origin + tilemap.size. At each coordinate you can retrieve the TileBase that's associated with it, and do whatever you need to do.
        //    // So YES, it is necessary to go through the whole map again, not only the list of tiles.

        //}


    }
    // TODO: Bug Fix: Tiles added via this process don't go through FOV and are always visible. 
    void ProcGenFixtures(int width, int height)
    {
        // Seems that this may be the cause why shorter FOV was not working. If I generate a new tile in Floor or Walls, it inherits the grey color and is not blue. I need to use a new tilemap for setting a new color because is set by TILEMAP, not by TILE!!!

        // Fills with walls the bottom and ceiling map openings from the walkers
        for (int i = 0; i < width; i++)
        {
            if (floorMap.GetTile(new Vector3Int(i, 0, 0)) != null)
            {
                _testing_map.SetTile(new Vector3Int(i, 0, 0), wallTile);
                _testing_map.SetColor(new Vector3Int(i, 0, 0), Color.green);
            }
            if (floorMap.GetTile(new Vector3Int(i, height-1, 0)) != null)
            {
                _testing_map.SetTile(new Vector3Int(i, height-1, 0), wallTile);
                _testing_map.SetColor(new Vector3Int(i, height-1, 0), Color.green);
            }
        }
        // Fills with walls the right and left map openings from the walkers
        for (int i = 0; i < height; i++)
        {
            if (floorMap.GetTile(new Vector3Int(0, i, 0)) != null)
            {
                _testing_map.SetTile(new Vector3Int(0, i, 0), wallTile);
                _testing_map.SetColor(new Vector3Int(0, i, 0), Color.green);
            }
            if (floorMap.GetTile(new Vector3Int(i, height - 1, 0)) != null)
            {
                _testing_map.SetTile(new Vector3Int(height, i, 0), wallTile);
                _testing_map.SetColor(new Vector3Int(height, i, 0), Color.green);
            }
        }
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

        } 
        // TODO: This may cause troubles of non-connected rooms, when we TELEPORT the walker somewhere else
        else if (currentWalkerPosition.x > mapWidthX) // Checks if x wants to go outside boundaries
        {

            // This would fill up the (0,0) corner
            TeleportWalker(new Vector2(currentWalkerPosition.x, currentWalkerPosition.y)); // TODO for now this does nothing, is just a WIP
            currentWalkerPosition.x = 0; //Approach 3: Teleport to a corner, but map feels a blob as is filling up only in the middle , so we teleport the walker entirely, not only one axis.
            currentWalkerPosition.y = 0;

        }
        else if (currentWalkerPosition.y > mapHeightY) // Checks if y wants to go outside boundaries
        {

            // This would fill up the (max,max) corner , see approach 3 above
            TeleportWalker(new Vector2(currentWalkerPosition.x, currentWalkerPosition.y)); // TODO for now this does nothing, is just a WIP
            currentWalkerPosition.x = mapWidthX;
            currentWalkerPosition.y = mapHeightY;
        }

        return new Vector2(currentWalkerPosition.x, currentWalkerPosition.y);

    }

    Vector2 NonRandomDirection(Vector2 currentWalkerPosition)
    {
    
        // As non-random direction we can use a weighted method.
        // Expecting the one that enters here comes from x=106y=30 so we want to go to the opposite corner, aka= 0,0 x-- y --
        float _rand = (int)Random.Range(0, 2); // 0, 1, 2, 3
        switch (_rand)
        {
            //case 0: //up x=0 y++
                //currentWalkerPosition.y += 1.0f;
                //break;
            case 0: //down x=0 y--
                currentWalkerPosition.y -= 1.0f;
                break;
            case 1: //left x-- y=0
                currentWalkerPosition.x -= 1.0f;
                break;
            //case 3://right x++ y=0
                //currentWalkerPosition.x += 1.0f;
                //break;
            default:
                break;

        }

        return new Vector2(currentWalkerPosition.x, currentWalkerPosition.y);

    }

    Vector2 TeleportWalker(Vector2 currentWalkerPosition) {

        return new Vector2(currentWalkerPosition.x, currentWalkerPosition.y);
    }

    private float GetMapData() {

        //float _mapDensity = (listOfWallTiles.Count / listOfFloorTiles.Count) * 100;
        float _mapDensity = (float)listOfFloorTiles.Count / (mapWidthX * mapHeightY);
        //Debug.Log(listOfFloorTiles.Count);
        //Debug.Log(mapWidthX);
        //Debug.Log(mapHeightY);
        Debug.Log("Map Density " + (_mapDensity * 100) + "% | Floor: " + listOfFloorTiles.Count.ToString() + " | Walls: " + listOfWallTiles.Count.ToString());

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
        // Random number of enemies:
        int _numberOfEnemyEntities = Random.Range(1,10);
        //Debug.Log("Monsters: " + _numberOfEnemyEntities);

        // Reference to which Prefab to use:
        GameObject _test_npc = Resources.Load<GameObject>("Prefabs/Enemy");

        // For each entity to be created, find a suitable spawning place and Instantiate an enemy
        for (int i = 0; i < _numberOfEnemyEntities; i++)
        {
            //Debug.Log("listOfFloorTiles count: " + listOfFloorTiles.Count.ToString());
            int _randomIndex = Random.Range(1, listOfFloorTiles.Count);
            Vector2 _randomVector = listOfFloorTiles[_randomIndex];

            Entity npcInstance = new Entity((int)_randomVector.x, (int)_randomVector.y, "Enemy", _test_npc, new Vector3(_randomVector.x, _randomVector.y, 0));

            //Engine.SchedulingSystem.Add(npcInstance); // We don't instantiate a new schedule, but add the npc's to the global instance declared in Engine.cs
            Debug.Log("npc instances are added");
            Instantiate(npcInstance.entityGameObject, npcInstance.entityLocation, Quaternion.identity);
            Engine.SchedulingSystem.Add(npcInstance);
            //npcInstance.name = npcInstance.tag;

            listOfEnemyEntities.Add(npcInstance); // Add current enemies to a List 
            // list[index]
            //Debug.Log("Available vector: " + _randomVector.ToString());
        }

        // WIP: Testing create new Monster class. KINDA WORKS.
        //monsterTest.entityLocation = new Vector3(6.5f, 6.5f, 0);
        //monsterTest.entityGameObject = Resources.Load<GameObject>("Prefabs/EnemyRed");
        //monsterTest.entityName = "the monster";
        Monster monsterTest = new Monster(); //(6, 6, "Monster", _test_npc, new Vector3(6,6,0)); // ERROR Monster does not have a constructor that accept 5 arguments.
        Instantiate(monsterTest.entityGameObject, new Vector3(6.5f, 6.5f, 0), Quaternion.identity); // This monster clone appears on 6,6 as expected, but the game object is still in 0,0 , wut
        Engine.SchedulingSystem.Add(monsterTest);

    }

    // Is inside our GridGenerator because regenerates / re-pain our grid constantly
    void CalculateEntityClosestFOV(List<Entity> _listOfEntities) {

        foreach (var enemy in _listOfEntities)
        {
            if (enemy != null)
            {
                Vector3 _entityLocation = new Vector3(enemy.entityLocation.x, enemy.entityLocation.y, 0); // Gets the position vector of each entity
                //Debug.Log("Enemy at: " + _entityLocation.ToString());
                int _entityVisibilityRadius = 3; // 
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

    // TODO: We can remove this after testing, Player is not an Entity as well.
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
}

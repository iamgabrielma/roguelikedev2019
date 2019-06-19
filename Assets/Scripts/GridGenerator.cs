using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// new dependencies:
using UnityEngine.Tilemaps;
using UnityEditor;
// Diagnostics and debugging
using System.Diagnostics; // enables System.Diagnostics.StopWatch
using System; // enables System.TimeSPan

public class GridGenerator : MonoBehaviour
{
    // The tile map stores sprites in a layout marked by a Grid component. Ref: https://docs.unity3d.com/2017.2/Documentation/ScriptReference/Tilemaps.Tilemap.html
    public Tilemap floorMap; // TODO: change to Tilemap highlightMap = GetComponent<TileMap>();
    public Tilemap wallMap;
    public Tile floorTile; // The Tile class is a simple class that allows a sprite to be rendered on the Tilemap.. Ref: https://docs.unity3d.com/2017.2/Documentation/Manual/Tilemap-ScriptableTiles-TileBase.html
    public Tile wallTile;
    //public TileBase tileBase; // Asigned to our floor tilebase in the editor.

    // Start is called before the first frame update
    void Start()
    {
        // We assigned the child TileMap from the Grid object for highlightMap directly in the inspector

        /* __This works__ */
        // https://docs.unity3d.com/ScriptReference/Vector3Int.html
        //highlightMap.SetTile(new Vector3Int(0,0,0), tileBase);
        //Debug.Log (highlightMap.GetTile(Vector3Int.zero)); // null if there's no tile, TileBase.tileBase if there's tile.
        /* __end__This works__end__ */

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        FloorSetup(106, 30);
        WallsSetup(106, 30);
        stopwatch.Stop();
        TimeSpan ts = stopwatch.Elapsed;
        int _ms = ts.Milliseconds; // Calculated 4ms vs 14ms for a 20x20 via Prefabs. 27ms in tilemaps vs 170 in prefabs for 80x80 map. Staying with TileMaps!
        // 16ms for generating both floors and walls on a 106x30 tileset

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void FloorSetup(int width, int height)
    {
        //GameObject floorTile = tiles[0].tileObject;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //GameObject _floorTile = Instantiate(floorTile, new Vector3(x, y, 0), Quaternion.identity);
                //_floorTile.transform.SetParent(_floorHolder);
                //highlightMap.SetTile(new Vector3Int(x, y, 0), tileBase);
                floorMap.SetTile(new Vector3Int(x, y, 0), floorTile);
                /* highLightTile initially was failing to work and I had to use TileBase.tileBase because I had created a custom class named Tile.cs , now this is overwritten as _Tile and the default functionality works nicely*/

            }
        }
    }

    public void WallsSetup(int width, int height)
    {

        //GameObject _test_wallObject = Resources.Load<GameObject>("Prefabs/Wall1");

        // TODO: For memory sake, shall we draw only the tiles that are visible? Needs Analysis

        // Fills from 20,20 to 20, -1
        for (int y = width; y >= -1; y--)
        {
            //GameObject _wallTile = Instantiate(_test_wallObject, new Vector3(width, y, 0), Quaternion.identity);
            //_wallTile.transform.SetParent(_wallHolder);
            wallMap.SetTile(new Vector3Int(width, y, 0), wallTile);
        }
        // Fills from -1,20 to -1, -1
        for (int y = width; y >= -1; y--)
        {
            //GameObject _wallTile = Instantiate(_test_wallObject, new Vector3(-1, y, 0), Quaternion.identity);
            //_wallTile.transform.SetParent(_wallHolder);
            wallMap.SetTile(new Vector3Int(-1, y, 0), wallTile);
        }
        // Fills from 0,0 to 20, 0
        for (int x = -1; x < height; x++)
        {
            //GameObject _wallTile = Instantiate(_test_wallObject, new Vector3(x, -1, 0), Quaternion.identity);
            //_wallTile.transform.SetParent(_wallHolder);
            wallMap.SetTile(new Vector3Int(x, -1, 0), wallTile);
        }
        // Fills from 0,20 to 20,20
        for (int x = -1; x < height; x++)
        {
            //GameObject _wallTile = Instantiate(_test_wallObject, new Vector3(x, height, 0), Quaternion.identity);
            //_wallTile.transform.SetParent(_wallHolder);
            wallMap.SetTile(new Vector3Int(x, height, 0), wallTile);
        }


    }
}

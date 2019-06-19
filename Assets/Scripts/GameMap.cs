using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Diagnostics;
using System;

// TODO: deprecate in favor of tilemaps and GridGenerator.cs
public class GameMap: MonoBehaviour
{
    // TODO: the comments with an arrow //-> are because the switch from prefab mapgen to tilemap mapgen, I had to rename Tile to _Tile and this was throwing errors, but needs a full rewrite anyway.
    //->public Tile[] tiles;

    public Transform _floorHolder;

    // 30 rows allow each row to be 36 pixels high in 1080p, the most common monitor size.
    readonly int mapX_width = 106;
    readonly int mapY_height = 30;

    private void Start()
    {

        _floorHolder = new GameObject("floorHolder").transform; // Will hold all our floor tiles, so the Inspector is not cluttered with GameObjects

        //foreach (var tile in tiles)
        //{
        //    Debug.Log(tile.name);
        //    Debug.Log(tile.InspectTile());

        //    // TODO: This seems to work, I can move WallsSetup and FloorSetup here from Engine.cs
        //    if (tile.tileObject != null)
        //    {
        //        Instantiate(tile.tileObject, new Vector3(30,30,30), Quaternion.identity);
        //    }

        //}

        //Stopwatch stopwatch = new Stopwatch();
        //stopwatch.Start();
        //FloorSetup(mapX_width, mapY_height);
        //stopwatch.Stop();
        //TimeSpan ts = stopwatch.Elapsed;
        //int _ms = ts.Milliseconds; // Calculated 14ms for a 20x20 Floorsetup
        //Console.WriteLine("_ms: " + _ms); // No debug.log, maybe because of using diagnostics?


    }

    // TEST: Experiment using the Tile class:
    public void FloorSetup(int width, int height)
    {
        //->GameObject floorTile = tiles[0].tileObject;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //->GameObject _floorTile = Instantiate(floorTile, new Vector3(x, y, 0), Quaternion.identity);
                //->_floorTile.transform.SetParent(_floorHolder);
            }
        }
    }
}

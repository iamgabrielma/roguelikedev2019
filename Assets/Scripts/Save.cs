using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // this class can be serialized, which means you can turn it into a stream of bytes and save it to a file on disk.
public class Save
{
    // Is this serializable? No. Vector3 is an Unity class not a System class, I would need to use other datatypes to represent it. V3 can be a float[3]
    //public Vector3 playerPosition;
    public float[] playerPosition;

    public Save(GameObject player){

        playerPosition = new float[3];
        //playerPosition[0] = player.transform.position.x;
        playerPosition[0] = player.transform.position.x;
        playerPosition[1] = player.transform.position.y;
        playerPosition[2] = player.transform.position.z;

        Debug.Log("Player position saved at x:" + playerPosition[0] + " y:" + playerPosition[1] + " z:" + playerPosition[2]);
    }
}

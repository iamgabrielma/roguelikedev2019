using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // this class can be serialized, which means you can turn it into a stream of bytes and save it to a file on disk.
public class Save
{
    // Is this serializable? No. Vector3 is an Unity class not a System class, I would need to use other datatypes to represent it. V3 can be a float[3]
    //public Vector3 playerPosition;
    public float[] playerPosition;
    public int[] playerStats;

    public float[,] enemyPositions;

    public Save(GameObject player, GameObject[] enemies){

        /* 1 - Player */

        // Position
        playerPosition = new float[3];
        playerPosition[0] = player.transform.position.x;
        playerPosition[1] = player.transform.position.y;
        playerPosition[2] = player.transform.position.z;

        // Component: Fighter
        playerStats = new int[6];
        Fighter playerFighterStatsReference = player.GetComponent<Fighter>();
        playerStats[0] = playerFighterStatsReference.attack;
        playerStats[1] = playerFighterStatsReference.defense;
        playerStats[2] = playerFighterStatsReference.speed;
        playerStats[3] = playerFighterStatsReference.health;
        playerStats[4] = playerFighterStatsReference.energy;
        playerStats[5] = playerFighterStatsReference.oxygen;

        // Component: Inventory Manager

        // Component: Equipment Manager

        /* 2 - Enemies */
        // Fail, this seems to save only the last one, like is overwriting all of them to 0 except the last of the list.
        //for (int i = 0; i < enemies.Length; i++)
        //{
        //    enemyPositions = new float[enemies.Length,3]; // 1 slot per enemy + 3 slots for their single location
        //    // Position
        //    enemyPositions[i, 0] = enemies[i].transform.position.x;
        //    enemyPositions[i, 1] = enemies[i].transform.position.y;
        //    enemyPositions[i, 2] = enemies[i].transform.position.z;
        //}

        enemyPositions = new float[enemies.Length, 3];

        for (int i = 0; i < enemies.Length; i++)
        {
            enemyPositions[i,0] = enemies[i].transform.position.x;
            enemyPositions[i,1] = enemies[i].transform.position.y;
            enemyPositions[i,2] = enemies[i].transform.position.z;
        }
    }
}

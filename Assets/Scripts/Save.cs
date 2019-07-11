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
        enemyPositions = new float[enemies.Length, 4];

        for (int i = 0; i < enemies.Length; i++)
        {
            enemyPositions[i,0] = enemies[i].transform.position.x;
            enemyPositions[i,1] = enemies[i].transform.position.y;
            enemyPositions[i,2] = enemies[i].transform.position.z;

            if (enemies[i].GetComponent<Fighter>().isAgressive == true)
            {
                enemyPositions[i, 3] = 1;
            }
            else
            {
                enemyPositions[i, 3] = 0;
            }
        }
    }
}

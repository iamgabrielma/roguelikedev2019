using UnityEngine;
using System;
using System.Collections.Generic;

public class ItemGenerator: MonoBehaviour
{

    private int numberOfItemsPerLevel;
    public List<GameObject> listOfItems = new List<GameObject>();

    public void GenerateItems(List<Vector2> floorTiles) {

        numberOfItemsPerLevel = 5;

        GameObject itemPrefab = null;
        GameObject itemConsumableHealth = Resources.Load<GameObject>("Prefabs/ItemHealth");
        GameObject itemConsumableOxygen = Resources.Load<GameObject>("Prefabs/ItemOxygen");

        for (int i = 0; i < numberOfItemsPerLevel; i++)
        {
            // Random vector where we'll instantiate this
            int _randPos = UnityEngine.Random.Range(0, floorTiles.Count); 

            // Random Item effect our instance will have
            int _rand = UnityEngine.Random.Range(0, 2);
            switch (_rand)
            {
                case 0:
                    itemPrefab = itemConsumableHealth;
                    break;
                case 1:
                    itemPrefab = itemConsumableOxygen;
                    break;
                default:
                    itemPrefab = itemConsumableOxygen; // Defaults to Oxygen and not Health
                    break;
            }

            Instantiate(itemPrefab, new Vector3(floorTiles[_randPos].x + 0.5f, floorTiles[_randPos].y + 0.5f, 0), Quaternion.identity);

            // Add to list to keep track
            listOfItems.Add(itemPrefab);
        }

    }
}

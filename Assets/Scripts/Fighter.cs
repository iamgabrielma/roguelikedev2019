using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour
{

    // I guess this should be an interface so we can get different stats for different fighters ... but for now is all good and we'll set this via  Inspector or another generic script
    public int attack;
    public int defense;
    public int speed;
    public int health;
    public int energy;
    public int oxygen;

    public bool isAgressive;


    void Start()
    {
        // Base stats:
        attack = 10;
        defense = 10;
        speed = 1;
        health = 3;
        energy = 10;
        oxygen = 1000;

        isAgressive = false;

        // Temporary approach, we should move this to Interfaces
        if (gameObject.tag == "Player")
        {
            health = 10;

            //GameObject _FOVCollisionHolder = Resources.Load<GameObject>("Prefabs/_FOVCollisionHolder");
            //Instantiate(_FOVCollisionHolder, gameObject.transform.localPosition, Quaternion.identity);
            //_FOVCollisionHolder.transform.SetParent(gameObject.transform);
            /* ERROR: 
             * Setting the parent of a transform which resides in a Prefab Asset is disabled to prevent data corruption (GameObject: '_FOVCollisionHolder').
                UnityEngine.Transform:SetParent(Transform)
                */
        }
    }

}

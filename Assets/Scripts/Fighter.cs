using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour
{
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

        }
    }

}

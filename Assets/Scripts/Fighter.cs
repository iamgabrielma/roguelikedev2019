using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    public int attack;
    public int defense;
    public int speed;
    public int maxHealth;
    public int health;
    public int maxEnergy;
    public int energy;
    public int oxygen;
    public int xp;

    public bool isAgressive;


    void Start()
    {
        // Base stats:
        attack = 10;
        defense = 10;
        speed = 1;
        maxHealth = 3;
        health = maxHealth;
        maxEnergy = 10;
        energy = maxEnergy;
        oxygen = 1000;
        xp = 5; // xp an entity gives when dies

        isAgressive = false;

        if (gameObject.tag == "Player")
        {
            maxHealth = 10;
            health = maxHealth;
            xp = 0; // player xp

        }
    }

}

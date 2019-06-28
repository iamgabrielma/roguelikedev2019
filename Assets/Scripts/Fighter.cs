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

    public bool isAgressive;


    void Start()
    {
        attack = 10;
        defense = 10;
        speed = 1;
        health = 3;

        isAgressive = false;
    }

}

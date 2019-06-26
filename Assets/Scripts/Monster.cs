using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Entity
{
    //The default contructor for Entity will be called, this accepts 5 parameters, and we need to explicitely call the contructor and provide them
    private int _monsterAwareness;


    public Monster() : base(6, 6, "nameofmonster", new GameObject("wut is this"), new Vector3())//, _monstergoInstance, _monsterloc)
    {
    //public Monster(int newx, int newy, string newname, GameObject newob, Vector3 newv)
    //{
        _monsterAwareness = 10;
        speed = 2; // Changed from default 1
        //entityLocation.x += 0.5f;
        //entityLocation.y += 0.5f;
        GetMonsterData();
        //name = "Monster";
        entityName = "The Monster";
        entityGameObject = Resources.Load<GameObject>("Prefabs/EnemyRed"); // Doesn't work.

    }

    private void GetMonsterData()
    {
        Debug.Log("Monster created! --> " + "awar " + _monsterAwareness + "speed " + speed + "health " + health + "location " + entityLocation.ToString() + " , vs passes location x " + x + " , passed loc Y " + y);
        // entityLocation = 6.5, 6.5  which I assume is base(6,6) + reallocateEntity function which does entityLocation = reallocateEntity(aX, aY);
        // passed location x and y are also 6,6
        // Por alguna razon llama "monster" a un transform completamente vacio, pero luego genera un Enemy(clone) en la lista normal.
    }
}

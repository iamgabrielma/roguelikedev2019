using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAI : MonoBehaviour
{

    void Start()
    {

        CreateFOVCollisionArea();

    }

    void CreateFOVCollisionArea()
    {
        // PROBLEM: Whatever I do or how I create this, moves the player to 0 0 0 . Resolved by adding a rigidbody2d
        GameObject FOVCollisionHolder = Resources.Load<GameObject>("Prefabs/_FOVCollisionHolder");
        GameObject _FOVCollisionHolder = Instantiate(FOVCollisionHolder, gameObject.transform.position, Quaternion.identity);
        _FOVCollisionHolder.transform.SetParent(gameObject.transform); // Adds it to the Player
        _FOVCollisionHolder.transform.localPosition = new Vector3(0, 0, 0); // Resets it to the center to the player parent

    }
}

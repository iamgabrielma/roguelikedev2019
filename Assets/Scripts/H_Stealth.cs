using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Disable Entities visibility until these are within the Player FOVCollisionHolder
public class H_Stealth : MonoBehaviour
{

    public void Start()
    {
        gameObject.GetComponent<Renderer>().enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.tag == "FOVCollisionHolder")
        {
            Debug.Log("Enemy is visible");
            gameObject.GetComponent<Renderer>().enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "FOVCollisionHolder")
        {
            Debug.Log("Enemy is hidden");
            gameObject.GetComponent<Renderer>().enabled = false;
        }
    }
}

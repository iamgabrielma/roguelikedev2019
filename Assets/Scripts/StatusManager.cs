using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusManager : MonoBehaviour
{
    public int integrity;
    public int energy;
    public Text integrityText;
    public Text energyText;
    public GameObject playerReference;

    bool areComponentReferencesLinked;

    void Start()
    {
        areComponentReferencesLinked = false;
        playerReference = Engine.__player;

    }

    void Update() // TODO: I don't need this per frame, but reference just a tthe beginning, andthen the update every time a turn passes
    {
        if (playerReference == null)
        {   
            playerReference = GameObject.FindWithTag("Player");
        }
        if (playerReference.GetComponent<Fighter>() != null)
        {
            integrityText.text = "INTEGRITY " + integrity.ToString();
            energyText.text = "ENERGY " + energy.ToString();

        }

        GrabComponentReferences();
    }

    void GrabComponentReferences()
    {
        integrity = playerReference.GetComponent<Fighter>().health;
        energy = playerReference.GetComponent<Fighter>().energy;
        areComponentReferencesLinked = true;
    }
}
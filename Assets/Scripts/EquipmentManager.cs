using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour{

    // TODO: Add component to player object
    [SerializeField] private GameObject propulsion;
    [SerializeField] private GameObject armament;
    [SerializeField] private GameObject sensors;
    [SerializeField] private GameObject navigation;
    [SerializeField] private GameObject communication;
    //[SerializeField] private GameObject capacity;

    public GameObject Propulsion { get { return propulsion; } }
    public GameObject Armament { get { return armament; } }
    public GameObject Sensors { get { return sensors; } }
    public GameObject Navigation { get { return navigation; } }
    public GameObject Communication { get { return communication; } }

    private void Start()
    {
        // Temporary initial values. TODO: Get them via prefabs and random inventory
        propulsion = new GameObject("Diesel-Electric");
        armament = new GameObject("Compressed Air Torpedo");
        sensors = new GameObject("Passive");
        navigation = new GameObject("Inertial Guidance System");
        communication = new GameObject("VLF (Low Freq)");
    }

    private void EquipmentSwitch()
    {
        // Each time there's an equipment switch, we should update both the player stats and the UI
    }

    private void Update()
    {
        // If there an update, call a switcher function
    }

    public static void InventorySlotSwitcher(GameObject newItem) {

        // 1 - Item must be in inventory. We assure this by passing the newItem through the InventoryManager and UseItem()
        // 2 - newItem Must replace the oldItem in Equipment, and oldItem must replace newItem in Inventory. We're switching places
        // Resources.Load<GameObject>("Prefabs/Enemy");

    }

}


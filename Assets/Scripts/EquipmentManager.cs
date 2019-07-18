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

    private GameObject playerReference;

    private GameObject[] equippedItems;

    public static bool __updateEquipedGear;

    private void Start()
    {
        __updateEquipedGear = false;
        // Temporary initial values. TODO: Get them via prefabs and random inventory
        propulsion = new GameObject("Diesel-Electric");
        //armament = new GameObject("Compressed Air Torpedo");
        armament = new GameObject("--------");
        sensors = new GameObject("Passive");
        navigation = new GameObject("Inertial Guidance System");
        communication = new GameObject("VLF (Low Freq)");

        playerReference = Engine.__player;
        if (playerReference == null)
        {
            playerReference = GameObject.Find("Player");
        }

        equippedItems = new GameObject[5];
        equippedItems[0] = propulsion;
        equippedItems[1] = armament;
        equippedItems[2] = sensors;
        equippedItems[3] = navigation;
        equippedItems[4] = communication;

        for (int i = 0; i < equippedItems.Length; i++)
        {
            equippedItems[i].transform.SetParent(playerReference.transform); // Adds them to the Player
            equippedItems[i].transform.localPosition = new Vector3(0, 0, 0); // Resets it to the center to the player parent
        }

        CheckEquipped();
    }

    //public static GameObject GearSwitcher(GameObject oldItem, GameObject newItem) {

    //    equi
    //    return newItem;
    
    //}

    private void Update()
    {
        // If there an update, call a switcher function
        if (__updateEquipedGear)
        {
            //GameObject _newItem = GearSwitcher(armament, newItem);
            //EquipmentSwitch(_oldItem, _newItem);
            EquipmentSwitch();
            __updateEquipedGear = !__updateEquipedGear;
        }
    }

    private void CheckEquipped()
    {
        if (armament.name == "Compressed Air Torpedo")
        {
            playerReference.GetComponent<Fighter>().attack += 1;
        }

    }

    private void EquipmentSwitch()
    {
        //GameObject _oldItem = Engine.__player.gameObject.GetComponent<EquipmentManager>().armament;
        //GameObject _newItem = InventoryManager.__itemToBeEquiped;
        GameObject _newItem = new GameObject("NEW TEST OBJECT");
        armament = _newItem;
        StatusManager.__updateInventoryUI = true;
        // Each time there's an equipment switch, we should update both the player stats and the UI


    }



    public static void InventorySlotSwitcher(GameObject newItem) {

        // 1 - Item must be in inventory. We assure this by passing the newItem through the InventoryManager and UseItem()
        // 2 - newItem Must replace the oldItem in Equipment, and oldItem must replace newItem in Inventory. We're switching places
        // Resources.Load<GameObject>("Prefabs/Enemy");

    }

}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* Display inventory, player, and game info via UI */
public class StatusManager : MonoBehaviour
{
    public int integrity;
    public int energy;
    public int payload; // current payload
    public int capacity; // max capacity
    public Text integrityText;
    public Text energyText;
    public Text payloadCapacityText;
    //public Text capacitytext;
    public GameObject playerReference;
    public Canvas canvasReference;
    [SerializeField] private RectTransform _newItemUISLotReference; // sub-UI for Inventory

    // WIP: Items-Inventory TODO: Move this to local-scoped variables inside other function?
    [SerializeField] private GameObject[] itemsInInventory; 
    [SerializeField] private GameObject gearInInventory; // Testing armament only
    [SerializeField] private List<GameObject> itemsInVisibleUI = new List<GameObject>(); // we'll use this to keep track of instantiated objects in UI and remove them when used.
    [SerializeField] private List<GameObject> itemsInUi = new List<GameObject>();

    // Equipment-Inventory
    [SerializeField] private GameObject propulsion;
    [SerializeField] private GameObject armament;
    [SerializeField] private GameObject sensors;
    [SerializeField] private GameObject navigation;
    [SerializeField] private GameObject communication;
    public Text propulsionText;
    public Text armamentText;
    public Text sensorsText;
    public Text navigationText;
    public Text communicationText;

    bool areComponentReferencesLinked;
    public static bool __updateInventoryUI;
    public static bool __removeUsedItems;

    private int currentPlayerLevel;
    private int currentLevelDepth;
    public Text currentPlayerLevelText;
    public Text currentLevelDepthText;

    private GridGenerator _gridReference;

    void Start()
    {
        playerReference = Engine.__player;
        _gridReference = FindObjectOfType<GridGenerator>();
        areComponentReferencesLinked = false;
        __updateInventoryUI = false;
        __removeUsedItems = false;

        itemsInUi.Clear();


        if (canvasReference == null)
        {
            canvasReference = FindObjectOfType<Canvas>();
        }

    }

    void Update() // TODO: I don't need this per frame, but reference just a tthe beginning, andthen the update every time a turn passes
    {
        if (playerReference == null)
        {   
            playerReference = GameObject.FindWithTag("Player");
        }
        if (!areComponentReferencesLinked)
        {
            GrabComponentReferences();
        }
        if (playerReference.GetComponent<Fighter>() != null)
        {
            integrityText.text = "INTEGRITY " + playerReference.GetComponent<Fighter>().health.ToString() + "/" + playerReference.GetComponent<Fighter>().maxHealth.ToString();
            energyText.text = "ENERGY " + playerReference.GetComponent<Fighter>().energy.ToString() + "/" + playerReference.GetComponent<Fighter>().maxEnergy.ToString();
        }
        if (playerReference.GetComponent<InventoryManager>() != null)
        {
            payloadCapacityText.text = "----/ Payload Capacity /---- [" + payload.ToString() + "/" + capacity.ToString()+ "]";

        }
        if (playerReference.GetComponent<EquipmentManager>() != null)
        {
            propulsionText.text = propulsion.name;
            armamentText.text = armament.name;
            sensorsText.text = sensors.name;
            navigationText.text = navigation.name;
            communicationText.text = communication.name;
        }
        if (playerReference.GetComponent<Level>() != null)
        {
            currentPlayerLevel = playerReference.GetComponent<Level>().CurrentLevel;
            currentPlayerLevelText.text = "Level: " + currentPlayerLevel.ToString();
        }

        if (_gridReference != null)
        {
            currentLevelDepth = _gridReference.CurrentFloor;
            currentLevelDepthText.text = "Depth: " + currentLevelDepth.ToString();
        }

        if (__updateInventoryUI)
        {
            GrabNewInventoryReferences();

            __updateInventoryUI = !__updateInventoryUI;
        }

        if (__removeUsedItems)
        {
            RemoveItemFromUI(0); // TESTING
            __removeUsedItems = !__removeUsedItems;
        }
    }

    // Other components shouldn't reference the UI, but the UI just grab the info needed from other components:
    void GrabComponentReferences()
    {
        // Grab STATUS data
        integrity = playerReference.GetComponent<Fighter>().health;
        energy = playerReference.GetComponent<Fighter>().energy;
        capacity = playerReference.GetComponent<InventoryManager>().Capacity;
        payload = playerReference.GetComponent<InventoryManager>().Payload;

        // Grab ITEM data
        itemsInInventory = playerReference.GetComponent<InventoryManager>().ItemsInInventory;
        gearInInventory = playerReference.GetComponent<EquipmentManager>().Armament; // Testing armament only

        // Grab EQUIPMENT data
        propulsion = playerReference.GetComponent<EquipmentManager>().Propulsion;
        armament = playerReference.GetComponent<EquipmentManager>().Armament;
        sensors = playerReference.GetComponent<EquipmentManager>().Sensors;
        navigation = playerReference.GetComponent<EquipmentManager>().Navigation;
        communication = playerReference.GetComponent<EquipmentManager>().Communication; // Testing armament only

        areComponentReferencesLinked = true;
    }

    void GrabNewInventoryReferences()
    {
        // Grab UPDATED ITEM data
        itemsInInventory = playerReference.GetComponent<InventoryManager>().ItemsInInventory;
        gearInInventory = playerReference.GetComponent<EquipmentManager>().Armament;



        GameObject inventoryUiSlot = Resources.Load<GameObject>("Prefabs/InventoryUiSlot");
        for (int i = 0; i < itemsInInventory.Length; i++)
        {

            // Adding the item to the inventory has to happen somewhere else, and before this update UI method is called
            if (itemsInInventory[i] != null)
            {
                // Create new UI element that contains this info:
                GameObject _newItemUISLot = Instantiate(inventoryUiSlot, new Vector3(0, 0, 0), Quaternion.identity); // Create UI versions from Items in the inventory
                _newItemUISLot.name = i + " " + itemsInInventory[i].name; // Set UI settings like the original item, adding the "i" to the name to avoid "(clone)" and also works as key input selection
                itemsInUi.Add(_newItemUISLot); // Add them to UI items list
            }

            // Esto no funciona aquí porque el item ya no es parte de itemsInInventory
            //else // For items that are null (existed, but have been [U]sed)
            //{
            //    itemsInUi.Remove(itemsInInventory[i]);
            //}
        }


        //foreach (var item in itemsInUi)
        for (int i = 0; i < itemsInUi.Count; i++)
        {
            itemsInUi[i].transform.SetParent(_newItemUISLotReference.transform); // Set each UI item to be a child of uiStatusPayload transform, so appear under PayLoad Capacity
            itemsInUi[i].transform.localPosition = new Vector3(-70,-226 + (i * -20),0); // This kinda works but is still hardcoded coordinates. i*20 should add correct space from original transform
            // todo BUG: The current tooltip object attached to this prefab won't work, need to attach the one is being used within the scene.
            // 1- Get its type, modify the scriptable object. NOPE, THIS SHOULD ONLY DISPLAY
            // 2- Get its text, display it
            itemsInUi[i].GetComponentInChildren<Text>().text = itemsInUi[i].name;
            itemsInVisibleUI.Add(itemsInUi[i]);
            Debug.Log("itemsInInventory[i].name " + itemsInInventory[i].name);
            Debug.Log("itemsInUi[i].name " + itemsInUi[i].name);
            Debug.Log("itemsInVisibleUI[i].name " + itemsInVisibleUI[i].name);

        }
        itemsInUi.Clear(); // Here we cleat the itemsInUi list so next item will regenerate the list from zero, otherwise will replicate and sum to the previous ones.

        /* WIP updating GEAR */
        armamentText.text = gearInInventory.name;


    }

    // _numericKeycode is hardcoded to 0 for testing, improve.
    public void RemoveItemFromUI(int _numericKeycode)
    {

        if (_newItemUISLotReference.transform.GetChild(_numericKeycode).gameObject != null)
        {
            Destroy(_newItemUISLotReference.transform.GetChild(_numericKeycode).gameObject);
        }


        Debug.Log("RemoveItemFromUI");
    }
}
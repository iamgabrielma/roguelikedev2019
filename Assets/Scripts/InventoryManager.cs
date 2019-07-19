using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{

    [SerializeField] private int payload;
    [SerializeField] private int capacity;
    [SerializeField] private int capacityModifier; // We want to be able to reduce/increase capacity from a base number. We cannot modify the array later on, but we can limit it initially to a submax that is not the total length allowed
    // TODO: Figure out if is better with Lists or Array:
    List<GameObject> itemsInInventoryList = new List<GameObject>();
    public GameObject[] itemsInInventory;
    //private GameObject _placeholderGO;


    [SerializeField] private GameObject item0;

    // Keeping them private but accessible to other modules (like UI) via getters
    public int Capacity { get { return capacity; } }
    public int Payload { get { return payload; } }
    public GameObject[] ItemsInInventory { get { return itemsInInventory; } }

    public static GameObject __itemToBeEquiped;

    /* TEST: NEW ENHANCED INVENTORY */
    public EnhancedInventory enhancedPlayerInventory; // = new EnhancedInventory();

    private void Start()
    {
        __itemToBeEquiped = null;
        capacity = 10;
        payload = 0;
        //_placeholderGO = new GameObject();

        // Initialize the inventory to 10 empty slots
        itemsInInventory = new GameObject[capacity];
        for (int i = 0; i < itemsInInventory.Length; i++)
        {
            itemsInInventory[i] = null; // La otra opción sería poner un placeholder GO en vez de null.
            //itemsInInventory[i] = _placeholderGO; no funcionará luego para manejarlo correctamente.
        }

    }

    public void InputHandlerAndUseItem(GameObject entity, int _numericKeycode) {

        //Debug.Log(entity); // player
        Debug.Log("Method linked, using item located on " + _numericKeycode);
        //Debug.Log(itemsInInventory[_numericKeycode].tag);
        //Debug.Log(itemsInInventory[_numericKeycode].name);
        Debug.Log(itemsInInventory[_numericKeycode].gameObject.GetType().ToString()); // GameObject

        Debug.Log(itemsInInventoryList[_numericKeycode].gameObject.GetComponent<ItemButtonGo>().item.typeOfItem.ToString()); // none
        Debug.Log(itemsInInventoryList[_numericKeycode].gameObject.GetComponent<ItemButtonGo>().item.Test_ItemType); // Armament

        if (itemsInInventoryList[_numericKeycode].gameObject.GetComponent<ItemButtonGo>().item.Test_ItemType == "Armament")
        {
            EquipItem(itemsInInventoryList[_numericKeycode].gameObject);
            //Entity.EquipGear(itemsInInventoryList[_numericKeycode].gameObject);
        }



        itemsInInventoryList.Remove(itemsInInventoryList[_numericKeycode]); // remove it from the list item.name = torpedosomething, so is reachable
        itemsInInventory[_numericKeycode] = null; // Set the item in array to null --> Works: This is the one we're using, not the list above.
        // Use item functionality
        StatusManager.__updateInventoryUI = true; // update UI. Doesn't seem to work.
        StatusManager.__removeUsedItems = true;

    }

    public void EquipItem(GameObject itemToEquip)
    {
        // WIP
        Debug.Log("Equiping item");
        __itemToBeEquiped = itemToEquip;
        //GameObject playerArmamentRef = Engine.__player.gameObject.GetComponent<EquipmentManager>().Armament;
        //playerArmamentRef = itemToEquip;
        //Entity.EquipGear(playerRef,itemToEquip);
        //EquipmentManager.GearSwitcher(playerArmamentRef, itemToEquip);
        EquipmentManager.__updateEquipedGear = true;
    }

    public void AddItem(GameObject item, int itemCount) 
    {
        /* TEST: NEW ENHANCED INVENTORY */
        //enhancedPlayerInventory.Test_Add();
        enhancedPlayerInventory.Test_Add_Different_Items(item);

        // If we have capacity, add it to our list.
        // ARRAY APPROACH: Get the first element that is null (empty inventory slot) and add the item
        for (int i = 0; i < itemsInInventory.Length; i++)
        {
            if (itemsInInventory[i] == null)
            {
                itemsInInventory[i] = item;
                StatusManager.__updateInventoryUI = true;
                break;
            }
        }

        // LIST APPROACH
        if (itemsInInventoryList.Count <= capacity)
        {
            itemsInInventoryList.Add(item);
            payload = itemsInInventoryList.Count();
        }

        else
        {
            // If we don't, do not add it and do not destroy the item in game
            MessageLogManager.Instance.AddToQueue(item.name + " cannot be picked up. Storage limit.");
        }

    }
    public void RemoveItem(Item itemName, int itemCount) 
    { 

        // if consumed, remove it

        // if droped from inventory, remove it and place it in scenario around the player

        // (?) if destroyed, pop confirm screen and destroy it (Maybe is good that we cannot destroy, just drop, so enemies can pick them up)
    }
    public void UseItem(GameObject item)
    {
        // Should assign one of the 26 keyboard keys to each item dynamically, increase capacity with caps maybe?
        // Consumable? Use, non-consumable?Equip
        EquipmentManager.InventorySlotSwitcher(item);
    }

    public string ItemType()
    {

        // Checks how many items of the same type we have, like oxygen modules.
        return "not implemented yet";
    }

    public int CheckItemCOunt()
    {

        return 0;
    }

    public GameObject OutputItem(int _index)
    {



        GameObject _testing_go = new GameObject("Testing Inventory item");
        return _testing_go;

    }
    /* In the future we also want to save this between games:
     * void SaveToDisk( string fileName);
     * void LoadFromDisk( string fileName);   
     */

}

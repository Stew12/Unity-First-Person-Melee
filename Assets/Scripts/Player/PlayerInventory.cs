using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.Mathematics;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public GameObject weaponStock;

    private GameObject selectedItem;
    private GameObject selectedItemGObj;
    [SerializeField] private GameObject selectedInvWindow;
    [SerializeField] private GameObject invTabs;

    [SerializeField] private PlayerController player;

    [HideInInspector] public GameObject inventoryInterface;

    [SerializeField] private GameObject weaponInvWindow;
    [SerializeField] private GameObject armourInvWindow;
    [SerializeField] private GameObject spellInvWindow;
    [SerializeField] private GameObject consumableInvWindow;

    public List<GameObject> weaponsList = new List<GameObject>();

    public List<GameObject> itemsList = new List<GameObject>();

    public List<GameObject> spellsList = new List<GameObject>();

    private List<int> itemValuesList = new List<int>();

    public GameObject[] hotKeyList = new GameObject[9];


    //Starts from 1 rather than 0
    public int weaponInvIndex = 1;
    public int itemInvIndex = 1;
    public int spellInvIndex = 1;

    void Awake()
    {
        inventoryInterface = selectedInvWindow.gameObject;
        inventoryInterface.SetActive(false);

        invTabs.SetActive(false);

        LoadHotkeys();

        LoadInventory();
    }

    private void LoadInventory()
    {

    }

    public void OpenInventoryTab(ItemTypeUI itemTypeUI)
    {
        Debug.Log("SSSS");
        weaponInvWindow.SetActive(false);
        armourInvWindow.SetActive(false);
        spellInvWindow.SetActive(false);
        consumableInvWindow.SetActive(false);

        switch (itemTypeUI)
        {
            case ItemTypeUI.WEAPON:
                selectedInvWindow = weaponInvWindow;
            break;

            case ItemTypeUI.ARMOUR:
                selectedInvWindow = armourInvWindow;
            break;

            case ItemTypeUI.SPELL:
                selectedInvWindow = spellInvWindow;
            break;

            case ItemTypeUI.CONSUMABLE:
                selectedInvWindow = consumableInvWindow;
            break;

            case ItemTypeUI.KEYITEM:
                selectedInvWindow = consumableInvWindow;
            break;

            case ItemTypeUI.ATTACKITEM:
                selectedInvWindow = consumableInvWindow;
            break;

            default:
                
            break;
        }

        selectedInvWindow.SetActive(true);
        inventoryInterface = selectedInvWindow;
    }

    public void InventoryToggle()
    {
        if (!inventoryInterface.activeInHierarchy)
        {
            if (!player.waiting)
            {
                //Show inventory
                inventoryInterface.SetActive(true);
                invTabs.SetActive(true);

                player.waiting = true;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
        else
        {
            //Hide inventory
            inventoryInterface.SetActive(false);
            invTabs.SetActive(false);

            player.waiting = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            player.attackPowerBuilding = true;
        }

    }

    public void ItemIsSelected(GameObject item, GameObject itemGObj)
    {
        selectedItem = item;
        selectedItemGObj = itemGObj;

        foreach (Transform icon in selectedInvWindow.transform)
        {
            if (icon.gameObject != selectedItemGObj)
            {
                icon.gameObject.GetComponent<OnInventoryIconClicked>().Selected(false);
            }
            else
            {
                icon.gameObject.GetComponent<OnInventoryIconClicked>().Selected(true);
            }
        }
    }

    private void LoadHotkeys()
    {
        //Can't Switch weapons while attacking!
        
            if (player.equippedWeapon.GetComponent<HotKey>().hotKey > 0 && player.equippedWeapon.GetComponent<HotKey>().hotKey < 10)
            {
                hotKeyList[player.equippedWeapon.GetComponent<HotKey>().hotKey - 1] = player.equippedWeapon;
            }

            foreach (Transform weapon in weaponStock.transform)
            {
                if (weapon.gameObject.GetComponent<HotKey>().hotKey > 0 && weapon.GetComponent<HotKey>().hotKey < 10)
                {
                    hotKeyList[weapon.gameObject.GetComponent<HotKey>().hotKey - 1] = weapon.gameObject;
                    //weapon.GetComponent<HotKey>().originalItemIndex = i;
                    //Debug.Log("DU: " + i);
                }

                //i++;
            }

        
    }

    public void HotKeyedItem(int index, bool weaponSheathed)
    {
        //TODO: make this for all items not just weapons, based on the GameObject type/tag
        if (hotKeyList[index - 1] != null)
        {
            
            //WEAPON
            if (!weaponSheathed && !player.attacking)
            {
                //Get values of initial weapon
                GameObject weaponParent = player.equippedWeapon.transform.parent.gameObject;
                Vector3 weaponPos = player.equippedWeapon.transform.position;
                
                //Send initial weapon back to the Weapon Stock and disable it
                player.equippedWeapon.transform.parent = weaponStock.transform;
                player.equippedWeapon.SetActive(false);

                //Look through the weapon stock for a weapon matching the hotKey
                GameObject newWeapon = hotKeyList[index - 1];
                newWeapon.SetActive(true);
                newWeapon.transform.SetParent(weaponParent.transform);
                //newWeapon.transform.position = weaponPos;
                newWeapon.transform.position = new Vector3(0,0,0);      

                player.equippedWeapon = newWeapon;

                //Set animations for new weapon
                player.animator = player.equippedWeapon.GetComponent<Animator>();
                player.GetComponent<PlayerAnimation>().WeaponAnimationChange(player.equippedWeapon.GetComponent<PlayerWeaponValues>().weaponClass, player);
                player.ResetAttack();
            }

            //SPELL

            //ITEM

        }
    }


}

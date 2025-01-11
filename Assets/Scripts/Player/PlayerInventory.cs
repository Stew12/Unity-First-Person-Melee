using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public GameObject weaponStock;

    [SerializeField] private PlayerController player;

    [HideInInspector] public GameObject inventoryInterface;

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
        inventoryInterface = transform.GetChild(0).gameObject;
        inventoryInterface.SetActive(false);

        LoadHotkeys();
    }

    public void InventoryToggle()
    {
        if (!inventoryInterface.activeInHierarchy)
        {
            //Show inventory
            inventoryInterface.SetActive(true);
            player.waiting = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            //Hide inventory
            inventoryInterface.SetActive(false);
            player.waiting = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

    }

    void LoadHotkeys()
    {
        //int i = 0;

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

    public void HotKeyedItem(int index)
    {
        //TODO: make this for all items not just weapons, based on the GameObject type/tag
        if (hotKeyList[index - 1] != null)
        {
            //Get values of initial weapon
            GameObject weaponParent = player.equippedWeapon.transform.parent.gameObject;
            Vector3 weaponPos = player.equippedWeapon.transform.position;

            //Save durability to the intial weapon
            //weaponsList[hotKeyList[index - 1].GetComponent<HotKey>().originalItemIndex].GetComponent<PlayerWeaponValues>().currentWeaponDurability = player.equippedWeapon.GetComponent<PlayerWeaponValues>().currentWeaponDurability;
            //itemValuesList[player.equippedWeapon.GetComponent<HotKey>().hotKey - 1] = player.equippedWeapon.GetComponent<PlayerWeaponValues>().currentWeaponDurability;
            //Debug.Log("DU: " + weaponsList[hotKeyList[index - 1].GetComponent<HotKey>().originalItemIndex].GetComponent<PlayerWeaponValues>().currentWeaponDurability);

            //Destroy(player.equippedWeapon.gameObject);
            
            //Send initial weapon back to the Weapon Stock and disable it
            player.equippedWeapon.transform.parent = weaponStock.transform;
            player.equippedWeapon.SetActive(false);

            //Look through the weapon stock for a weapon matching the hotKey
            
            
            GameObject newWeapon = hotKeyList[index - 1];
            newWeapon.SetActive(true);
            newWeapon.transform.SetParent(weaponParent.transform);
            //newWeapon.transform.position = weaponPos;
            newWeapon.transform.position = new Vector3(0,0,0);
    
            Debug.Log(weaponPos);       

            //if (itemValuesList[index - 1])
            //newWeapon.GetComponent<PlayerWeaponValues>().currentWeaponDurability = itemValuesList[index - 1];
            
            //weaponsList[newWeapon.GetComponent<HotKey>().originalItemIndex].GetComponent<PlayerWeaponValues>().currentWeaponDurability;

            player.equippedWeapon = newWeapon;

            //Set animations for new weapon
            player.animator = player.equippedWeapon.GetComponent<Animator>();
            player.GetComponent<PlayerAnimation>().WeaponAnimationChange(player.equippedWeapon.GetComponent<PlayerWeaponValues>().weaponClass, player);

        }
    }


}

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    public GameObject weaponStock;
    private AudioSource invAudioSource;

    [SerializeField] private Image HUDSpell;

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

    public Dictionary<GameObject, int> consumablesList = new Dictionary<GameObject, int>();
    public List<GameObject> DEBUGConsumablesList = new List<GameObject>();
    public List<int> DEBUGConsumablesQuantities = new List<int>();

    public List<GameObject> spellsList = new List<GameObject>();

    //private List<int> itemValuesList = new List<int>();

    public GameObject[] hotKeyList = new GameObject[9];


    //Starts from 1 rather than 0
    public int weaponInvIndex = 1;
    public int itemInvIndex = 1;
    public int spellInvIndex = 1;

    void Awake()
    {
        inventoryInterface = selectedInvWindow.gameObject;
        inventoryInterface.SetActive(false);

        invAudioSource = GetComponent<AudioSource>();

        invTabs.SetActive(false);

        LoadHotkeys();

        LoadInventory();
    }

    private void LoadInventory()
    {

    }

    public void InventoryTab(ItemTypeUI itemTypeUI)
    {
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
        // Make item not null if item selected
        selectedItem = item;
        selectedItemGObj = itemGObj;

        // If item STILL null
        //if (item == null && itemGObj == null)
        //{
            if (itemGObj.GetComponent<OnInventoryIconClicked>().selected)
            {
                switch (itemGObj.GetComponent<OnInventoryIconClicked>().itemTypeUI)
                {
                    case ItemTypeUI.WEAPON:
                        EquipWeapon(item);
                    break;

                    case ItemTypeUI.ARMOUR:
                        
                    break;

                    case ItemTypeUI.SPELL:
                        EquipSpell(itemGObj.GetComponent<OnInventoryIconClicked>().dragonSpells);
                    break;

                    case ItemTypeUI.CONSUMABLE:
                        itemGObj.GetComponent<OnInventoryIconClicked>().consumable.GetComponent<ConsumableItem>().UseConsumable(player.gameObject.GetComponent<PlayerValues>());
                    break;

                    case ItemTypeUI.KEYITEM:

                break;

                    case ItemTypeUI.ATTACKITEM:
            
                    break;

                    default:
                        
                    break;
                }
            }
            else
            {
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
        //}
        
    }

    public void AddToInventory(GameObject itemFound)
    {
        GameObject savedItem = new GameObject();
        savedItem.AddComponent<InteractableItem>();
        savedItem.GetComponent<InteractableItem>().interactedItemType = itemFound.GetComponent<InteractableItem>().interactedItemType;
        savedItem.name = itemFound.GetComponent<InteractableItem>().itemName;

        Destroy(itemFound);

        switch (savedItem.GetComponent<InteractableItem>().interactedItemType)
        {
            case ItemTypeUI.WEAPON:
                
            break;

            case ItemTypeUI.ARMOUR:
                
            break;

            case ItemTypeUI.SPELL:
                
            break;

            case ItemTypeUI.CONSUMABLE:
               consumablesList.Add(savedItem, 1);
               DEBUGConsumablesList.Add(savedItem);
               DEBUGConsumablesQuantities.Add(1);


            break;
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
        if (!player.attacking)
        {
            //TODO: make this for all items not just weapons, based on the GameObject type/tag
            //Hot keys might be a stretch feature as using the inventory is surprsingly smooth
            if (hotKeyList[index - 1] != null)
            {
                
                //WEAPON
                //TODO: If is a weapon
                EquipWeapon(hotKeyList[index - 1]);

                //SPELL

                //ITEM

            }
        }
    }

    private void EquipWeapon(GameObject newWeapon)
    {
        if (!player.weaponSheathed)
            {
                //Get values of initial weapon
                GameObject weaponParent = player.equippedWeapon.transform.parent.gameObject;
                Vector3 weaponPos = player.equippedWeapon.transform.position;
                
                //Send initial weapon back to the Weapon Stock and disable it
                player.equippedWeapon.transform.parent = weaponStock.transform;
                player.equippedWeapon.SetActive(false);

                //Look through the weapon stock for a weapon matching the hotKey
                newWeapon.SetActive(true);
                newWeapon.transform.SetParent(weaponParent.transform);
                //newWeapon.transform.position = weaponPos;
                newWeapon.transform.position = new Vector3(0,0,0);      

                player.equippedWeapon = newWeapon;

                //Set animations for new weapon
                player.animator = player.equippedWeapon.GetComponent<Animator>();
                player.GetComponent<PlayerAnimation>().WeaponAnimationChange(player.equippedWeapon.GetComponent<PlayerWeaponValues>().weaponClass, player);
                player.ResetAttack();

                //Play sound of unsheathing
                invAudioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
                invAudioSource.PlayOneShot(newWeapon.GetComponent<PlayerWeaponValues>().unsheatheSound);
            }
    }

    private void EquipSpell(DragonSpells newSpell)
    {
        HUDSpell.sprite = selectedItemGObj.transform.GetChild(0).GetComponent<Image>().sprite;

        player.dragonSpellSelected = newSpell;
    }

}

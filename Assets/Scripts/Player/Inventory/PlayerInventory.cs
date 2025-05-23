using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    [Header("Items and Tabs")]
    private GameObject selectedItem;
    private GameObject selectedItemGObj;
    [SerializeField] private GameObject selectedInvWindow;
    [SerializeField] private GameObject invTabs;
    [HideInInspector] public GameObject inventoryInterface;

    [Header("Inventory Windows")]
    [SerializeField] private GameObject weaponInvWindow;
    [SerializeField] private GameObject armourInvWindow;
    [SerializeField] private GameObject spellInvWindow;
    [SerializeField] private GameObject consumableInvWindow;

    [Header("Inventory Lists")]
    [SerializeField] private int weaponInventoryLength = 15;
    [SerializeField] private int armourInventoryLength = 15;
    [SerializeField] private int spellInventoryLength = 15;
    [SerializeField] private int itemInventoryLength = 15;
    public GameObject[] weaponsList;
    public GameObject[] armourList;
     public GameObject[] spellsList;
    public GameObject[] consumablesList;
    [SerializeField] private GameObject[] UIIconsInInventory;

    public GameObject[] hotKeyList = new GameObject[9];

    [Header("Indices")]
    //Starts from 1 rather than 0
    public int weaponInvIndex = 1;
    public int itemInvIndex = 1;
    public int spellInvIndex = 1;

    [Header("Other Variables")]
    public GameObject weaponStock;
    private AudioSource invAudioSource;
    [SerializeField] private Image HUDSpell;
    private int inventoryCols = 5;
    [SerializeField] private PlayerController player;


    void Awake()
    {
        inventoryInterface = selectedInvWindow.gameObject;
        inventoryInterface.SetActive(false);

        invAudioSource = GetComponent<AudioSource>();

        invTabs.SetActive(false);

        weaponsList = new GameObject[weaponInventoryLength];
        armourList = new GameObject[armourInventoryLength];
        spellsList = new GameObject[spellInventoryLength];
        consumablesList = new GameObject[itemInventoryLength];
        UIIconsInInventory = new GameObject[weaponInventoryLength];

        weaponsList[0] = player.startingWeaponPickup;

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


        if (itemGObj.GetComponent<OnInventoryIconClicked>().selected)
        {
            switch (itemGObj.GetComponent<OnInventoryIconClicked>().itemTypeUI)
            {
                case ItemTypeUI.WEAPON:

                    foreach (Transform weapon in weaponStock.transform)
                    {
                        if (weapon.gameObject.GetComponent<PlayerWeaponValues>().weaponID == itemGObj.GetComponent<OnInventoryIconClicked>().ITEM.GetComponent<InteractableItem>().itemName)
                        {
                            EquipWeapon(weapon.gameObject);
                        }
                    }

                    
                break;

                case ItemTypeUI.ARMOUR:
                        
                break;

                case ItemTypeUI.SPELL:
                    EquipSpell(itemGObj.GetComponent<OnInventoryIconClicked>().dragonSpells);
                break;

                case ItemTypeUI.CONSUMABLE:
                    itemGObj.GetComponent<OnInventoryIconClicked>().consumable.GetComponent<ConsumableItem>().UseConsumable(player.gameObject.GetComponent<PlayerValues>(), this, item);
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
                        Debug.Log(icon.gameObject);
                        icon.gameObject.GetComponent<OnInventoryIconClicked>().Selected(true);
                    }
                }
            }
        
    }

    public void AddToInventory(GameObject itemFound)
    {
        GameObject savedItem = new GameObject();
        
        //savedItem = itemFound;

        savedItem.AddComponent<InteractableItem>();
        
        savedItem.GetComponent<InteractableItem>().itemName = itemFound.GetComponent<InteractableItem>().itemName;
        savedItem.GetComponent<InteractableItem>().interactedItemType = itemFound.GetComponent<InteractableItem>().interactedItemType;
        savedItem.GetComponent<InteractableItem>().UIIcon = itemFound.GetComponent<InteractableItem>().UIIcon; 

        GameObject invIcon;
        GameObject weaponInStock;

        switch (savedItem.GetComponent<InteractableItem>().interactedItemType)
        {
            case ItemTypeUI.WEAPON:
                savedItem.AddComponent<WeaponPickup>();
                savedItem.GetComponent<WeaponPickup>().weaponHeldObject = itemFound.GetComponent<WeaponPickup>().weaponHeldObject;

                Vector3 weaponSpawnPos = savedItem.GetComponent<WeaponPickup>().weaponHeldObject.transform.position;
                Quaternion weaponSpawnRot = savedItem.GetComponent<WeaponPickup>().weaponHeldObject.transform.rotation;
                //Debug.Log(weaponSpawnPos);

                //Add icon to UI
                invIcon = Instantiate(savedItem.GetComponent<InteractableItem>().UIIcon);
                weaponInStock = Instantiate(savedItem.GetComponent<WeaponPickup>().weaponHeldObject);
                
                string[] weaponNames = new string[weaponInventoryLength];
                for (int j = 0; j < weaponsList.Length - 1; j++)
                {
                    if (weaponsList[j] != null)
                        weaponNames[j] = weaponsList[j].GetComponent<InteractableItem>().itemName;
                }

                //New Weapon, add
                for (int i = 0; i < weaponsList.Length - 1; i++)
                {
                    if (weaponsList[i] == null)
                    {
                        int index = 0;

                        // Create a unique weapon name
                        while (weaponNames.Contains(savedItem.GetComponent<InteractableItem>().itemName))
                        {
                            index++;
                            savedItem.GetComponent<InteractableItem>().itemName += index.ToString();
                        }

                        weaponsList[i] = savedItem;

                        weaponInStock.transform.SetParent(weaponStock.transform);
                        weaponInStock.transform.localPosition = weaponSpawnPos;
                        weaponInStock.transform.localRotation = weaponSpawnRot;
                        
                        weaponInStock.GetComponent<PlayerWeaponValues>().weaponID = savedItem.GetComponent<InteractableItem>().itemName;
                        weaponInStock.SetActive(false);
                        

                        invIcon.transform.parent = weaponInvWindow.transform;
                        invIcon.GetComponent<OnInventoryIconClicked>().ItemSetup();
                        invIcon.GetComponent<OnInventoryIconClicked>().ITEM = savedItem;
                        SetInvIconUILocation(invIcon);
                        
                        break;
                    }
                }

            break;

            case ItemTypeUI.ARMOUR:
                
            break;

            case ItemTypeUI.SPELL:
                
            break;

            case ItemTypeUI.CONSUMABLE:
                
                savedItem.AddComponent<ConsumableItem>();
                savedItem.GetComponent<ConsumableItem>().itemQuantity = itemFound.GetComponent<ConsumableItem>().itemQuantity;
                
                bool itemExists = false;
                for (int i = 0; i < consumablesList.Length - 1; i++)
                {
                    //Debug.Log("NAMEINLIST: " + consumablesList[i].GetComponent<InteractableItem>().itemName + ", NAME OF PICKED UP ITEM: " + savedItem.GetComponent<InteractableItem>().itemName);
                    
                    if (consumablesList[i] != null)
                    {
                        if (consumablesList[i].GetComponent<InteractableItem>().itemName == savedItem.GetComponent<InteractableItem>().itemName)
                        {
                            //Dupicate item, stack
                            itemExists = true;

                            consumablesList[i].GetComponent<ConsumableItem>().itemQuantity++;
                            
                            if (UIIconsInInventory[i].GetComponent<OnInventoryIconClicked>().quantityText != null)
                            {
                                UIIconsInInventory[i].GetComponent<OnInventoryIconClicked>().quantityText.text = consumablesList[i].GetComponent<ConsumableItem>().itemQuantity.ToString();
                                Debug.Log(consumablesList[i].GetComponent<ConsumableItem>().itemQuantity.ToString());
                            }
                            break;
                        }
                    }
                }

                if (!itemExists)
                {
                    //Add icon to UI
                    invIcon = Instantiate(savedItem.GetComponent<InteractableItem>().UIIcon);
                    
                    //savedItem.GetComponent<InteractableItem>().UIIcon = invIcon;
                    
                    //New Item, add
                    for (int i = 0; i < consumablesList.Length - 1; i++)
                    {
                        if (consumablesList[i] == null)
                        {
                            consumablesList[i] = savedItem;
                            UIIconsInInventory[i] = invIcon;

                            invIcon.transform.parent = consumableInvWindow.transform;

                            consumablesList[i].GetComponent<ConsumableItem>().itemQuantity = 1;
                            
                            if (consumablesList[i].GetComponent<InteractableItem>().UIIcon.GetComponent<OnInventoryIconClicked>().quantityText != null)
                            {
                                consumablesList[i].GetComponent<InteractableItem>().UIIcon.GetComponent<OnInventoryIconClicked>().quantityText.text = 1.ToString();
                            }
                            
                            invIcon.GetComponent<OnInventoryIconClicked>().ItemSetup();
                            invIcon.GetComponent<OnInventoryIconClicked>().ITEM = savedItem;
                            SetInvIconUILocation(invIcon);
                            
                            break;
                        }
                    }
                }

            break;
        }

        Destroy(itemFound);
    }

    public void DecreaseOrRemoveConsumable(GameObject dorItem)
    {
        for (int i = 0; i < consumablesList.Length - 1; i++)
        {
            //Find Item
            if (consumablesList[i] != null)
            {
                //Debug.Log(consumablesList[i].GetComponent<InteractableItem>().UIIcon.GetComponent<OnInventoryIconClicked>().consumable.name + ", " + dorItem.GetComponent<InteractableItem>().itemName);
                if (consumablesList[i].GetComponent<InteractableItem>().UIIcon.GetComponent<OnInventoryIconClicked>().consumable.name == dorItem.GetComponent<InteractableItem>().itemName)
                {
                    consumablesList[i].GetComponent<ConsumableItem>().itemQuantity--;

                    if (consumablesList[i].GetComponent<ConsumableItem>().itemQuantity > 0)
                    {
                        //Item has only had its quantity decreased. Update inventory label
                        UIIconsInInventory[i].GetComponent<OnInventoryIconClicked>().quantityText.text = consumablesList[i].GetComponent<ConsumableItem>().itemQuantity.ToString();
                    }
                    else
                    {
                        //Remove item from inventory
                        Destroy(UIIconsInInventory[i].gameObject);

                        consumablesList[i] = null;
                        UIIconsInInventory[i] = null;
                    }
                }
            }
        }
    }

    private void SetInvIconUILocation(GameObject IIU)
    {
        GameObject[] selectedInvList = null;

        switch (IIU.GetComponent<OnInventoryIconClicked>().itemTypeUI)
        {
            case ItemTypeUI.WEAPON:
                selectedInvList = weaponsList;
            break;

            case ItemTypeUI.ARMOUR:
                selectedInvList = armourList;
            break;

            case ItemTypeUI.CONSUMABLE:
                selectedInvList = consumablesList;

            break;
        }

        // Get UI position
        float invx = 0;
        float invy = 0;

        float xSpacing = 106.2f;
        float ySpacing = 100; 

        float startingx = -209.8f;

        Debug.Log(selectedInvList[0]);

        for (int i = 0; i < selectedInvList.Length - 1; i++)
        {
            int listPos = i + 1;

            if (selectedInvList[i] != null)
            {
                //Debug.Log("UI ICON: " + selectedInvList[i].GetComponent<InteractableItem>().UIIcon.name + ", IIU: " + IIU.name);
                if (selectedInvList[i].GetComponent<InteractableItem>().itemName == IIU.GetComponent<OnInventoryIconClicked>().ITEM.GetComponent<InteractableItem>().itemName)
                {
                    
                    // Inv item 1-5
                    if (listPos > 0 && listPos < inventoryCols + 1)
                    {
                        invx = (listPos - 1) * xSpacing;
                        invy = ySpacing;
                        Debug.Log(listPos);
                    }
                    // Inv item 6-10
                    else if (listPos > inventoryCols && listPos < inventoryCols * 2 + 1)
                    {
                        invx = (listPos - 1 - inventoryCols) * xSpacing;
                        invy = 0;
                    }
                    // Inv item 11-15
                    else if (listPos > inventoryCols * 2 && listPos < inventoryCols * 3 + 1)
                    {
                        invx = (listPos - 1 - inventoryCols * 2) * xSpacing;
                        invy = -ySpacing;
                    }
                    // Cannot fit in inventory
                    else
                    {
                        Debug.LogError("ERROR: list pos is higher than inventory limit");
                    }
                    
                    // Place the UI icon on a position on the UI
                    IIU.GetComponent<RectTransform>().anchoredPosition = new Vector2(startingx + invx, invy);
                    
                    // Give it the scale to display properly
                    IIU.GetComponent<RectTransform>().localScale = new Vector3(1.2f,1,1);

                    break;
                }
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
                //newWeapon.transform.position = new Vector3(0,0,0);      

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

    //Disable hotkeys for now
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
                //EquipWeapon(hotKeyList[index - 1]);

                //SPELL

                //ITEM

            }
        }
    }

}

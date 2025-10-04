using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum InventoryDir
{
    UP,
    DOWN,
    LEFT,
    RIGHT
}

public class PlayerInventory : MonoBehaviour
{
    [Header("Items and Tabs")]
    [HideInInspector] public GameObject selectedItem;
    [HideInInspector] public GameObject selectedItemGObj;
    [SerializeField] private GameObject selectedInvWindow;
    [SerializeField] private GameObject invTabs;
    [SerializeField] private GameObject[] inventoryTabsArray;
    [HideInInspector] public GameObject inventoryInterface;
    [SerializeField] private GameObject defaultSwordObject;

    [Header("Inventory Windows")]
    [SerializeField] private GameObject weaponInvWindow;
    [SerializeField] private GameObject armourInvWindow;
    [SerializeField] private GameObject spellInvWindow;
    [SerializeField] private GameObject consumableInvWindow;
    public int tabIndex = 1; //Index from 1

    [Header("Inventory Lists")]
    [SerializeField] private int weaponInventoryLength = 15;
    [SerializeField] private int armourInventoryLength = 15;
    [SerializeField] private int spellInventoryLength = 15;
    [SerializeField] private int itemInventoryLength = 15;
    public int currInventoryIndex = 0;
    public GameObject[] weaponsList;
    public GameObject[] armourList;
    public GameObject[] spellsList;
    public GameObject[] consumablesList;
    public GameObject[] weaponsUIIcons;
    public GameObject[] armourUIIcons;
    public GameObject[] spellsUIIcons;
    public GameObject[] consumablesUIIcons;

    public GameObject[] hotKeyList = new GameObject[9];

    [Header("Indices")]
    //Starts from 1 rather than 0
    public int weaponInvIndex = 1;
    public int itemInvIndex = 1;
    public int spellInvIndex = 1;

    [Header("UI")]
    public Image HUDSpell;
    public Image HUDItem;

    [Header("Other Variables")]
    public GameObject weaponStock;
    private AudioSource invAudioSource;
    private int inventoryCols = 5;
    private int inventoryRows = 3;
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

        //weaponsList[0] = player.startingWeaponPickup;

        //TODO: add spells here

        //LoadHotkeys();
        LoadInventory();

        EquipWeapon(defaultSwordObject.GetComponent<WeaponPickup>().weaponHeldObject);
        AddToInventory(defaultSwordObject);
        Destroy(weaponStock.transform.GetChild(0).gameObject);        
    }

    private void LoadInventory()
    {

    }

    public void InventoryTab(ItemTypeUI tabTypeUI)
    {
        weaponInvWindow.SetActive(false);
        armourInvWindow.SetActive(false);
        spellInvWindow.SetActive(false);
        consumableInvWindow.SetActive(false);

        switch (tabTypeUI)
        {
            case ItemTypeUI.WEAPON:
                selectedInvWindow = weaponInvWindow;
                tabIndex = 1;
                break;

            case ItemTypeUI.ARMOUR:
                selectedInvWindow = armourInvWindow;
                tabIndex = 2;
                break;

            case ItemTypeUI.SPELL:
                selectedInvWindow = spellInvWindow;
                tabIndex = 3;
                break;

            case ItemTypeUI.CONSUMABLE:
                selectedInvWindow = consumableInvWindow;
                tabIndex = 4;
                break;

            case ItemTypeUI.KEYITEM:
                selectedInvWindow = consumableInvWindow;
                tabIndex = 4;
                break;

            case ItemTypeUI.ATTACKITEM:
                selectedInvWindow = consumableInvWindow;
                tabIndex = 4;
                break;

            default:

                break;
        }

        selectedInvWindow.SetActive(true);
        inventoryInterface = selectedInvWindow;

        // Make selected tab solid and all other tabs transparent

        for (int i = 0; i < inventoryTabsArray.Length; i++)
        {
            inventoryTabsArray[i].GetComponent<OnInventoryTabClicked>().TabIsSelected(false);
        }
        inventoryTabsArray[tabIndex - 1].GetComponent<OnInventoryTabClicked>().TabIsSelected(true);
            

        if (selectedItemGObj != null)
            selectedItemGObj.GetComponent<OnInventoryIconClicked>().Selected(false);

        currInventoryIndex = 0;
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

    public void SetInventoryIndex(GameObject inventoryIcon)
    {
        GameObject[] UIIconsInInventory = GetCurrentUIIconsType();

        for (int i = 0; i < UIIconsInInventory.Length; i++)
        {
            if (UIIconsInInventory[i] == inventoryIcon)
            {
                // Inventory index will index from 1, 0 means there is no index currently
                currInventoryIndex = i + 1;
            }
        }

         
    }

    public void ItemIsSelected(GameObject item, GameObject itemGObj, bool confirmable)
    {
        // Make item not null if item selected
        selectedItem = item;
        selectedItemGObj = itemGObj;

        if (itemGObj.GetComponent<OnInventoryIconClicked>().selected && confirmable)
        {
            UseItem();
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

                    if (itemGObj.GetComponent<OnInventoryIconClicked>().itemTypeUI == ItemTypeUI.CONSUMABLE)
                    {
                        HUDItem.sprite = itemGObj.transform.GetChild(0).GetComponent<Image>().sprite;
                        HUDItem.GetComponent<QuickSelect>().itemInQS = true;
                    }
                }
            }
        }

    }

    private GameObject[] GetCurrentUIIconsType()
    {
        GameObject[] UIIconType = new GameObject[0];

        if (selectedInvWindow == weaponInvWindow)
        {
            UIIconType = weaponsUIIcons;
        }
        else if (selectedInvWindow == armourInvWindow)
        {
            UIIconType = armourUIIcons;
        }
        else if (selectedInvWindow == spellInvWindow)
        {
            UIIconType = spellsUIIcons;
        }
        else if (selectedInvWindow == consumableInvWindow)
        {
            UIIconType = consumablesUIIcons;
        }                

        return UIIconType;
    }

    public void SelectInventoryPos(InventoryDir inventoryDir)
    {
        GameObject[] UIIconsInInventory = GetCurrentUIIconsType();

        //Debug.Log("Selected");
        if (currInventoryIndex == 0)
        {
            // Find first available item icon
            for (int i = 0; i < UIIconsInInventory.Length; i++)
            {
                if (UIIconsInInventory[i] != null)
                {
                    ItemIsSelected(UIIconsInInventory[i].GetComponent<OnInventoryIconClicked>().ITEM, UIIconsInInventory[i].gameObject, false);
                    currInventoryIndex = 1;
                    break;
                }
            }
        }
        else
        {

            switch (inventoryDir)
            {
                case InventoryDir.UP:
                    // MOVE UP
                    for (int i = currInventoryIndex - inventoryCols; i > 0; i--)
                    {
                        if (UIIconsInInventory[i-1] != null)
                        {
                            ItemIsSelected(UIIconsInInventory[i-1].GetComponent<OnInventoryIconClicked>().ITEM, UIIconsInInventory[i-1].gameObject, false);
                            SetInventoryIndex(UIIconsInInventory[i-1]);
                            break;
                        }
                    }
                    break;

                case InventoryDir.DOWN:
                    // MOVE DOWN
                    for (int i = currInventoryIndex + inventoryCols; i < UIIconsInInventory.Length; i++)
                    {
                        if (UIIconsInInventory[i-1] != null)
                        {
                            ItemIsSelected(UIIconsInInventory[i-1].GetComponent<OnInventoryIconClicked>().ITEM, UIIconsInInventory[i-1].gameObject, false);
                            SetInventoryIndex(UIIconsInInventory[i-1]);
                            break;
                        }
                    }
                    break;

                case InventoryDir.LEFT:
                    // MOVE LEFT
                    DecreaseInvIndex(UIIconsInInventory);
                    break;

                case InventoryDir.RIGHT:
                    // MOVE RIGHT
                    for (int i = currInventoryIndex; i < UIIconsInInventory.Length; i++)
                    {
                        if (UIIconsInInventory[i] != null)
                        {
                            ItemIsSelected(UIIconsInInventory[i].GetComponent<OnInventoryIconClicked>().ITEM, UIIconsInInventory[i].gameObject, false);
                            currInventoryIndex++;
                            break;
                        }
                    }
                    break;
            }
        }
    }

    public void SelectInventoryTab(InventoryDir inventoryDir)
    {
        if (inventoryDir == InventoryDir.RIGHT)
        {
            tabIndex++;

            if (tabIndex > inventoryTabsArray.Length)
                tabIndex = 1;
        }
        else if (inventoryDir == InventoryDir.LEFT)
        {
            tabIndex--;

            if (tabIndex < 1)
                tabIndex = inventoryTabsArray.Length;
        }

        ItemTypeUI ttUI = ItemTypeUI.WEAPON;

        switch (tabIndex)
        {
            case 1:
                ttUI = ItemTypeUI.WEAPON;
                break;
            case 2:
                ttUI = ItemTypeUI.ARMOUR;
                break;
            case 3:
                ttUI = ItemTypeUI.SPELL;
                break;
            case 4:
                ttUI = ItemTypeUI.CONSUMABLE;
                break;
                
        }

        InventoryTab(ttUI);
    }

    private void DecreaseInvIndex(GameObject[] UIIconsInInventory)
    {
        try
        {
            for (int i = currInventoryIndex; i > 0; i--)
            {
                if (UIIconsInInventory[i - 2] != null)
                {

                    ItemIsSelected(UIIconsInInventory[i - 2].GetComponent<OnInventoryIconClicked>().ITEM, UIIconsInInventory[i - 2].gameObject, false);
                    currInventoryIndex--;
                    break;

                }
            }
        }
        catch (IndexOutOfRangeException)
        { }
    }

    public void UseItem()
    {
        if (selectedItem != null && selectedItemGObj != null)
        {
            switch (selectedItemGObj.GetComponent<OnInventoryIconClicked>().itemTypeUI)
            {
                case ItemTypeUI.WEAPON:

                    foreach (Transform weapon in weaponStock.transform)
                    {
                        if (weapon.gameObject.GetComponent<PlayerWeaponValues>().weaponID == selectedItemGObj.GetComponent<OnInventoryIconClicked>().ITEM.GetComponent<InteractableItem>().itemName)
                        {
                            EquipWeapon(weapon.gameObject);
                        }
                    }


                    break;

                case ItemTypeUI.ARMOUR:

                    break;

                case ItemTypeUI.SPELL:
                    EquipSpell(selectedItemGObj.GetComponent<OnInventoryIconClicked>().dragonSpells);
                    break;

                case ItemTypeUI.CONSUMABLE:
                    selectedItemGObj.GetComponent<OnInventoryIconClicked>().consumable.GetComponent<ConsumableItem>().UseConsumable(player.gameObject.GetComponent<PlayerValues>(), this, selectedItem);
                    break;

                case ItemTypeUI.KEYITEM:

                    break;

                case ItemTypeUI.ATTACKITEM:

                    break;

                default:

                    break;
            }
        }
    }

    public void AddToInventory(GameObject itemFound)
    {
        GameObject savedItem = new GameObject();

        savedItem.transform.parent = gameObject.transform;

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
                for (int j = 0; j < weaponsList.Length; j++)
                {
                    if (weaponsList[j] != null)
                        weaponNames[j] = weaponsList[j].GetComponent<InteractableItem>().itemName;
                }

                //New Weapon, add
                for (int i = 0; i < weaponsList.Length; i++)
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
                        weaponsUIIcons[i] = invIcon;

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
                for (int i = 0; i < consumablesList.Length; i++)
                {
                    //Debug.Log("NAMEINLIST: " + consumablesList[i].GetComponent<InteractableItem>().itemName + ", NAME OF PICKED UP ITEM: " + savedItem.GetComponent<InteractableItem>().itemName);

                    if (consumablesList[i] != null)
                    {
                        if (consumablesList[i].GetComponent<InteractableItem>().itemName == savedItem.GetComponent<InteractableItem>().itemName)
                        {
                            //Dupicate item, stack
                            itemExists = true;

                            // Delete duplicate gameobjects of item being spawned in
                            Destroy(savedItem);

                            consumablesList[i].GetComponent<ConsumableItem>().itemQuantity++;

                            if (consumablesUIIcons[i].GetComponent<OnInventoryIconClicked>().quantityText != null)
                            {
                                consumablesUIIcons[i].GetComponent<OnInventoryIconClicked>().quantityText.text = consumablesList[i].GetComponent<ConsumableItem>().itemQuantity.ToString();
                            }
                            break;
                        }
                    }
                }

                if (!itemExists)
                {
                    //Add icon to UI
                    invIcon = Instantiate(savedItem.GetComponent<InteractableItem>().UIIcon);

                    //New Item, add
                    for (int i = 0; i < consumablesList.Length - 1; i++)
                    {
                        if (consumablesList[i] == null)
                        {
                            consumablesList[i] = savedItem;
                            consumablesUIIcons[i] = invIcon;

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
                        consumablesUIIcons[i].GetComponent<OnInventoryIconClicked>().quantityText.text = consumablesList[i].GetComponent<ConsumableItem>().itemQuantity.ToString();
                    }
                    else
                    {
                        //Remove item from inventory
                        RemoveFromInventory(consumablesList, consumablesUIIcons, i);
                    }
                }
            }
        }
    }

    private void RemoveFromInventory(GameObject[] inventoryList, GameObject[] inventoryListUIIcons, int index)
    {
        Destroy(inventoryList[index]);
        Destroy(inventoryListUIIcons[index].gameObject);

        inventoryList[index] = null;
        inventoryListUIIcons[index] = null;

        HUDItem.GetComponent<QuickSelect>().itemInQS = false;
        HUDItem.GetComponent<QuickSelect>().MakeImageBlank();
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

        for (int i = 0; i < selectedInvList.Length; i++)
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
                    IIU.GetComponent<RectTransform>().localScale = new Vector3(1.2f, 1, 1);

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

    //private void LoadHotkeys()
    //{
        //Can't Switch weapons while attacking!

      //  if (player.equippedWeapon.GetComponent<HotKey>().hotKey > 0 && player.equippedWeapon.GetComponent<HotKey>().hotKey < 10)
        //{
          //  hotKeyList[player.equippedWeapon.GetComponent<HotKey>().hotKey - 1] = player.equippedWeapon;
        //}

        //foreach (Transform weapon in weaponStock.transform)
        //{
          //  if (weapon.gameObject.GetComponent<HotKey>().hotKey > 0 && weapon.GetComponent<HotKey>().hotKey < 10)
            //{
              //  hotKeyList[weapon.gameObject.GetComponent<HotKey>().hotKey - 1] = weapon.gameObject;
                //weapon.GetComponent<HotKey>().originalItemIndex = i;
                //Debug.Log("DU: " + i);
            //}

            //i++;
        //}


    //}

    //Disable hotkeys for now
    //public void HotKeyedItem(int index, bool weaponSheathed)
    //{
      //  if (!player.attacking)
       // {
            //TODO: make this for all items not just weapons, based on the GameObject type/tag
            //Hot keys might be a stretch feature as using the inventory is surprsingly smooth
         //   if (hotKeyList[index - 1] != null)
          //  {

                //WEAPON
                //TODO: If is a weapon
                //EquipWeapon(hotKeyList[index - 1]);

                //SPELL

                //ITEM

           // }
        //}
   // }

}

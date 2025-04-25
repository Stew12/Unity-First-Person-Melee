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
    public List<GameObject> weaponsList = new List<GameObject>();
    public List<GameObject> armourList = new List<GameObject>();
    //public Dictionary<GameObject, int> consumablesList = new Dictionary<GameObject, int>();
    public List<GameObject> consumablesList = new List<GameObject>();
    [SerializeField] private List<GameObject> UIIconsInInventory = new List<GameObject>();
    
    // So I can look at consumable list values in inspector (can't see Dictionary in inspector)
    //public List<GameObject> DEBUGConsumablesList = new List<GameObject>();
    //public List<int> DEBUGConsumablesQuantities = new List<int>();
    
    
    public List<GameObject> spellsList = new List<GameObject>();
    //private List<int> itemValuesList = new List<int>();
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
                    EquipWeapon(item);
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

        switch (savedItem.GetComponent<InteractableItem>().interactedItemType)
        {
            case ItemTypeUI.WEAPON:
                
            break;

            case ItemTypeUI.ARMOUR:
                
            break;

            case ItemTypeUI.SPELL:
                
            break;

            case ItemTypeUI.CONSUMABLE:
                
                savedItem.AddComponent<ConsumableItem>();

                savedItem.GetComponent<ConsumableItem>().itemQuantity = itemFound.GetComponent<ConsumableItem>().itemQuantity;

                bool itemExists = false;
                for (int i = 0; i < consumablesList.Count; i++)
                {
                    Debug.Log("NAMEINLIST: " + consumablesList[i].GetComponent<InteractableItem>().itemName + ", NAME OF PICKED UP ITEM: " + savedItem.GetComponent<InteractableItem>().itemName);

                    if (consumablesList[i].GetComponent<InteractableItem>().itemName == savedItem.GetComponent<InteractableItem>().itemName)
                    {
                        //Dupicate item, stack
                        itemExists = true;

                        consumablesList[i].GetComponent<ConsumableItem>().itemQuantity++;

                        //Debug.Log(consumablesList[i].GetComponent<InteractableItem>().UIIcon.name);
                        if (UIIconsInInventory[i].GetComponent<OnInventoryIconClicked>().quantityText != null)
                        {
                            UIIconsInInventory[i].GetComponent<OnInventoryIconClicked>().quantityText.text = consumablesList[i].GetComponent<ConsumableItem>().itemQuantity.ToString();
                            Debug.Log(consumablesList[i].GetComponent<ConsumableItem>().itemQuantity.ToString());
                        }
                        break;
                    }
                }

                if (!itemExists)
                {
                    //Add icon to UI
                    GameObject invIcon = Instantiate(savedItem.GetComponent<InteractableItem>().UIIcon);
                    
                    //savedItem.GetComponent<InteractableItem>().UIIcon = invIcon;
                    
                    //New Item, add
                    consumablesList.Add(savedItem);
                    UIIconsInInventory.Add(invIcon);

                    invIcon.transform.parent = consumableInvWindow.transform;

                    consumablesList[consumablesList.Count - 1].GetComponent<ConsumableItem>().itemQuantity = 1;

                    if (consumablesList[consumablesList.Count - 1].GetComponent<InteractableItem>().UIIcon.GetComponent<OnInventoryIconClicked>().quantityText != null)
                    {
                        consumablesList[consumablesList.Count - 1].GetComponent<InteractableItem>().UIIcon.GetComponent<OnInventoryIconClicked>().quantityText.text = 1.ToString();
                    }

                    invIcon.GetComponent<OnInventoryIconClicked>().ItemSetup();
                    SetInvIconUILocation(invIcon);
                }

            break;
        }

        Destroy(itemFound);
    }

    public void DecreaseOrRemoveConsumable(GameObject dorItem)
    {
        for (int i = 0; i < consumablesList.Count; i++)
        {
            //Find Item
            if (consumablesList[i].GetComponent<InteractableItem>().UIIcon.GetComponent<OnInventoryIconClicked>().consumable.name == dorItem.name)
            {
                consumablesList[i].GetComponent<ConsumableItem>().itemQuantity--;

                if (consumablesList[i].GetComponent<ConsumableItem>().itemQuantity > 0)
                {
                    //Item has only had its quantity decreased. Update inventory label
                    consumablesList[i].GetComponent<InteractableItem>().UIIcon.GetComponent<OnInventoryIconClicked>().quantityText.text = consumablesList[i].GetComponent<ConsumableItem>().itemQuantity.ToString();
                }
                else
                {
                    //Remove item from inventory
                    consumablesList.Remove(consumablesList[i]);
                    Destroy(consumablesList[i].GetComponent<InteractableItem>().UIIcon.gameObject);
                }

                Debug.Log("NAME: " + dorItem.name);
            }
        }
    }

    private void SetInvIconUILocation(GameObject IIU)
    {
        int listPos = 0;

        switch (IIU.GetComponent<OnInventoryIconClicked>().itemTypeUI)
        {
            case ItemTypeUI.WEAPON:
                listPos = weaponsList.Count;
            break;

            case ItemTypeUI.ARMOUR:
                listPos = armourList.Count;
            break;

            case ItemTypeUI.CONSUMABLE:
                listPos = consumablesList.Count;

            break;
        }

        // Get UI position
        float invx = 0;
        float invy = 0;

        float xSpacing = 106.2f;
        float ySpacing = 100; 

        float startingx = -209.8f;

        //DEBUG
        //listPos += 9;

        // Inv item 1-5
        if (listPos > 0 && listPos < inventoryCols + 1)
        {
            invx = (listPos - 1) * xSpacing;
            invy = ySpacing;
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

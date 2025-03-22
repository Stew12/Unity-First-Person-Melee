using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableInInventory : MonoBehaviour
{
    public string itemName;
    public int itemQuantity;
    public ItemTypeUI itemTypeUI;
    public GameObject UIIcon;
    public ConsumableItem consumableItem;

    public ConsumableInInventory(GameObject item, int itemQuantity)
    {
        InteractableItem interactableItem = item.GetComponent<InteractableItem>();

        itemName = interactableItem.itemName;
        this.itemQuantity = itemQuantity;
        itemTypeUI = interactableItem.interactedItemType;
        UIIcon = interactableItem.UIIcon;
        consumableItem = item.GetComponent<ConsumableItem>();
    }
}

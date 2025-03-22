using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableInInventory : MonoBehaviour
{
    public string itemName;
    public int itemQuantity;

    public ConsumableInInventory(string itemName, int itemQuantity)
    {
        this.itemName = itemName;
        this.itemQuantity = itemQuantity;
    }
}

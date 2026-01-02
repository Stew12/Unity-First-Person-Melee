using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum ConsumableType
{
    HEALING,
    MANA,
    DURABILITY,
    DAMAGING,
    BUFF
}

public abstract class ConsumableItem : MonoBehaviour
{

    //Restores health, mana etc depending on what's selected
    [SerializeField] private int restoreAmount;

    public int itemQuantity = 0;
    
    public void UseConsumable(PlayerValues playerValues, PlayerInventory playerInventory, GameObject item)
    {
        UseConsumableItem(playerValues, playerInventory, restoreAmount);

        //Consumable item is either decremented or removed from player inventory upon use
        playerInventory.DecreaseOrRemoveConsumable(item);
    }

    public void AddConsumableItemComponent(GameObject newItem)
    {
        AddUniqueConsumableComponent(newItem);
    }

    public abstract void UseConsumableItem(PlayerValues playerValues, PlayerInventory playerInventory, int restoreAmount);
    public abstract void AddUniqueConsumableComponent(GameObject newItem);
}

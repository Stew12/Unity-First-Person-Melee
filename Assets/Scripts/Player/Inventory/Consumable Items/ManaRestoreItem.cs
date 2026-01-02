using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaRestoreItem : ConsumableItem
{
    public override void UseConsumableItem(PlayerValues playerValues, PlayerInventory playerInventory, int restoreAmount)
    {
        // Restore player Dragon Points (mana)

        int newDP = playerValues.currentDragonPoints + restoreAmount;
        
        if (newDP >= playerValues.maxDragonPoints)
        {
            playerValues.currentDragonPoints = playerValues.maxDragonPoints;
        }
        else
        {
            playerValues.currentDragonPoints = newDP;
        } 
    }

    public override void AddUniqueConsumableComponent(GameObject newItem)
    {
        newItem.AddComponent<ManaRestoreItem>();
    }
}

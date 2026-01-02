using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingItem : ConsumableItem
{
    public override void UseConsumableItem(PlayerValues playerValues, PlayerInventory playerInventory, int restoreAmount)
    {
        // Heal player
        int newHP = playerValues.currentHealth + restoreAmount;
        
        if (newHP >= playerValues.maxHealth)
        {
            playerValues.currentHealth = playerValues.maxHealth;
        }
        else
        {
            playerValues.currentHealth = newHP;
        } 
    }

    public override void AddUniqueConsumableComponent(GameObject newItem)
    {
        newItem.AddComponent<HealingItem>();
    }
}

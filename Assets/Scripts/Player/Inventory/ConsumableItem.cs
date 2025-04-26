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

public class ConsumableItem : MonoBehaviour
{
    [SerializeField] private ConsumableType consumableType;

    //Restores health, mana etc depending on what's selected
    [SerializeField] private int restoreAmount;

    public int itemQuantity = 0;
    
    public void UseConsumable(PlayerValues playerValues, PlayerInventory playerInventory, GameObject item)
    {
        switch (consumableType)
        {
            case ConsumableType.HEALING:
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
            break;

            case ConsumableType.MANA:
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
            break;

            case ConsumableType.DURABILITY:
                //TODO: Restore player weapon durability
            break;
        }

        //Consumable item is either decremented or removed from player inventory upon use
        playerInventory.DecreaseOrRemoveConsumable(item);
    }
}

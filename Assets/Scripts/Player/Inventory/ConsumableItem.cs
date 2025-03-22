using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum ConsumableType
{
    HEALING,
    MANA,
    DAMAGING,
    BUFF
}

public class ConsumableItem : MonoBehaviour
{
    [SerializeField] private ConsumableType consumableType;

    //Restores health, mana etc depeneding on what's selected
    [SerializeField] private int restoreAmount;
    
    public void UseConsumable(PlayerValues playerValues)
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
        }
    }
}

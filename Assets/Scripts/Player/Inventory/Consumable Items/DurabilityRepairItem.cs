using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DurabilityRepairItem : ConsumableItem
{
    public override void UseConsumableItem(PlayerValues playerValues, PlayerInventory playerInventory, int restoreAmount)
    {
        PlayerWeaponValues currWeaponValues = playerValues.gameObject.GetComponent<PlayerController>().equippedWeapon.GetComponent<PlayerWeaponValues>();

        int newDura = currWeaponValues.currentWeaponDurability + restoreAmount;
        
        if (newDura > currWeaponValues.maxWeaponDurability)
        {
            currWeaponValues.currentWeaponDurability = currWeaponValues.maxWeaponDurability;
        }
        else
        {
            currWeaponValues.currentWeaponDurability = newDura;
        } 
    }

    public override void AddUniqueConsumableComponent(GameObject newItem)
    {
        newItem.AddComponent<DurabilityRepairItem>();
    }
}

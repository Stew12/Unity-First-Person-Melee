using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupInteraction : Interactable
{
    void Awake()
    {
        interactionText = GetComponent<InteractableItem>().itemName;    
    }

    public override void InteractionBehaviour(PlayerController player)
    {
        player.playerInventory.AddToInventory(transform.gameObject);
    }
}

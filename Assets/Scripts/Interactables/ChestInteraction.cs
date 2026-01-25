using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestInteraction : Interactable
{

    public override void InteractionBehaviour(PlayerController player)
    {
        GetComponent<TreasureChest>().ChestOpen();
    }
}

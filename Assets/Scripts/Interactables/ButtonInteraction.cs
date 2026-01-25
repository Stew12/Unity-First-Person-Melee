using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInteraction : Interactable
{
    public override void InteractionBehaviour(PlayerController player)
    {
        transform.parent.GetComponent<DungeonButton>().ButtonActivation(player);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public string interactionText = "";

    public void Interacted(PlayerController player)
    {
        InteractionBehaviour(player);
    }

    public string InteractText()
    {
        return interactionText;
    }

    public abstract void InteractionBehaviour(PlayerController player);
}

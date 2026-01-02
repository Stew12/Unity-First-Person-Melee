using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] private string interactionText = "";

    public void Interacted()
    {
        InteractionBehaviour();
    }

    public string InteractText()
    {
        return interactionText;
    }

    public abstract void InteractionBehaviour();
}

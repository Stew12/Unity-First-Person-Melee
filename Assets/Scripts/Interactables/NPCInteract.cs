using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteract : Interactable
{
    public override void InteractionBehaviour(PlayerController player)
    {
        player.waiting = true;
        Cursor.lockState = CursorLockMode.None;
        player.dialogueTextBox.transform.parent.gameObject.SetActive(true);

        player.speakingNPC = GetComponent<NPC>();
        player.speakingNPC.PlayDialogue(player.dialogueTextBox);
    }
}

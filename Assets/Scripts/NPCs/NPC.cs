using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [HideInInspector] public int dialogueBoxIndex = 0;

    public List<string> dialogue = new List<string>();

    public void PlayDialogue(TextCrawl dialogueText)
    {
        dialogueText.GetComponent<TextCrawl>().WriteNewTextBox(dialogue[dialogueBoxIndex]);
    }

    private void Update()
    {
        //Debug.Log(PlayerInput.devices[0]);
    }

}

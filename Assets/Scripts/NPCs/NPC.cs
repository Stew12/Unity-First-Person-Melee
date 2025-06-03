using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public List<string> dialogue = new List<string>();

    public void PlayDialogue(TextCrawl dialogueText)
    {
        dialogueText.GetComponent<TextCrawl>().WriteNewTextBox(dialogue[0]);
    }

    private void Update()
    {
        //Debug.Log(PlayerInput.devices[0]);
    }

}

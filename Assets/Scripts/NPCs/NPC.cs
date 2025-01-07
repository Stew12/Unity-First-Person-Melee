using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public List<string> dialogue = new List<string>();
    public void PlayDialogue()
    {
        GameObject DialogueText = GameObject.FindGameObjectWithTag("Dialog Text");

        DialogueText.GetComponent<TextCrawl>().WriteNewTextBox(dialogue[0]);

    }

}

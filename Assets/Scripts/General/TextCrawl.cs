using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class TextCrawl : MonoBehaviour
{
    private TextMeshProUGUI textLabel;
    private AudioSource audioSource;

    private IEnumerator coroutine;

    [HideInInspector] public bool boxFinished = false;
    [HideInInspector] public bool showAllText = false;

    [SerializeField] float textWriteInterval = 0.05f;

    void Awake()
    {
        textLabel = GetComponent<TextMeshProUGUI>();
        textLabel.text = "";
        audioSource = GetComponent<AudioSource>();

        //WriteNewTextBox("TESTSTSTSTSTSTST LOLLLLLLL");
    }

    public void WriteNewTextBox(string line)
    {
        boxFinished = false;
        textLabel.text = "";
        showAllText = false;
        boxFinished = false;

        coroutine = writeTextBox(line);
        StartCoroutine(coroutine);
    }

    private IEnumerator writeTextBox(string line)
    {
        foreach(char c in line)
        {
            yield return new WaitForSeconds(textWriteInterval);

            if (!showAllText)
            {

                textLabel.text += c;

                // Play text sound
                audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
                audioSource.Play();
            }
            else
            {
                //Text finished early
                textLabel.text = "";
                textLabel.text = line;
                break;
            }
        }
        //Text finished
        boxFinished = true;
    }
}

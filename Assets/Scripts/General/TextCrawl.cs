using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextCrawl : MonoBehaviour
{
    private TextMeshProUGUI textLabel;
    private AudioSource audioSource;

    private IEnumerator coroutine;

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
        coroutine = writeTextBox(line);
        StartCoroutine(coroutine);
    }

    private IEnumerator writeTextBox(string line)
    {
        //for (int i = 0; i < 10; i++)
        //{
        foreach(char c in line)
        {
            yield return new WaitForSeconds(textWriteInterval);

            textLabel.text += c;

            // Play text sound
            audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
            audioSource.Play();
        }
        //}
    }
}

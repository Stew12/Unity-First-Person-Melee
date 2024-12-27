using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecordingDotFlash : MonoBehaviour
{
    public float maxFlashTime = 0.1f;
    private float flashTime;

    private bool imageOn = true;

    private Image recordingDot;

    // Start is called before the first frame update
    void Start()
    {
        recordingDot = GetComponent<Image>();

        flashTime = maxFlashTime;
    }

    // Update is called once per frame
    void Update()
    {
        flashTime -= Time.deltaTime;

        if (flashTime <= 0)
        {
            if (imageOn)
            {
                recordingDot.enabled = false;
                imageOn = false;
            }
            else
            {
                recordingDot.enabled = true;
                imageOn = true;
            }

            flashTime = maxFlashTime;
        }


    }
}

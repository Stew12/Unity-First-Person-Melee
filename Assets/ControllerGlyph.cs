using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllerGlyph : MonoBehaviour
{
    private Image displayedImage;

    [SerializeField] private Sprite keyboardGlyph;
    [SerializeField] private Sprite gamepadGlyph;

    void Awake()
    {
        displayedImage = GetComponent<Image>();
    }

    public void showGlyph(bool on)
    {
        if (on)
            displayedImage.enabled = true;
        else
            displayedImage.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (ControllerSettings.currentDevice == ActiveDevice.KEYBOARD)
        {
            displayedImage.sprite = keyboardGlyph;
        }
        else if (ControllerSettings.currentDevice == ActiveDevice.GAMEPAD)
        {
            displayedImage.sprite = gamepadGlyph;
        }
    }
}

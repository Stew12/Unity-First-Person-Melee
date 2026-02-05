using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum ActiveDevice
{
    KEYBOARD,
    GAMEPAD
}
public static class ControllerSettings
{
    public static ActiveDevice currentDevice = ActiveDevice.KEYBOARD;
}

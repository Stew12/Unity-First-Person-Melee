using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DebugSettings
{
    // Enables the 'tank controls' implementation- turn on or off based on player preference. May become a game setting
    public static bool noLookInputModeControls = true;

    // Enables whether touching the enemy (not their attack/projeciles) hurts the player. Want to test both ways to see what players like more.
    public static bool enemyGameObjectCollision = false; 

     // Controller type
    public static string controlType = "";

    // public static ActiveDevice getCurrentDevice()
    // {
    //     // Check control type
    //     foreach (InputDevice device in InputSystem.devices)
    //     {
    //         Debug.Log("Device Name: " + device.name);
    //     }
    // } 

    // static void Update()
    // {
    //     OnEnable();
    // }

    

}



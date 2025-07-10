using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;

    public enum GameStates
    {
        MAINMENU,
        RUNNING,
        PAUSED
    }

    public GameStates currentState;

    public void ChangeGameState(GameStates newState)
    {
        currentState = newState;

        // switch (newState)
        // {
        //     case GameStates.MAINMENU:
        //         inputSystemRef.SwitchCurrentActionMap("UI");
        //         break;
        //     case GameStates.Running:
        //         inputSystemRef.SwitchCurrentActionMap("Player");
        //         break;
        //     case GameStates.Paused:
        //         inputSystemRef.SwitchCurrentActionMap("UI");
        //         break;

        // }
    }

    
}

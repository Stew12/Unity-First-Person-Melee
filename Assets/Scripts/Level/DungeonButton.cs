using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonButton : MonoBehaviour
{
    [SerializeField] private GameObject connectedDevice;
    public void ButtonActivation(PlayerController player)
    {
        if (connectedDevice.GetComponent<Door>() != null)
        {
            //Open or close door automatically
            if (connectedDevice.GetComponent<Door>().buttonOpenDir == DoorDir.FRONT)
            {
                if (connectedDevice.GetComponent<Door>().closed)
                {
                    connectedDevice.GetComponent<Door>().DoorOpenOrClose(connectedDevice.GetComponent<Door>().LeftFront);
                }
                else
                {
                    connectedDevice.GetComponent<Door>().DoorOpenOrClose(connectedDevice.GetComponent<Door>().LeftBack);
                }
            }
            else
            {
                if (connectedDevice.GetComponent<Door>().closed)
                {
                    connectedDevice.GetComponent<Door>().DoorOpenOrClose(connectedDevice.GetComponent<Door>().LeftBack);
                }
                else
                {
                    connectedDevice.GetComponent<Door>().DoorOpenOrClose(connectedDevice.GetComponent<Door>().LeftFront);
                }
            }

            //connectedDevice.GetComponent<Door>().locked = !connectedDevice.GetComponent<Door>().locked;

            if (player != null)
            {
                player.StatusMessageShow(player.openDoorStatusMessage);
            }
        }
        //TODO spike trap
        //else if (connectedDevice.GetComponent<Door>() != null)
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DoorInteraction : Interactable
{
    [SerializeField] private string lockedDoorStatusMessage = "Locked...";
    //public string lockedOpenDoorStatusMessage = "Can't be moved.";
    [SerializeField] private string openDoorStatusMessage = "You heard something moving.";

    public override void InteractionBehaviour(PlayerController player)
    {   
        //Debug.Log("I: " + gameObject.name);

        GameObject doorRoot = null;

        if (gameObject.name == "DoorL" || gameObject.name == "DoorR")
        {
           doorRoot = gameObject.transform.parent.gameObject;
        }
        else if (gameObject.name == "DoorLBack" || gameObject.name == "DoorRBack")
        {
            doorRoot = gameObject.transform.parent.parent.gameObject;
        }
         
        if (doorRoot != null)
        {
            //Check if door unlocked
            if (!doorRoot.GetComponent<Door>().locked)
            {
                doorRoot.GetComponent<Door>().DoorOpenOrClose(gameObject.GetComponent<BoxCollider>());

                if (doorRoot.GetComponent<Door>().closed)
                {
                    interactionTextChange(doorRoot, "Open");
                }
                else
                {
                    interactionTextChange(doorRoot, "Close");
                }
            }
            else
            {
                if (doorRoot.GetComponent<Door>().closed)
                {
                    interactionTextChange(doorRoot, "Locked...");
                }
            }

        }
    }

    private void interactionTextChange(GameObject door, string newText)
    {
        door.GetComponent<Door>().LeftFront.gameObject.GetComponent<DoorInteraction>().interactionText = newText;
        door.GetComponent<Door>().RightFront.gameObject.GetComponent<DoorInteraction>().interactionText = newText;
        door.GetComponent<Door>().LeftBack.gameObject.GetComponent<DoorInteraction>().interactionText = newText;
        door.GetComponent<Door>().RightBack.gameObject.GetComponent<DoorInteraction>().interactionText = newText;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisions : MonoBehaviour
{
    private void OnTriggerEnter(Collider col)
    {
        switch (col.tag)
        {
            case "Detection Radius Enter":
                Debug.Log("Entered detection radius!");
                col.GetComponent<DetectionRadius>().PlayerEntered();
            break;

            default:

            break;
        }
    }

    private void OnTriggerExit(Collider col)
    {
        switch (col.tag)
        {
            case "Detection Radius Exit":
                Debug.Log("Exited detection radius...");
                col.GetComponent<DetectionRadius>().PlayerExited();
            break;

            default:

            break;
        }
    }
}

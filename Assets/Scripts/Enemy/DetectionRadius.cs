using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionRadius : MonoBehaviour
{
    public void PlayerEntered()
    {
        // Make the enemy start chasing the player
        transform.parent.gameObject.GetComponent<Enemy>().ChasePlayer();
    }
}

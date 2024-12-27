using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollisions : MonoBehaviour
{
    public CharacterController enemyController;

    // private void OnTriggerEnter(Collider col)
    // {
    //     switch (col.gameObject.tag)
    //     {
    //         case "Enemy Wall":
    //             //Don't move
    //             Debug.Log("AAAAAAAA");
    //             //col.isTrigger = false;
    //             enemyController.Move(new Vector3(0, 0, 0));
    //         break;
    //     }
    // }
}

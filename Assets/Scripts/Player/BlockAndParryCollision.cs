using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockAndParryCollision : MonoBehaviour
{
   /*  void Update()
    {
        GetComponentInParent<PlayerCollisions>().
    } */

    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Enemy")
        {
            if (GetComponentInParent<PlayerController>().parryWindowTime <= 0)
            {
                //Attack blocked normally
                GetComponentInParent<PlayerCollisions>().attackBlocked = true;
            }
            else
            {
                //Attack parried
                GetComponentInParent<PlayerCollisions>().attackParried = true;
            }

        }
    }
}

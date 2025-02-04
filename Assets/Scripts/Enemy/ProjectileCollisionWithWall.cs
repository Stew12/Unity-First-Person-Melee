using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectileCollisionWithWall : MonoBehaviour
{
    void OnCollisionEnter(Collision col)
    {
        Debug.Log("P HIT");
        if (col.transform.gameObject.tag != "Player")
        {
            Debug.Log("PN HIT");
            Destroy(transform.parent.gameObject);
        }
    }
}

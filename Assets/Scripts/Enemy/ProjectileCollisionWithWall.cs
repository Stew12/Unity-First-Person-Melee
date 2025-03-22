using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectileCollisionWithWall : MonoBehaviour
{
    void OnCollisionEnter(Collision col)
    {
        Debug.Log("P HIT");
        if (col.transform.gameObject.tag != "Player" && col.transform.gameObject.tag != "Enemy")
        {
            Debug.Log("PN HIT");

            transform.parent.GetComponent<EnemyProjectile>().enemyCasterClass.canFireProjectile = true;
            Destroy(transform.parent.gameObject);
        }
    }
}

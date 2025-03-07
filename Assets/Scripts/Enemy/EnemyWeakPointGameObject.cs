using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeakPointGameObject : MonoBehaviour
{
    public Enemy parentEnemy;

    void Awake()
    {
        parentEnemy = transform.parent.GetComponent<Enemy>();
    }
}

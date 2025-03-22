using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollisions : MonoBehaviour
{
    private Enemy enemy;

    void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player Projectile")
        {
            enemy.TakeDamage((int) col.GetComponent<PlayerProjectile>().projectileDamage, false, 0);
        }
        else if (col.gameObject.tag == "Player AOE")
        {
            enemy.TakeDamage((int) col.GetComponent<PlayerAOEAttack>().projectileDamage, false, 0);
            enemy.EnemyKnockBack(col.GetComponent<PlayerAOEAttack>().caster, false);
        }
    }
}

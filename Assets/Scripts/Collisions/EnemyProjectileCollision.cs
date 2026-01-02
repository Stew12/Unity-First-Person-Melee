using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileCollision : Collision
{
    public override void collision(Collider col, PlayerCollisions playerCol)
    {
        playerCol.EnemyProjectileCollision(col);
    }

    public override int damageValue(Collider col)
    {
        int totalDamage = 0;

        if (col.GetComponent<EnemyProjectile>() != null)
            totalDamage = (int)col.GetComponent<EnemyProjectile>().projectileDamage;
        else if (col.GetComponent<EnemyAttackDisjoint>() != null)
            totalDamage = (int)col.GetComponent<EnemyAttackDisjoint>().projectileDamage;

        return totalDamage;   
    }
}

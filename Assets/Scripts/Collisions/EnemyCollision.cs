using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollision : Collision
{
    private Enemy enemy;
    private IEnumerator coroutine;
    public float enemyColDelay = 0.00001f;
    
    void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    public override void collision(Collider col, PlayerCollisions playerCol)
    {
        if (DebugSettings.enemyGameObjectCollision)
        {
            coroutine = playerCol.EnemyCollision(col, enemyColDelay);
            StartCoroutine(coroutine);
        }
    }

    public override int damageValue(Collider col)
    {
        return (int)col.GetComponent<Enemy>().attackDamage;
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

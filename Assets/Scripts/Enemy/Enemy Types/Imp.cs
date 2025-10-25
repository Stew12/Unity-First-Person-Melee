using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Imp : Enemy
{
    private EnemyBehaviourAndAttackList enemyBehaviourAndAttackList;

    void Awake()
    {
        enemyBehaviourAndAttackList = new EnemyBehaviourAndAttackList();
    }

    public override EnemyAttack EnemyAttackBehaviour()
    {
        return enemyBehaviourAndAttackList.EnemyAttackRandomChoice(new EnemyAttack[] { EnemyAttack.BASICPHYSICAL, EnemyAttack.BASICAOE } );
    }
}

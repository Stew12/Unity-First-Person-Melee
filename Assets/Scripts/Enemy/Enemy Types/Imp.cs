using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Imp : Enemy
{
    void Awake()
    {
    }

    public override void EnemyAttackBehaviour(EnemyBehaviourAndAttackList enemyBehaviourAndAttackList)
    {
        // TODO random chance
        //return enemyBehaviourAndAttackList.EnemyAttackRandomChoice(new EnemyAttack[] { EnemyAttack.BASICPHYSICAL, EnemyAttack.BASICAOE } );
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sorceress : Enemy
{
    public override void EnemyAttackBehaviour(EnemyBehaviourAndAttackList enemyBehaviourAndAttackList)
    {
        enemyBehaviourAndAttackList.BasicRangedAttack();
    }
}

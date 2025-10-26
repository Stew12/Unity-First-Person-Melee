using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sorceress : Enemy
{
    private EnemyAttack enemyAttackType;

    public override void EnemyAttackBehaviour(EnemyBehaviourAndAttackList enemyBehaviourAndAttackList)
    {
        enemyBehaviourAndAttackList.BasicRangedAttack();
    }

    public override void SelectAttackType(EnemyBehaviourAndAttackList enemyBehaviourAndAttackList)
    {
        enemyAttackType = EnemyAttack.BASICRANGED;
    }

    public override GameObject SelectAttackMarker(GameObject[] atkMarkers)
    {
        return atkMarkers[0];
    }
}

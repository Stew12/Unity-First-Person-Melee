using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : Enemy
{
    private EnemyAttack enemyAttackType;

    public override void EnemyAttackBehaviour(EnemyBehaviourAndAttackList enemyBehaviourAndAttackList)
    {
        enemyBehaviourAndAttackList.BasicPhysicalAttack();
    }

    public override void SelectAttackType(EnemyBehaviourAndAttackList enemyBehaviourAndAttackList)
    {
        enemyAttackType = EnemyAttack.BASICPHYSICAL;
    }

    public override GameObject SelectAttackMarker(GameObject[] atkMarkers)
    {
        return atkMarkers[0];
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Imp : Enemy
{
    private EnemyAttack enemyAttackType;

    public override void SelectAttackType(EnemyBehaviourAndAttackList enemyBehaviourAndAttackList)
    {
        enemyAttackType = enemyBehaviourAndAttackList.EnemyAttackRandomChoice(new EnemyAttack[] { EnemyAttack.BASICPHYSICAL, EnemyAttack.BASICAOE } );
    }

    public override void EnemyAttackBehaviour(EnemyBehaviourAndAttackList enemyBehaviourAndAttackList)
    {
        if (enemyAttackType == EnemyAttack.BASICAOE)
        {
            enemyBehaviourAndAttackList.BasicAOEAttack(); 
        }
        else
        {
            enemyBehaviourAndAttackList.BasicPhysicalAttack(); 
        }
       
    }

    public override GameObject SelectAttackMarker(GameObject[] atkMarkers)
    {
        if (enemyAttackType == EnemyAttack.BASICAOE)
        {
            return atkMarkers[1]; 
        }
        else
        {
            return atkMarkers[0]; 
        }
    }
}

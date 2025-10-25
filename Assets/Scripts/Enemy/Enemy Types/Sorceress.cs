using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sorceress : Enemy
{
    public override EnemyAttack EnemyAttackBehaviour()
    {
        return EnemyAttack.BASICRANGED;
    }
}

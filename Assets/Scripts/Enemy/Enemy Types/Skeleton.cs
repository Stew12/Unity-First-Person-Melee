using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : Enemy
{
    public override EnemyAttack EnemyAttackBehaviour()
    {
        return EnemyAttack.BASICPHYSICAL;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapCollision : Collision
{
    public override void collision(Collider col, PlayerCollisions playerCol)
    {
        playerCol.TrapCollision(col);
    }

    public override int damageValue(Collider col)
    {
        return col.GetComponent<Trap>().damage;
    }

}

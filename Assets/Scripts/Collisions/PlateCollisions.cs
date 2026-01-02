using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateCollisions : Collision
{
    public override void collision(Collider col, PlayerCollisions playerCol)
    {
        col.GetComponent<DungeonButton>().ButtonActivation(playerCol.gameObject.GetComponent<PlayerController>());
    }

    public override int damageValue(Collider col)
    {
        return 0;
    }
}

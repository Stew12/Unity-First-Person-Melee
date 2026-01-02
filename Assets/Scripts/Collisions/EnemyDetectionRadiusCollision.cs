using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetectionRadiusCollision : Collision
{
    public override void collision(Collider col, PlayerCollisions playerCol)
    {
        col.GetComponent<DetectionRadius>().PlayerEntered();
    }

    public override int damageValue(Collider col)
    {
        return 0;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Collision : MonoBehaviour
{
    public void collidedWith(Collider col, PlayerCollisions playerCol)
    {
        collision(col, playerCol);
    }

    public int calculateDamage(Collider col)
    {
        return damageValue(col);
    }

    public abstract void collision(Collider col, PlayerCollisions playerCol);
    public abstract int damageValue(Collider col);
}

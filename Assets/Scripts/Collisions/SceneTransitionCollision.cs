using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransitionCollision : Collision
{
    public override void collision(Collider col, PlayerCollisions playerCol)
    {
        StartCoroutine(playerCol.LoadScene(col.GetComponent<SceneChange>()));
    }

    public override int damageValue(Collider col)
    {
        return 0;
    }
}

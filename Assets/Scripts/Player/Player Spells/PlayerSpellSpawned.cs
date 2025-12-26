using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerSpellSpawned : MonoBehaviour
{
    public void projSpawn(PlayerController player)
    {
        projectileSpawn(player);
    }

    public abstract void projectileSpawn(PlayerController player);
}

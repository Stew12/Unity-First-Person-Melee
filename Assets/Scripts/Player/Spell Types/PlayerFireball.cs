using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.AI;

public class PlayerFireball : PlayerSpellSpawned
{
    [SerializeField] private float spellSpawnHeight = 0.75f;
    [SerializeField] private float spellSpawnHorizOffset = 0.6f;

    public override void projectileSpawn(PlayerController player)
    {
        //transform.parent = player.gameObject.transform;
        
        Vector3 playerPos = player.gameObject.transform.position;

        Instantiate(gameObject, new Vector3(playerPos.x, playerPos.y + spellSpawnHeight, playerPos.z + spellSpawnHorizOffset), Quaternion.identity);
        
        //transform.position = new Vector3(playerPos.x, playerPos.y + spellSpawnHeight, playerPos.z + spellSpawnHorizOffset);

        //transform.parent = null;
    }
}

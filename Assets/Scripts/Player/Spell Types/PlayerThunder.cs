using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerThunder : PlayerSpellSpawned
{
    [HideInInspector] public GameObject player;
    [SerializeField] private GameObject thunderAOE;
    private GameObject spawnedTarget;

    private static bool spellTargetSpawned = false; 
    [SerializeField] private float spellTargetDistance = 0.75f;
    [SerializeField] private float spellTargetyOffset = 0.1f;

    public override void projectileSpawn(PlayerController player)
    {
        Debug.Log("STS " + spellTargetSpawned);
        if (!spellTargetSpawned)
        {
            // Spawn the spell target
            Vector3 playerlSpawnPos = new Vector3(player.gameObject.transform.position.x, player.gameObject.transform.position.y + spellTargetyOffset, player.gameObject.transform.position.z);
            spawnedTarget = Instantiate(gameObject, playerlSpawnPos + player.transform.forward + player.transform.forward * spellTargetDistance, Quaternion.Euler(new Vector3(90, 0, 0)));
            spawnedTarget.transform.parent = player.transform;

            spellTargetSpawned = true;
        }
        else
        {
            //Spawn the actual attack
            if (spawnedTarget != null)
            {
                GameObject lightning = Instantiate(thunderAOE, spawnedTarget.transform.position, Quaternion.identity);

                lightning.GetComponent<PlayerAOEAttack>().caster = player.gameObject;

                Destroy(spawnedTarget);

                spellTargetSpawned = false;
            }
        }
        
    }
}

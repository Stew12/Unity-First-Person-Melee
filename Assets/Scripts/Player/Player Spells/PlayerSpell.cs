using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class PlayerSpell : MonoBehaviour
{
    //private float castCoolDownTime;
    private IEnumerator coroutine;
    private PlayerController player;

    public GameObject spellObject;
    public float castTime;
    public int castDPCost;


    public void PrepareDragonSpell(PlayerController player, PlayerValues playerValues)
    {
        this.player = player;

        playerValues.currentDragonPoints -= castDPCost;

        coroutine = ExecuteDragonSpell(castTime);
        player.waiting = true;
        StartCoroutine(coroutine);
    }

    IEnumerator ExecuteDragonSpell(float castTime)
    {
        yield return new WaitForSeconds(castTime);

        Instantiate(spellObject);
        spellObject.GetComponent<PlayerSpellSpawned>().projSpawn(player);

        player.waiting = false;
    }

    private void FireBallBreath()
    {
        
    }

    // private void ThunderBreath()
    // {
    //     Vector3 playerPos = player.gameObject.transform.localPosition;

    //     if (!thunderTargetSpawned)
    //     {
    //         // Spawn thunder target
            
    //         Vector3 playerlSpawnPos = new Vector3 (transform.position.x, transform.position.y + thunderTargetyOffset, transform.position.z);

    //         GameObject lTarget = Instantiate(player.thunderTarget, playerlSpawnPos + player.transform.forward + player.transform.forward * thunderTargetDistance, Quaternion.Euler(new Vector3(90, 0, 0)));
            
    //         lTarget.transform.parent = player.transform;

    //         spawnedLTarget = lTarget;

    //         thunderTargetSpawned = true;
    //     }
    //     else
    //     {
    //         // Use lightning
    //         if (spawnedLTarget != null)
    //         {
    //             GameObject lightning = Instantiate(player.thunder, spawnedLTarget.transform.position, Quaternion.identity);

    //             lightning.GetComponent<PlayerAOEAttack>().caster = player.gameObject;

    //             Destroy(spawnedLTarget);

    //             thunderTargetSpawned = false;
    //         }
    //     }

    // }

    //public abstract void castSpell(PlayerController player, GameObject fireball, float offset1, float offset2, GameObject supportingSpellObject);

}

using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public enum DragonSpells
{
        NONE,
        FIREBALLBREATH,
        THUNDERBREATH
}

public class PlayerDragonSpellList : MonoBehaviour
{
    //private float castCoolDownTime;
    private IEnumerator coroutine;
    private PlayerController player;

    [Header("Fireball")]
    public float fireBallBreathCastTime = 1;
    public float fireBallBreathDPCost = 5;
    public float thunderBreathDPCost = 10;
    [SerializeField] private float fireBallBreathSpawnHeight = 0.75f;
    [SerializeField] private float fireBallBreathSpawnHorizOffset = 0.6f;


    [Header("Thunder Target")]
    [SerializeField] private float thunderBreathTargetCastTime = 0.1f;
    [SerializeField] private float thunderBreathCastTime = 1.1f;
    private bool thunderTargetSpawned; 
    private GameObject spawnedLTarget;
    [SerializeField] private float thunderTargetDistance = 0.75f;
    [SerializeField] private float thunderTargetyOffset = 0.1f;
    
    

    public void PrepareDragonSpell(DragonSpells dragonSpell, PlayerController player, PlayerValues playerValues)
    {
        this.player = player;
        float castTime = 0;
        float dragonPointCost = 0;

        switch (dragonSpell)
        {
            case DragonSpells.FIREBALLBREATH:
                castTime = fireBallBreathCastTime;
                dragonPointCost = fireBallBreathDPCost;
                break;

            case DragonSpells.THUNDERBREATH:
                if (!thunderTargetSpawned)
                    castTime = thunderBreathTargetCastTime;
                else
                    castTime = thunderBreathCastTime;

                if (thunderTargetSpawned)
                    dragonPointCost = fireBallBreathDPCost;
                break;
        }

        playerValues.currentDragonPoints -= (int)dragonPointCost;

        coroutine = ExecuteDragonSpell(dragonSpell, castTime);
        player.waiting = true;
        StartCoroutine(coroutine);
    }

    IEnumerator ExecuteDragonSpell(DragonSpells dragonSpell, float castTime)
    {
        yield return new WaitForSeconds(castTime);

        switch (dragonSpell)
        {
            case DragonSpells.FIREBALLBREATH:
                FireBallBreath();
            break;

            case DragonSpells.THUNDERBREATH:
                ThunderBreath();
            break;
        }

        player.waiting = false;
    }

    private void FireBallBreath()
    {
        Vector3 playerPos = player.gameObject.transform.position;
        //Fire a fireball
        GameObject fb = Instantiate(player.fireBall, new Vector3(playerPos.x, playerPos.y + fireBallBreathSpawnHeight, playerPos.z + fireBallBreathSpawnHorizOffset), Quaternion.identity);
    }

    private void ThunderBreath()
    {
        Vector3 playerPos = player.gameObject.transform.localPosition;

        if (!thunderTargetSpawned)
        {
            // Spawn thunder target
            
            Vector3 playerlSpawnPos = new Vector3 (transform.position.x, transform.position.y + thunderTargetyOffset, transform.position.z);

            GameObject lTarget = Instantiate(player.thunderTarget, playerlSpawnPos + player.transform.forward + player.transform.forward * thunderTargetDistance, Quaternion.Euler(new Vector3(90, 0, 0)));
            
            lTarget.transform.parent = player.transform;

            spawnedLTarget = lTarget;

            thunderTargetSpawned = true;
        }
        else
        {
            // Use lightning
            if (spawnedLTarget != null)
            {
                GameObject lightning = Instantiate(player.thunder, spawnedLTarget.transform.position, Quaternion.identity);

                lightning.GetComponent<PlayerAOEAttack>().caster = player.gameObject;

                Destroy(spawnedLTarget);

                thunderTargetSpawned = false;
            }
        }

    }

}

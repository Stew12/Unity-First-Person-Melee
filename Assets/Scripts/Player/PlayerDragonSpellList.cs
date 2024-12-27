using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public enum DragonSpells
{
        FIREBALLBREATH
}

public class PlayerDragonSpellList : MonoBehaviour
{
    //private float castCoolDownTime;
    private IEnumerator coroutine;
    private PlayerController player;

    [Header("Dragon Spell Values")]
    public float fireBallBreathCastTime = 1;
    public float fireBallBreathDPCost = 5;
    [SerializeField] private float fireBallBreathSpawnHeight = 0.75f;

    public void PrepareDragonSpell(DragonSpells dragonSpell, PlayerController player, PlayerValues playerValues)
    {
        float castTime = 0;
        float dragonPointCost = 0;

        switch (dragonSpell)
        {
            case DragonSpells.FIREBALLBREATH:
                this.player = player;
                castTime = fireBallBreathCastTime;
                dragonPointCost = fireBallBreathDPCost;
            break;
        }

        playerValues.currentDragonPoints -= (int)dragonPointCost;

        coroutine = ExecuteDragonSpell(dragonSpell, castTime);
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
        }
    }

    private void FireBallBreath()
    {
        Vector3 playerPos = player.gameObject.transform.position;
        //Fire a fireball
        GameObject fb = Instantiate(player.fireBall, new Vector3(playerPos.x, playerPos.y + fireBallBreathSpawnHeight, playerPos.z), Quaternion.identity);
        //fb.GetComponent<PlayerProjectile>().playerPos = player.gameObject.transform;
    }

}

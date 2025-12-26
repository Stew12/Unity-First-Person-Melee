using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerThunder : MonoBehaviour
{
    [HideInInspector] public GameObject player;
    private GameObject spellTarget;
    private bool spellTargetSpawned; 
    [SerializeField] private float spellTargetDistance = 0.75f;
    [SerializeField] private float spellTargetyOffset = 0.1f;

    // public override void castSpell()
    // {
    //     Vector3 playerPos = player.gameObject.transform.position;
    //     //Fire a fireball
    //     GameObject fb = Instantiate(player.fireBall, new Vector3(playerPos.x, playerPos.y + fireBallBreathSpawnHeight, playerPos.z + fireBallBreathSpawnHorizOffset), Quaternion.identity);
    // }
}

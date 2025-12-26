using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerFireball : MonoBehaviour
{
    [HideInInspector] public GameObject player;

    [SerializeField] private float spellSpawnHeight = 0.75f;
    [SerializeField] private float spellSpawnHorizOffset = 0.6f;

    public void castSpell()
    {
        Vector3 playerPos = player.gameObject.transform.position;
        
        transform.position = new Vector3(playerPos.x, playerPos.y + spellSpawnHeight, playerPos.z + spellSpawnHorizOffset);
    }
}

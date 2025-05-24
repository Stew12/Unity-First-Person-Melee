using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    [SerializeField] private GameObject treasure;
    [SerializeField] private GameObject bronzeDrop;
    [SerializeField] private Sprite openChest;
    [SerializeField] private bool bronzeInside = false;
    [SerializeField] private int bronze = 0;
    private float treasureSpawnDistanceRedMultiplier = 2.8f;

    public void ChestOpen()
    {
        GameObject trSpawned = null;

        if (bronzeInside)
        {
            // Spawn bronze
            trSpawned = Instantiate(bronzeDrop, transform.position + transform.forward / treasureSpawnDistanceRedMultiplier, transform.rotation);
            bronzeDrop.GetComponent<CoinDrop>().coinsSpawning = bronze;
        }
        else
        {
            // Spawn item
            trSpawned = Instantiate(treasure, transform.position + transform.forward / treasureSpawnDistanceRedMultiplier, transform.rotation);
        }
        trSpawned.transform.parent = transform;

        GetComponent<SpriteRenderer>().sprite = openChest;

        GetComponent<AudioSource>().Play();
    }

    void Update()
    {
       // Debug.Log(transform.forward);
    }

}
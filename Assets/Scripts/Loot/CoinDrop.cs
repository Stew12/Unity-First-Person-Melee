using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CoinDrop : MonoBehaviour
{
    [SerializeField] private GameObject coin;

    [HideInInspector] public int coinsSpawning;

    private float xSpawnRange;
    private float zSpawnRange;

    // Start is called before the first frame update
    void Start()
    {
        xSpawnRange = GetComponent<BoxCollider>().size.x / 2;
        zSpawnRange = GetComponent<BoxCollider>().size.z / 2;

        for (int i = 0; i < coinsSpawning; i++)
        {
            GameObject c = Instantiate(coin, new Vector3(transform.position.x + Random.Range(-xSpawnRange, xSpawnRange), transform.position.y, transform.position.z + Random.Range(-zSpawnRange, zSpawnRange)), Quaternion.Euler(new Vector3(90, 0, 0)));
            
            c.transform.parent = gameObject.transform;
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            col.GetComponent<PlayerCollisions>().CoinPickup(coinsSpawning);
            Destroy(gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathEffect : MonoBehaviour
{
    private IEnumerator coroutine;
    public float deathEffectTime = 0.3f;

    [SerializeField] private GameObject coinsDrop;
    [HideInInspector] public int coinAmount;
    

    private AudioSource deathAudioSource;
    public AudioClip deathSound;


    // Start is called before the first frame update
    void Start()
    {
        deathAudioSource = GetComponent<AudioSource>();

        coroutine = DeathEffect();
        StartCoroutine(coroutine);
    }

    // Play enemy death sound, then destroy object after set amount of time.
    private IEnumerator DeathEffect() 
    {
        deathAudioSource.PlayOneShot(deathSound);

        GameObject bronzeDrop = Instantiate(coinsDrop, transform.position, Quaternion.identity);
        bronzeDrop.GetComponent<CoinDrop>().coinsSpawning = coinAmount;

        yield return new WaitForSeconds(deathEffectTime);

        Destroy(gameObject);
    }

}

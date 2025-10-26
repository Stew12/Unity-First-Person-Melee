using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackMarker : MonoBehaviour
{
    [SerializeField] private float timeToAppear = 0.5f;
    [SerializeField] private float appearDuration = 1f;
    void Awake()
    {
        GetComponent<SpriteRenderer>().enabled = false;

        StartCoroutine(Appear(timeToAppear));
    }

    // Attack marker takes a moment to appear, appearing some moments after the enemy attack warning to catch player off guard.
    private IEnumerator Appear(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        GetComponent<SpriteRenderer>().enabled = true;
        StartCoroutine(Disappear(appearDuration));
    }

    // Destroy attack marker when time is up
    private IEnumerator Disappear(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        Destroy(gameObject);
    }

}

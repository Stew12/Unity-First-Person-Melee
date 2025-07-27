using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FIreTrap : MonoBehaviour
{
    [SerializeField] private GameObject flame;

    [SerializeField] private BoxCollider fireHitbox;

    [HideInInspector] public bool fireOn = false;
    private bool fireReady = false;
    [SerializeField] private bool autoFlame = false;

    [SerializeField] private float fireActivationTime = 1f;
    [SerializeField] private float timeBeforeFireOff = 3.5f;
    [SerializeField] private float fireCoolDownTime = 2f;

    // Start is called before the first frame update
    void Awake()
    {
        flame = gameObject.transform.GetChild(0).gameObject;
        fireHitbox = flame.GetComponent<BoxCollider>();
        flame.SetActive(false);
    }

    void Update()
    {
        if (autoFlame)
        {
            if (!fireReady)
            {
                StartCoroutine(FireAuto());
                fireReady = true;
            }
        }
    }

    // Player or enemy touches base of trap
    private void OnTriggerEnter(Collider col)
    {
        if (!fireOn && !autoFlame)
        {
            if (col.tag == "Player" || col.tag == "Enemy")
            {
                FireTrapTriggered(true);
            }
        }
    }

    public void FireTrapTriggered(bool fire)
    {
        if (fire)
        {
            // Ready fire trap
            StartCoroutine(FireSpawn());
            StartCoroutine(FireOff());
        }
        else
        {
            //Deactivate fire trap
            fireOn = false;
            fireReady = false;
            flame.SetActive(false);
        }

    }

    private IEnumerator FireSpawn()
    {
        yield return new WaitForSeconds(fireActivationTime);

        fireOn = true;
        flame.SetActive(true);
    }

    private IEnumerator FireOff()
    {
        yield return new WaitForSeconds(timeBeforeFireOff);

        FireTrapTriggered(false);
    }

    private IEnumerator FireAuto()
    {
        yield return new WaitForSeconds(fireCoolDownTime);

        FireTrapTriggered(true);
    }
}

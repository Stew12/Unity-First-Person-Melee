using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private BoxCollider spikeHitbox;

    private string currentAnimationState;
    private string SPIKESUPANIM = "spikeTrapImpale";
    private string SPIKESREADYANIM = "spikeTrapReadyImpale";
    private string SPIKESDOWNANIM = "spikeTrapRetract";

    [HideInInspector] public bool spikesUp = false;
    private bool spikesReady = false;
    [SerializeField] private bool autoImpaling = false;

    [SerializeField] private float spikeActivationTime = 1f;
    [SerializeField] private float timeBeforeSpikeRetraction = 3.5f;
    [SerializeField] private float spikeCoolDownTime = 2f;

    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
        spikeHitbox.enabled = false;
    }

    void Update()
    {
        if (autoImpaling)
        {
            if (!spikesReady)
            {
                StartCoroutine(SpikesAutoImpale());
                spikesReady = true;
            }
        }
    }

    // Player or enemy touches base of trap
    private void OnTriggerEnter(Collider col)
    {
        if (!spikesUp && !autoImpaling)
        {
            if (col.tag == "Player" || col.tag == "Enemy")
            {
                SpikeTrapTriggered(true);
            }
        }
    }

    public void SpikeTrapTriggered(bool impale)
    {

        if (impale)
        {
            // Ready spike trap
            ChangeSpikeTrapAnimationState(SPIKESREADYANIM);
            StartCoroutine(SpikesFullImpale());
            StartCoroutine(SpikesRetraction());
        }
        else
        {
            //Deactivate spike trap
            ChangeSpikeTrapAnimationState(SPIKESDOWNANIM);
        }

    }

    private void ChangeSpikeTrapAnimationState(string newState)
    {
        // STOP THE SAME ANIMATION FROM INTERRUPTING WITH ITSELF //
        if (currentAnimationState == newState) return;

        // PLAY THE ANIMATION //
        currentAnimationState = newState;
        animator.CrossFadeInFixedTime(currentAnimationState, 0.2f);
    }

    private IEnumerator SpikesFullImpale()
    {
        yield return new WaitForSeconds(spikeActivationTime);

        ChangeSpikeTrapAnimationState(SPIKESUPANIM);
        spikesUp = true;
        spikeHitbox.enabled = true;
    }

    private IEnumerator SpikesRetraction()
    {
        yield return new WaitForSeconds(timeBeforeSpikeRetraction);

        SpikeTrapTriggered(false);
        spikesUp = false;
        spikesReady = false;
        spikeHitbox.enabled = false;
    }

    private IEnumerator SpikesAutoImpale()
    {
        yield return new WaitForSeconds(spikeCoolDownTime);

        SpikeTrapTriggered(true);
    }

}

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    private Animator animator;

    private string currentAnimationState;
    private string SPIKESUPANIM = "spikeTrapImpale";
    private string SPIKESDOWNANIM = "spikeTrapRetract";

    public bool spikesUp = false;

    [SerializeField] private float spikeActivationTime = 0.2f;
    [SerializeField] private float timeBeforeSpikeRetraction = 2f;

    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Player or enemy touches base of trap
    private void OnTriggerEnter(Collider col)
    {
        if (!spikesUp)
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
            // Activate spike trap
            ChangeSpikeTrapAnimationState(SPIKESUPANIM);
            StartCoroutine(MakeSpikesDeadly());
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

    private IEnumerator MakeSpikesDeadly()
    {
        yield return new WaitForSeconds(spikeActivationTime);

        spikesUp = true;
    }

    private IEnumerator SpikesRetraction()
    {
        yield return new WaitForSeconds(timeBeforeSpikeRetraction);

        SpikeTrapTriggered(false);
        spikesUp = false;
    }
}

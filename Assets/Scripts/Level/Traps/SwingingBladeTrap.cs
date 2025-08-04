using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingingBladeTrap : MonoBehaviour
{
    private Animator animator;

    private string currentAnimationState;
    private string BLADELEFT = "SwingingBladeLeft";
    private string BLADERIGHT = "SwingingBladeRight";

    [HideInInspector] public bool sBladeReady = false;
    [SerializeField] private bool autoSwinging = false;

    [SerializeField] private float sBladeActivationTime = 1f;
    //[SerializeField] private float timeBeforesBladeOff = 3.5f;
    [SerializeField] private float sBladeCoolDownTime = 2f;

    public bool rightSwing = true;

    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();

        //SBladeTrapTriggered(true);
        //StartCoroutine(SBladeSwing());
    }

    // Update is called once per frame
    void Update()
    {
        if (autoSwinging)
        {
            if (!sBladeReady)
            {
                StartCoroutine(SBladeAutoSwing());
                sBladeReady = true;
            }
        }
    }

    public void SBladeTrapTriggered(bool swing)
    {
        if (swing)
        {
            StartCoroutine(SBladeSwing());
        }
        else
        {
            //Stop Swinging

        }

        
    }

    private void ChangeSBladeTrapAnimationState(string newState)
    {
        // STOP THE SAME ANIMATION FROM INTERRUPTING WITH ITSELF //
        if (currentAnimationState == newState) return;

        // PLAY THE ANIMATION //
        currentAnimationState = newState;
        animator.CrossFadeInFixedTime(currentAnimationState, 0.2f);
    }

    private IEnumerator SBladeSwing()
    {
        yield return new WaitForSeconds(sBladeActivationTime);

        if (rightSwing)
        {
            //Swing right
            ChangeSBladeTrapAnimationState(BLADERIGHT);
        }
        else
        {
            //Swing left
            ChangeSBladeTrapAnimationState(BLADELEFT);
        }

        //sBladeOn = true;
        //spikeHitbox.enabled = true;

        rightSwing = !rightSwing;
        
        sBladeReady = false;
    }

    private IEnumerator SBladeAutoSwing()
    {
        yield return new WaitForSeconds(sBladeCoolDownTime);

        SBladeTrapTriggered(true);
    }
}

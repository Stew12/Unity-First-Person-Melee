using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DoorDir
{
    FRONT,
    BACK
}

public class Door : MonoBehaviour
{
    public bool locked = false;

    public BoxCollider LeftFront;
    public BoxCollider RightFront;
    public BoxCollider LeftBack;
    public BoxCollider RightBack;

    public DoorDir buttonOpenDir = DoorDir.FRONT;

    [HideInInspector] public bool closed = true;
    private bool openedForward = false; /* Else, opened backward */
    [SerializeField] private float openedPitch = 0.8f;
    [SerializeField] private float closedPitch = 0.7f;

    private Animator animator;
    private AudioSource audioSource;
    string currentAnimationState;
    private string DOOROPENFORWARD = "doorOpenForward";
    private string DOORCLOSEFORWARD = "doorCloseForward";
    private string DOOROPENBACKWARD = "doorOpenBackward";
    private string DOORCLOSEBACKWARD = "doorCloseBackward";
    private string DOORCLOSED = "Null";

    void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    public void DoorOpenOrClose(BoxCollider collider)
    {
        if (closed)
        {
            /* Open na noor */
            if (collider == LeftFront || collider == RightFront)
            {
                ChangeDoorAnimationState(DOOROPENFORWARD);
                openedForward = true;
            }
            else if (collider == LeftBack || collider == RightBack)
            {
                ChangeDoorAnimationState(DOOROPENBACKWARD);
                openedForward = false;
            }

            closed = false;

            audioSource.pitch = openedPitch;
        }
        else
        {
            /* Close door */
            if (openedForward)
            {
                ChangeDoorAnimationState(DOORCLOSEFORWARD);
            }
            else
            {
                ChangeDoorAnimationState(DOORCLOSEBACKWARD);
            }

            closed = true;

            audioSource.pitch = closedPitch;
        }

        audioSource.Play();
    }

    public void ChangeDoorAnimationState(string newState)
    {
        // STOP THE SAME ANIMATION FROM INTERRUPTING WITH ITSELF //
        if (currentAnimationState == newState) return;

        // PLAY THE ANIMATION //
        currentAnimationState = newState;
        animator.CrossFadeInFixedTime(currentAnimationState, 0.2f);
    }
}

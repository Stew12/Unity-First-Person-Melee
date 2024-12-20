using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardingPlayerProjectile : MonoBehaviour
{
    private Transform cam;

    // Angular speed in radians per sec.
    //public float turnSpeed = 10.0f;

    void Awake()
    {
        cam = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = cam.transform.rotation;
    }
}

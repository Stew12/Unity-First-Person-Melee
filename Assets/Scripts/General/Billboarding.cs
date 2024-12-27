using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboarding : MonoBehaviour
{
    private Transform cam;

    // Angular speed in radians per sec.
    public float turnSpeed = 10.0f;

    private float yRotation = 0;

    void Awake()
    {
        cam = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetDirection = cam.position - transform.position;

        float singleStep = turnSpeed * Time.deltaTime;

        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

        //Remove y from new Direction- we do not want billboarded sprites to rotate up or down
        newDirection = new Vector3(newDirection.x, yRotation, newDirection.z);

        transform.rotation = Quaternion.LookRotation(newDirection);
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class RotatingPlatform : MonoBehaviour
{
    [SerializeField] private GameObject turningSegment;
    [SerializeField] private float rotationSpeed = 1.4f;
    [SerializeField] private bool horizontal = true;

    // Update is called once per frame
    void Update()
    {
        if (horizontal)
        {
            turningSegment.transform.Rotate(new UnityEngine.Vector3(0, 0, rotationSpeed));
        }
    }
}

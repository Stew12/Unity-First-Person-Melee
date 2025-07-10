using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    //[HideInInspector] public Transform playerPos;

    private Vector3 trajectory;

    [SerializeField] private float projectileSpeed = 4;
    public float projectileDamage;
    [SerializeField] private float rangeTime = 5;

    // Start is called before the first frame update
    void Start()
    {
        Transform cameraPos = Camera.main.transform;

        //Point towards player pos upon spawning
        trajectory = new Vector3(cameraPos.forward.x * projectileSpeed, cameraPos.forward.y * projectileSpeed, cameraPos.forward.z *  projectileSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        rangeTime -= Time.deltaTime;

        if (rangeTime <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            //Continue in that same direction even if player moves away
            transform.position += trajectory * Time.deltaTime;
        }
    }
}

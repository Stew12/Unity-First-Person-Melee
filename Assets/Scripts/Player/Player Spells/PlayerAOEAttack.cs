using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAOEAttack : MonoBehaviour
{
    public float projectileDamage;

    [HideInInspector] public GameObject caster;

    [SerializeField] private float rangeTime;

    // Update is called once per frame
    void Update()
    {
        rangeTime -= Time.deltaTime;

        if (rangeTime <= 0)
        {
            Destroy(gameObject);
        }
    }
}

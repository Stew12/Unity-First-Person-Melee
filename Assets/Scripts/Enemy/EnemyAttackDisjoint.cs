using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackDisjoint : MonoBehaviour
{
     [HideInInspector] public Enemy enemyCasterClass;
    [HideInInspector] public float projectileDamage;
    [HideInInspector] public float rangeTime;
    public bool parryable = false;
    private Vector3 trajectory;

    private Camera cam;

    private bool trajectorySet = false;

    void Awake()
    {
        if (GetComponent<AudioSource>() != null)
            GetComponent<AudioSource>().Play();
    }

    // Update is called once per frame
    void Update()
    {
        rangeTime -= Time.deltaTime;

        if (rangeTime <= 0)
        {
            EnemyCasterCanFire();
            Destroy(gameObject);
        }
    }

    public void EnemyCasterCanFire()
    {
        enemyCasterClass.canFireProjectile = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAOEAttack : MonoBehaviour
{
     [HideInInspector] public Enemy enemyCasterClass;
    public float projectileDamage;
    [SerializeField] private float rangeTime;

    private Vector3 trajectory;

    private Camera cam;

    private bool trajectorySet = false;

    // Start is called before the first frame update
    void Start()
    {

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [HideInInspector] public Enemy enemyCasterClass;

    [SerializeField] private float projectileSpeed = 4;
    public float projectileDamage;
    [SerializeField] private float rangeTime;

    private Vector3 trajectory;

    private Camera cam;

    private bool trajectorySet = false;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (!trajectorySet)
        {
             Vector3 targetDirection = cam.transform.position - transform.position;

            float singleStep = 10000000 * Time.deltaTime;

            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

            transform.rotation = Quaternion.LookRotation(newDirection);

            //Point towards player pos upon spawning
            trajectory = new Vector3(gameObject.transform.forward.x * projectileSpeed * Time.deltaTime, gameObject.transform.forward.y * projectileSpeed * Time.deltaTime, gameObject.transform.forward.z *  projectileSpeed * Time.deltaTime);

            trajectorySet = true;
        }

        rangeTime -= Time.deltaTime;

        if (rangeTime <= 0)
        {
            EnemyCasterCanFire();
            Destroy(gameObject);
        }
        else
        {
            //Continue in that same direction even if player moves away
            transform.position += trajectory;
            //rigidbody.AddForce(trajectory);
        }
    }

    public void EnemyCasterCanFire()
    {
        enemyCasterClass.canFireProjectile = true;
    }
}

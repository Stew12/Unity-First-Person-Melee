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

        //transform.rotation = Quaternion.Euler(0f,0f,0f);
        Debug.Log("Proj rot " + transform.rotation.x);
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
            trajectory = new Vector3(gameObject.transform.forward.x * projectileSpeed, gameObject.transform.forward.y * projectileSpeed, gameObject.transform.forward.z *  projectileSpeed);

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
            transform.position += trajectory * Time.deltaTime;
            //rigidbody.AddForce(trajectory);
        }
    }

    public void EnemyCasterCanFire()
    {
        enemyCasterClass.canFireProjectile = true;
    }
}

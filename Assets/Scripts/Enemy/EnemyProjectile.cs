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

    // Start is called before the first frame update
    void Start()
    {
        //Point towards player pos upon spawning
        trajectory = new Vector3(enemyCasterClass.gameObject.transform.forward.x * projectileSpeed * Time.deltaTime, enemyCasterClass.gameObject.transform.forward.y * projectileSpeed * Time.deltaTime, enemyCasterClass.gameObject.transform.forward.z *  projectileSpeed * Time.deltaTime);
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
        else
        {
            //Continue in that same direction even if player moves away
            transform.position += trajectory;
        }
    }

    public void EnemyCasterCanFire()
    {
        enemyCasterClass.canFireProjectile = true;
    }
}

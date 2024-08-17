using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    int currentHealth;
    public int maxHealth;

    private GameObject camera;

    void Awake()
    {
       // camera = GameObject.FindGameObjectWithTag("Camera");
        currentHealth = maxHealth;
    }

    void Update()
    {
        transform.LookAt(Camera.main.transform);
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if(currentHealth <= 0)
        { Death(); }
    }

    void Death()
    {
        // Death function
        // TEMPORARY: Destroy Object
        Destroy(gameObject);
    }
}

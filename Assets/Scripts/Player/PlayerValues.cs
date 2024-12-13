using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerValues : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public int maxDragonPoints;
    public int currentDragonPoints;

    

    // Start is called before the first frame update
    void Awake()
    {
        currentHealth = maxHealth;
        currentDragonPoints = maxDragonPoints;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

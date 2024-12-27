using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerValues : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 100;
    [HideInInspector] public int currentHealth;
    public int maxDragonPoints;
    [HideInInspector] public int currentDragonPoints;

    

    // Start is called before the first frame update
    void Awake()
    {
        currentHealth = maxHealth;
        currentDragonPoints = maxDragonPoints;
    }

}

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

    public int strength = 10;
    public int dexterity = 10;
    public int magicAttack = 10;
    public int defense = 10;
    public int magicDefence = 10;
    public int maxMomentum = 10;
    //Resourcefulness- increases effects of consumables
    public int resourcefulness = 10;    

    [Header("Collection")]

    public int bronze = 0;

    // Start is called before the first frame update
    void Awake()
    {
        currentHealth = maxHealth;
        currentDragonPoints = maxDragonPoints;
    }

}

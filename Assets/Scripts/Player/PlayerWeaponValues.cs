using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerWeaponValues : MonoBehaviour
{
    public enum WeaponClass
    {
        AXE,
        BAT,
        BROADSWORD,
        BOW,
        CLEAVER,
        DAGGER,
        GLAIVE,
        GREATSWORD,
        HAMMER,
        HATCHET,
        KNIFE,
        KUNAI,
        MACE,
        RAPIER,
        SCHIMTAR,
        SWORDSWORD,
        WAND
    }
    
    public WeaponClass weaponClass = WeaponClass.SWORDSWORD;

    public int weaponAttackDamage = 2;
    //Weight- higher weight slows player down when they attack
    public float weaponWeight = 1.2f;
    public float weaponAttackSpeed = 0.4f;
    public float weaponAttackDistance = 2.5f;
    public float weaponAttackDelay = 1.7f;
    [HideInInspector] public float weaponAttackDelayDefault = 0;

    public int maxWeaponDurability = 10;
    [HideInInspector] public int currentWeaponDurability = 0;

    void Awake()
    {
        currentWeaponDurability = maxWeaponDurability;

        weaponAttackDelayDefault = weaponAttackDelay;
    }

}

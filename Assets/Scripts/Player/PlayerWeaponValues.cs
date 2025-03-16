using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

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
        SHORTSWORD,
        WAND
    }

public class PlayerWeaponValues : MonoBehaviour
{
    public WeaponClass weaponClass = WeaponClass.SHORTSWORD;

    public float weaponAttackDamage = 2;
    //Weight- higher weight slows player down when they attack
    public float weaponWeight = 1.2f;
    public float weaponAttackSpeed = 0.4f;
    public float weaponAttackDistance = 2.5f;
    public float weaponAttackDelay = 1.7f;
    [HideInInspector] public float weaponAttackDelayDefault = 0;
    private float damageReduceFactor = 10;

    public int maxWeaponDurability = 10;
    public int currentWeaponDurability = 0;

    public AudioClip unsheatheSound;

    void Awake()
    {
        currentWeaponDurability = maxWeaponDurability;

        weaponAttackDelayDefault = weaponAttackDelay;
    }

    //If durability = 0, grey out the weapon to show wear and reduce attack damage greatly
    public void NoWeaponDurability()
    {
        GetComponent<SpriteRenderer>().color = new Color(0.33f, 0.33f, 0.33f);

        weaponAttackDamage = (int) (weaponAttackDamage / damageReduceFactor);
    }

}

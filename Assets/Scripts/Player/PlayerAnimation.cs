using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{

    //public const string BLOCK = "Sword Block";

    //GLAIVE
    public const string GL_THRUST = "Glaive Thrust";

    //MACE
    public const string MA_SWINGACROSS = "Mace Swing Across";
    public const string MA_SWINGBACK = "Mace Swing Across Back";

    // SHORTSWORD
    public const string SW_SWINGACROSS = "Sword Swing Across";
    //public const string SWINGDOWN = "Sword Swing Down";
    public const string SW_SWINGBACK = "Sword Swing Across Back";



    public void WeaponAnimationChange(WeaponClass weapon, PlayerController player)
    {
        switch (weapon)
        {

            case WeaponClass.AXE:
                player.SWINGACROSS = SW_SWINGACROSS;
                player.SWINGBACK = SW_SWINGBACK;
            break;

            case WeaponClass.DAGGER:
                player.SWINGACROSS = GL_THRUST;
                player.SWINGBACK = GL_THRUST;
            break;

            case WeaponClass.GLAIVE:
                player.SWINGACROSS = GL_THRUST;
                player.SWINGBACK = GL_THRUST;
            break;

            case WeaponClass.SHORTSWORD:
                player.SWINGACROSS = SW_SWINGACROSS;
                player.SWINGBACK = SW_SWINGBACK;
            break;
            



        }


    }

}

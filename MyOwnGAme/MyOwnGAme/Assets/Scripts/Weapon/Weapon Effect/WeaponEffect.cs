using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A GameObject that is spawned as ana effect of a weapon firing, e.g. proojectiles, auras, pulses.

public abstract class WeaponEffect : MonoBehaviour
{

    [HideInInspector]public PlayerStats owner;
    [HideInInspector]public Weapon weapon;

    public PlayerStats Owner;
    public float GetDamage()
    {
        
       return  weapon.GetDamage();
    }
}

using UnityEngine;
using System.Collections;

public class WeaponData : BaseItem
{
    public int physicalAttack;
    public int magicAttack;

    public enum WeaponTypes
    {
        SWORD,
        SWORD_2H,
        STAFF,
        SPEAR,
        MACE,
        BOW
    }

    public WeaponTypes weaponType;
}

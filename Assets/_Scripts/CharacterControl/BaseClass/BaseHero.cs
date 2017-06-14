using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class BaseHero: BaseClass
{
    public BaseHero()
    {
        baseStrength = 7;
        baseAgility = 8;
        baseResilience = 8;
        baseMind = 6;
        baseSpirit = 5;
        baseHealth = 100;

        BaseSpeed = CurrentAgility;
        BaseAccuracy = CurrentAgility + CurrentMind;
        BaseEvasion = CurrentAgility + CurrentSpirit;

        BaseAttackPower = CurrentStrength; //+ EquippedWeapon.PhysicalPower
        BaseMagicPower = CurrentMind; //+ EquippedWeapon.MagicPower
        BasePhysicalDefense = CurrentResilience; //+ CombinedEquipment.PhysicalDefense
        BaseMagicDefense = CurrentSpirit; //+ CombinedEquipment.MagicDefense
    }

    public List<AttackData> attacks = new List<AttackData>();
    public List<AttackData> fireSpells = new List<AttackData>();
    public List<AttackData> waterSpells = new List<AttackData>();
    public List<AttackData> earthSpells = new List<AttackData>();
    public List<AttackData> utility = new List<AttackData>();
    public List<AttackData> defend = new List<AttackData>();
}

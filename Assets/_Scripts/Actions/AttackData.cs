using UnityEngine;
using System.Collections;

public class AttackData : MonoBehaviour
{
    public GameObject projectile;

    public enum AttackType
    {
        MELEE, RANGED, SPELL, CLEANSE, RESTORE, HEAL, BUFF, DEBUFF, DEFEND
    }

    public enum DamageType
    {
        NORMAL, FIRE, WATER, EARTH, WOOD, METAL
    }

    // Action Basics
    [Header ("Action Basics")]
    public string attackName;
    public string attackDescription;

    public int attackDamage;
    public int energyCost;
    public int chargeCost;

    // Animation control
    [Header ("Animation Control")]
    public float attackWaitTime;
    public float damageWaitTime;
    public string attackAnimation;

    // Targetting info
    [Header ("Targetting Info")]
    public bool partyTargeting = false;
    public bool selfTarget = false;
    public float targetOffset;
    public float attackOffset;
    public float finishOffset;
    public bool moveDuringAttack = false;

    // Resource manipulation
    [Header ("Resource Manipulation")]
    public int energyRestore;
    public int fireChargeRestore;
    public int waterChargeRestore;
    public int earthChargeRestore;

    // Action effects
    [Header("Action Effects")]
    public int healthChange;
    public int strengthChange;
    public int speedChange;
    public int defenseChange;

    [Header ("Attack & Element Enums")]
    public AttackType attackType;
    public DamageType damageType;
}

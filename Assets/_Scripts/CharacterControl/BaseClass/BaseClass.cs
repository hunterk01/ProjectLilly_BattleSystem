using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseClass
{
    // Character info (public)
    public string characterName;
    public string characterDescription;

    // Basic Attributes (public)
    public int baseStrength;
    public int baseAgility;
    public int baseResilience;
    public int baseMind;
    public int baseSpirit;

    // Basic Attributes (private)
    private int currentStrength;
    private int currentAgility;
    private int currentResilience;
    private int currentMind;
    private int currentSpirit;

    // Resources (public)
    public float baseHealth;
    public float baseEnergy;
    public int maxFireCharges;
    public int maxWaterCharges;
    public int maxEarthCharges;

    // Resources (private)
    private float currentHealth;
    private float currentEnergy;
    private int currentFireCharges;
    private int currentWaterCharges;
    private int currentEarthCharges;

    // Secondary Attributes (private)
    // Speed = Agility
    private int baseSpeed;
    private int currentSpeed;
    // Accuracy = Agility + Mind
    private int baseAccuracy;
    private int currentAccuracy;
    // Evasion = Agility + (Resilience or Spirit?)
    private int baseEvasion;
    private int currentEvasion;
    // Attack Power = Strength + Weapon Base Physical Power
    private int baseAttackPower;
    private int currentAttackPower;
    // Magic Power = Mind + Weapon Base Magic Power
    private int baseMagicPower;
    private int currentMagicPower;
    // Physical Defense = Resilience + Combined Armor Base Physical Defense
    private int basePhysicalDefense;
    private int currentPhysicalDefense;
    // Magic Defense = Spirit + Combined Armor Base Magic Defense
    private int baseMagicDefense;
    private int currentMagicDefense;

    //// Name and description
    //public string CharacterName
    //{
    //    get { return characterName; }
    //    set { characterName = value; }
    //}

    //public string CharacterDescription
    //{
    //    get { return characterDescription; }
    //    set { characterDescription = value; }
    //}

    // Basic attributes (private)
    //public int BaseStrength
    //{
    //    get { return baseStrength; }
    //    set { baseStrength = value; }
    //}

    public int CurrentStrength
    {
        get { return currentStrength; }
        set { currentStrength = value; }
    }

    //public int BaseAgility
    //{
    //    get { return baseAgility; }
    //    set { baseAgility = value; }
    //}

    public int CurrentAgility
    {
        get { return currentAgility; }
        set { currentAgility = value; }
    }

    //public int BaseResilience
    //{
    //    get { return baseResilience; }
    //    set { baseResilience = value; }
    //}

    public int CurrentResilience
    {
        get { return currentResilience; }
        set { currentResilience = value; }
    }
    
    //public int BaseMind
    //{
    //    get { return baseMind; }
    //    set { baseMind = value; }
    //}

    public int CurrentMind
    {
        get { return currentMind; }
        set { currentMind = value; }
    }

    //public int BaseSpirit
    //{
    //    get { return baseSpirit; }
    //    set { baseSpirit = value; }
    //}

    public int CurrentSpirit
    {
        get { return currentSpirit; }
        set { currentSpirit = value; }
    }

    // Resources (private)
    //public int BaseHealth
    //{
    //    get { return baseHealth; }
    //    set { baseHealth = value; }
    //}

    public float CurrentHealth
    {
        get { return currentHealth; }
        set { currentHealth = value; }
    }

    //public int BaseEnergy
    //{
    //    get { return baseEnergy; }
    //    set { baseEnergy = value; }
    //}

    public float CurrentEnergy
    {
        get { return currentEnergy; }
        set { currentEnergy = value; }
    }

    //public int MaxFireCharges
    //{
    //    get { return maxFireCharges; }
    //    set { maxFireCharges = value; }
    //}

    public int CurrentFireCharges
    {
        get { return currentFireCharges; }
        set { currentFireCharges = value; }
    }

    //public int MaxWaterCharges
    //{
    //    get { return maxWaterCharges; }
    //    set { maxWaterCharges = value; }
    //}

    public int CurrentWaterCharges
    {
        get { return currentWaterCharges; }
        set { currentWaterCharges = value; }
    }

    //public int MaxEarthCharges
    //{
    //    get { return maxEarthCharges; }
    //    set { maxEarthCharges = value; }
    //}

    public int CurrentEarthCharges
    {
        get { return currentEarthCharges; }
        set { currentEarthCharges = value; }
    }

    // Speed, accuracy, and evasion
    public int BaseSpeed
    {
        get { return baseSpeed; }
        set { baseSpeed = value; }
    }

    public int CurrentSpeed
    {
        get { return currentSpeed; }
        set { currentSpeed = value; }
    }

    public int BaseAccuracy
    {
        get { return baseAccuracy; }
        set { baseAccuracy = value; }
    }

    public int CurrentAccuracy
    {
        get { return currentAccuracy; }
        set { currentAccuracy = value; }
    }

    public int BaseEvasion
    {
        get { return baseEvasion; }
        set { baseEvasion = value; }
    }

    public int CurrentEvasion
    {
        get { return currentEvasion; }
        set { currentEvasion = value; }
    }

    // Attack and Defense
    public int BaseMagicPower
    {
        get { return baseMagicPower; }
        set { baseMagicPower = value; }
    }

    public int CurrentMagicPower
    {
        get { return currentMagicPower; }
        set { currentMagicPower = value; }
    }

    public int BaseAttackPower
    {
        get { return baseAttackPower; }
        set { baseAttackPower = value; }
    }

    public int CurrentAttackPower
    {
        get { return currentAttackPower; }
        set { currentAttackPower = value; }
    }

    public int BasePhysicalDefense
    {
        get { return basePhysicalDefense; }
        set { basePhysicalDefense = value; }
    }

    public int CurrentPhysicalDefense
    {
        get { return currentPhysicalDefense; }
        set { currentPhysicalDefense = value; }
    }

    public int BaseMagicDefense
    {
        get { return baseMagicDefense; }
        set { baseMagicDefense = value; }
    }

    public int CurrentMagicDefense
    {
        get { return currentMagicDefense; }
        set { currentMagicDefense = value; }
    }
}
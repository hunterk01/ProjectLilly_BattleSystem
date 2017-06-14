using UnityEngine;
using System.Collections;

public class BaseArmor : BaseItem
{
    public int physicalDefense;
    public int magicDefense;

    public enum ArmorTypes
    {
        HELM,
        BODY,
        LEGS,
        ACCESSORY
    }

    public enum ArmorMaterials
    {
        CLOTH,
        LEATHER,
        PLATE
    }

    public ArmorTypes armorType;
    public ArmorMaterials armorMaterial;
}

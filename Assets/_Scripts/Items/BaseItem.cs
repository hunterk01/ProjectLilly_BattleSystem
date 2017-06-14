using UnityEngine;
using System.Collections;

public class BaseItem : MonoBehaviour
{
    public string itemName;
    public string itemDescription;
    public string itemID;

    public int strengthBonus;
    public int agilityBonus;
    public int resilienceBonus;
    public int mindBonus;
    public int spiritBonus;

    public enum ItemTypes
    {
        ARMOR,
        WEAPON,
        POTION
    }

    public ItemTypes itemType;
}

using UnityEngine;
using System.Collections;

public class PotionData : BaseItem
{
    public int healthRestore;
    public int staminaRestore;
    public int resolveRestore;
    public int fireChargeRestore;
    public int waterChargeRestore;
    public int earthChargeRestore;

    public enum PotionTypes
    {
            RESTORE,
            BUFF,
            DEBUFF        
    }

    public PotionTypes potionType;
}

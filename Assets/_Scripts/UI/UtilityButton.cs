using UnityEngine;
using System.Collections;

public class UtilityButton : MonoBehaviour
{
    public AttackData utilityToUse;

    public void doUtility()
    {
        GameObject.Find("BattleManager").GetComponent<BattleController>().SetUtilityInput(utilityToUse);
    }
}

using UnityEngine;
using System.Collections;

public class MeleeAttackButton : MonoBehaviour
{
    public AttackData meleeAttack;

    public void doMeleeAttack()
    {
        GameObject.Find("BattleManager").GetComponent<BattleController>().MeleeInput(meleeAttack);
    }
}

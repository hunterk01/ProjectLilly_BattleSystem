using UnityEngine;
using System.Collections;

public class SpellCastButton : MonoBehaviour
{
    public AttackData spellToCast;

    public void castSpell()
    {
        GameObject.Find("BattleManager").GetComponent<BattleController>().SpellInput(spellToCast);
    }
}

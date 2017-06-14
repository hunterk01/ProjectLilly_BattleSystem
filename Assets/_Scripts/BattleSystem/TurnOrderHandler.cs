using UnityEngine;
using System.Collections;

[System.Serializable]
public class TurnOrderHandler
{
    public string activeAgent;      // Name of the active agent
    public GameObject agentGO;      // GameObject of active agent
    public GameObject targetGO;     // GameObject of target being attacked
    public AttackData chosenAttack; // Chosen attack details
}

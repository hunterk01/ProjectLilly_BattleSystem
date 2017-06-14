using UnityEngine;
using System.Collections;

public class EnemySelectButton : MonoBehaviour
{
    public GameObject enemyPrefab;

    public void SelectEnemy()
    {
        GameObject.Find("BattleManager").GetComponent<BattleController>().EnemySelectInput(enemyPrefab);
    }

    public void ShowSelector()
    {
        enemyPrefab.transform.FindChild("Selector").gameObject.SetActive(true);
    }

    public void HideSelector()
    {
        enemyPrefab.transform.FindChild("Selector").gameObject.SetActive(false);
    }
}

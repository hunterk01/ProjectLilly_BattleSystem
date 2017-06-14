using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroSelectButton : MonoBehaviour
{
    public GameObject heroPrefab;

    public void SelectHero()
    {
        GameObject.Find("BattleManager").GetComponent<BattleController>().HeroSelectInput(heroPrefab);
    }

    public void ShowSelector()
    {
        heroPrefab.transform.FindChild("Selector").gameObject.SetActive(true);
    }

    public void HideSelector()
    {
        heroPrefab.transform.FindChild("Selector").gameObject.SetActive(false);
    }
}

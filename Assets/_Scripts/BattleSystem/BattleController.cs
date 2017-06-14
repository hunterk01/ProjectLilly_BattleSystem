
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BattleController : MonoBehaviour
{
    // Enums for tracking primary and secondary evironment elements
    public enum TerrainElementPrimary
    {
        FIRE, WATER, EARTH, WOOD, METAL
    }
    [HideInInspector]
    public TerrainElementPrimary terrainElementPrimary;

    public enum TerrainElementSecondary
    {
        FIRE, WATER, EARTH, WOOD, METAL
    }
    [HideInInspector]
    public TerrainElementSecondary terrainElementSecondary;

    // State engine for perfoming actions
    public enum ActionState
    {
        WAITING,        // Waiting for input
        RECEIVEACTION,  // Receive input
        PERFORMACTION,  // Perfrom action based on input
        CHECKFORDEAD,   // Check if any agents are dead
        WIN,            // Heroes won the battle
        LOSE            // Heroes lost the battle
    }
    public ActionState actionState;

    // State engine for handling hero input
    public enum HeroUI
    {
        ACTIVATE,   // Turn on hero UI
        IDLE,       // Waiting between actions
        INPUT1,     // Button 1 selected
        INPUT2,     // Button 2 selected
        DONE        // Input has been completed
    }
    public HeroUI heroInput;

    private TurnOrderHandler heroChoice;

    [Header("Animators")]
    Animator animatorCamera;
    public Animator animPanelGabi;
    public Animator animPanelQuinn;
    public Animator animPanelArvandus;

    [Header("Battle Control Lists")]
    public List<TurnOrderHandler> activeAgentList = new List<TurnOrderHandler>();
    public List<GameObject> heroesInBattle = new List<GameObject>();
    public List<GameObject> enemiesInBattle = new List<GameObject>();
    public List<GameObject> deadHeroes = new List<GameObject>();
    public List<GameObject> deadEnemies = new List<GameObject>();
    public List<GameObject> heroesToManage = new List<GameObject>();
    public List<GameObject> upperRowEnemies = new List<GameObject>();
    public List<GameObject> lowerRowEnemies = new List<GameObject>();

    [Header("UI Panels")]
    public GameObject fadeInPanel;
    public GameObject actionPanel;
    public GameObject earthPanel;
    public GameObject firePanel;
    public GameObject waterPanel;
    public GameObject attackPanel;
    public GameObject utilityPanel;
    public GameObject failedActionPanel;
    public GameObject enemySelectPanel;
    public GameObject heroSelectPanel;
    public GameObject corruptionMeter;
    public GameObject victoryPanel;
    public GameObject defeatPanel;
    public GameObject endGamePanel;

    [Header("UI Panel Spacers")]
    public Transform actionSpacer;
    public Transform earthSpacer;
    public Transform fireSpacer;
    public Transform waterSpacer;
    public Transform attackSpacer;
    public Transform utilitySpacer;

    [Header("UI Buttons")]
    public GameObject actionButton;
    public GameObject defendButton;
    public GameObject meleeButton;
    public GameObject spellButton;
    public GameObject utilityButton;
    public GameObject enemyButton;

    [Header("Other Stuff")]
    //Button Lists
    private List<GameObject> attackButtons = new List<GameObject>();
    private List<GameObject> enemyButtonList = new List<GameObject>();

    // Battle start delay
    public bool startBattle = false;
    private float startDelay = 5f;
    private float startDelayTimer;
    public bool pauseBattle = false;

    // Battle end delay
    private float endDelay = 5f;
    private float endDelayTimer;

    // Fade In Properties
    public float fadeInTimer = 0.0f, fadeInLength = 10f;
    public float endGameLength = 10f;
    Color fadeInColorStart, fadeInColorEnd;

    // Hero panel animation
    public bool gabiTurn = false;
    public bool quinnTurn = false;
    public bool arvandusTurn = false;
    public bool heroPanelActive = false;
    public bool heroPanelOn = false;
    private bool expandTriggered = false;
    private bool contractTriggered = false;

    void Start ()
	{
        SpawnEnemies();

        animatorCamera = GameObject.Find("MainCamera").GetComponentInChildren<Animator>();

        terrainElementPrimary = TerrainElementPrimary.EARTH;
        terrainElementSecondary = TerrainElementSecondary.WOOD;

        actionState = ActionState.WAITING;
        heroInput = HeroUI.ACTIVATE;
        enemiesInBattle.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        heroesInBattle.AddRange(GameObject.FindGameObjectsWithTag("Hero"));
        actionPanel.SetActive(false);
        attackPanel.SetActive(false);
        earthPanel.SetActive(false);
        firePanel.SetActive(false);
        earthPanel.SetActive(false);
        waterPanel.SetActive(false);
        utilityPanel.SetActive(false);
        failedActionPanel.SetActive(false);
        enemySelectPanel.SetActive(false);
        heroSelectPanel.SetActive(false);
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
        endGamePanel.SetActive(false);

        startDelayTimer = startDelay;
        endDelayTimer = endDelay;
    }
	
	void Update ()
	{
        if (startBattle)
        {
            if (!pauseBattle)
            {
                CheckActionState();
                CheckHeroInputState();
            }
            else
            {
                if (fadeInTimer < endGameLength)
                {
                    fadeInTimer += Time.deltaTime;
                }
                else
                {
                    endGamePanel.SetActive(true);
                }
            }
        }
        else
        {
            if (startDelayTimer <= 0)
            {
                startBattle = true;
            }
            else
            {
                if (fadeInTimer > fadeInLength)
                {
                    fadeInTimer += Time.deltaTime;
                }
                else
                {
                    endGamePanel.SetActive(true);
                }

                startDelayTimer -= Time.deltaTime;
            }
        }

        if (heroPanelActive)
        {
            if (heroPanelOn) ExpandHeroPanels();
            else ContractHeroPanels();
        }
    }

    // Spawn random enemies at the spawn points
    void SpawnEnemies()
    {
        // Make sure all upper and lower row enemies are inactive
        for (int i = 0; i < upperRowEnemies.Count; i++)
        {
            upperRowEnemies[i].SetActive(false);
        }

        for (int i = 0; i < lowerRowEnemies.Count; i++)
        {
            lowerRowEnemies[i].SetActive(false);
        }

        // Spawn random upper row enemy
        int randomSpawn1;
        randomSpawn1 = Random.Range(0, upperRowEnemies.Count);
        upperRowEnemies[randomSpawn1].SetActive(true);

        // Spawn random lower row enemy
        int randomSpawn2;
        randomSpawn2 = Random.Range(0, lowerRowEnemies.Count);
        lowerRowEnemies[randomSpawn2].SetActive(true);
    }

    // Switch for handling battle phases
    void CheckActionState()
    {
        switch (actionState)
        {
            case (ActionState.WAITING):
                if (activeAgentList.Count > 0) actionState = ActionState.RECEIVEACTION;
                break;
            case (ActionState.RECEIVEACTION):
                ReceiveAction();
                break;
            case (ActionState.PERFORMACTION):
                // Idle
                break;
            case (ActionState.CHECKFORDEAD):
                CheckForDead();
                break;
            case (ActionState.WIN):
                WinBattle();
                break;
            case (ActionState.LOSE):
                LoseBattle();
                break;
        }
    }

    // Switch for handling hero turn/waiting battle states
    void CheckHeroInputState()
    {
        switch (heroInput)
        {
            case (HeroUI.ACTIVATE):
                ActivateHero();
                break;
            case (HeroUI.IDLE):
                // Idle state
                break;
            case (HeroUI.INPUT1):

                break;
            case (HeroUI.INPUT2):

                break;
            case (HeroUI.DONE):
                HeroInputDone();
                break;
        }
    }

    // Activate the hero whose turn is up and setup the proper UI
    void ActivateHero()
    {
        if (heroesToManage.Count > 0)
        {
            heroesToManage[0].transform.FindChild("Selector").gameObject.SetActive(true);
            heroChoice = new TurnOrderHandler();

            // TODO: Hero panel animation

            actionPanel.SetActive(true);
            CreateActionButtons();

            heroInput = HeroUI.IDLE;
        }
    }

    // Log hero, or enemy, choices into the active agent list
    void ReceiveAction()
    {
        GameObject agent = GameObject.Find(activeAgentList[0].activeAgent);

        if (agent.transform.tag == "Enemy")
        {
            EnemyController enemyControl = agent.GetComponent<EnemyController>();

            for (int i = 0; i < heroesInBattle.Count; i++)
            {
                if (activeAgentList[0].targetGO == heroesInBattle[i])
                {
                    enemyControl.currentState = EnemyController.EnemyState.ACTION;
                    break;
                }
                else
                {
                    activeAgentList[0].targetGO = heroesInBattle[Random.Range(0, heroesInBattle.Count)];
                    enemyControl.currentState = EnemyController.EnemyState.ACTION;
                }
            }
        }
        else if (agent.transform.tag == "Hero")
        {
            HeroController heroControl = agent.GetComponent<HeroController>();
            heroControl.enemyToAttack = activeAgentList[0].targetGO;
            heroControl.currentState = HeroController.HeroState.ACTION;
        }

        actionState = ActionState.PERFORMACTION;
    }

    // Send hero and enemy choices to the active agent list
    public void ActionCollector(TurnOrderHandler _agentInfo)
    {
        activeAgentList.Add(_agentInfo);
    }

    // Add dead characters to the appropriate list and check if there are any left alive
    void CheckForDead()
    {
        if (heroesInBattle.Count <= 0)
        {
            actionState = ActionState.LOSE;
        }
        else if (enemiesInBattle.Count <= 0)
        {

            actionState = ActionState.WIN;
        }
        else
        {
            ClearAttackPanel();
            heroInput = HeroUI.ACTIVATE;
            actionState = ActionState.WAITING;
            deadHeroes.AddRange(GameObject.FindGameObjectsWithTag("DeadHero"));
            deadEnemies.AddRange(GameObject.FindGameObjectsWithTag("DeadEnemy"));
        }
    }

    // Update the heroesInBattle and enemiesInBattle lists
    public void UpdateInBattleLists()
    {
        heroesInBattle.Clear();
        heroesInBattle.AddRange(GameObject.FindGameObjectsWithTag("Hero"));
        enemiesInBattle.Clear();
        enemiesInBattle.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
    }

    // Once the hero has chosen an action, activate the apporpriate target select panel
    public void ActionInput()
    {
        heroChoice.activeAgent = heroesToManage[0].name;
        heroChoice.agentGO = heroesToManage[0];
        heroChoice.chosenAttack = heroesToManage[0].GetComponent<HeroController>().hero.attacks[0];

        if (heroChoice.chosenAttack.partyTargeting)
        {
            heroSelectPanel.SetActive(true);
        }
        else
        {
            enemySelectPanel.SetActive(true);
        }
    }

    // Enter the chosen enemy into the hero's target choice
    public void EnemySelectInput(GameObject chosenEnemy)
    {
        heroChoice.targetGO = chosenEnemy;
        heroInput = HeroUI.DONE;
    }

    // Enter the chosen hero into the hero's target choice
    public void HeroSelectInput(GameObject chosenHero)
    {
        heroChoice.targetGO = chosenHero;
        heroInput = HeroUI.DONE;
    }

    // Enter hero action choice into the active agent list and clean up the battle scene UI
    public void HeroInputDone()
    {
        activeAgentList.Add(heroChoice);
        ClearAttackPanel();

        heroesToManage[0].transform.FindChild("Selector").gameObject.SetActive(false);
        heroesToManage.RemoveAt(0);
        heroInput = HeroUI.ACTIVATE;
    }

    // Deactivate all attack panels
    void ResetAttackPanels()
    {
        enemySelectPanel.SetActive(false);
        heroSelectPanel.SetActive(false);
        earthPanel.SetActive(false);
        firePanel.SetActive(false);
        waterPanel.SetActive(false);
        attackPanel.SetActive(false);
        utilityPanel.SetActive(false);
        failedActionPanel.SetActive(false);
    }

    // Deactivate all attack panels and destroy any instantiated buttons
    void ClearAttackPanel()
    {
        enemySelectPanel.SetActive(false);
        heroSelectPanel.SetActive(false);
        actionPanel.SetActive(false);
        earthPanel.SetActive(false);
        firePanel.SetActive(false);
        waterPanel.SetActive(false);
        attackPanel.SetActive(false);
        utilityPanel.SetActive(false);
        failedActionPanel.SetActive(false);

        foreach (GameObject attackButton in attackButtons)
        {
            Destroy(attackButton);
        }
        attackButtons.Clear();
    }

    // Flags which hero is active to start the correct hero panel animations
    void ActivateHeroPanels()
    {
        if (activeAgentList[0].agentGO.GetComponent<HeroController>().name == "Gabi")
        {
            gabiTurn = true;
        }
        else if (activeAgentList[0].agentGO.GetComponent<HeroController>().name == "Quinn")
        {
            quinnTurn = true;
        }
        else if (activeAgentList[0].agentGO.GetComponent<HeroController>().name == "Arvandus")
        {
            arvandusTurn = true;
        }
    }

    // Animate hero panels to active position when a hero's turn is active
    void ExpandHeroPanels()
    {
        if (!expandTriggered)
        {
            if (gabiTurn)
            {
                animPanelGabi.SetTrigger("expand");
                expandTriggered = true;
            }
            else if (quinnTurn)
            {
                animPanelGabi.SetTrigger("gabiUp");
                animPanelQuinn.SetTrigger("expand");
                expandTriggered = true;
            }
            else if (arvandusTurn)
            {
                animPanelGabi.SetTrigger("gabiUp");
                animPanelQuinn.SetTrigger("quinnUp");
                animPanelArvandus.SetTrigger("expand");
                expandTriggered = true;
            }
            contractTriggered = false;
        }
    }

    // Animate hero panels to starting position when a hero's turn is active
    void ContractHeroPanels()
    {
        if (!contractTriggered)
        {
            if (gabiTurn)
            {
                animPanelGabi.SetTrigger("minimize");
                contractTriggered = true;
                gabiTurn = false;
            }
            else if (quinnTurn)
            {
                animPanelGabi.SetTrigger("gabiDown");
                animPanelQuinn.SetTrigger("minimize");
                contractTriggered = true;
                quinnTurn = false;
            }
            else if (arvandusTurn)
            {
                animPanelGabi.SetTrigger("gabiDown");
                animPanelQuinn.SetTrigger("quinnDown");
                animPanelArvandus.SetTrigger("minimize");
                contractTriggered = true;
                arvandusTurn = false;
            }

            heroPanelActive = false;
            expandTriggered = false;
        }
    }
    
    // Coroutine for announcing lack of resources for selected action
    // ***** Not currently active in build *****
    public IEnumerator FailedActionNotification(string _resourceType)
    {
        Text failedActionText = failedActionPanel.transform.FindChild("text").gameObject.GetComponent<Text>();
        failedActionText.text = _resourceType;
        failedActionPanel.SetActive(true);
        yield return new WaitForSeconds(3);

        failedActionPanel.SetActive(false);
        yield return null;
    }

    // Create action buttons
    void CreateActionButtons()
    {
        // Create attack buttons
        if (heroesToManage[0].GetComponent<HeroController>().hero.attacks.Count > 0)
        {
            // Attack Button
            GameObject attackButton = Instantiate(actionButton) as GameObject;
            Text attackButtonText = attackButton.transform.FindChild("Text").gameObject.GetComponent<Text>();
            attackButtonText.text = "Attack";
            attackButton.GetComponent<Button>().onClick.AddListener(() => AttackInput());
            attackButton.transform.SetParent(actionSpacer, false);
            attackButtons.Add(attackButton);

            foreach (AttackData attack in heroesToManage[0].GetComponent<HeroController>().hero.attacks)
            {
                GameObject meleeBtn = Instantiate(meleeButton) as GameObject;
                Text attackTypeButtonText = meleeBtn.transform.FindChild("Text").gameObject.GetComponent<Text>();
                attackTypeButtonText.text = attack.attackName;
                MeleeAttackButton meleeAttackButton = meleeBtn.GetComponent<MeleeAttackButton>();
                meleeAttackButton.meleeAttack = attack;
                meleeBtn.transform.SetParent(attackSpacer, false);
                attackButtons.Add(meleeBtn);
            }
        }

        // Create fire spell buttons
        if (heroesToManage[0].GetComponent<HeroController>().hero.fireSpells.Count > 0)
        {
            // Magic Button
            GameObject magicButton = Instantiate(actionButton) as GameObject;
            Text magicButtonText = magicButton.transform.FindChild("Text").gameObject.GetComponent<Text>();
            magicButtonText.text = "Fire Spells";
            magicButton.GetComponent<Button>().onClick.AddListener(() => MagicInput("Fire"));
            magicButton.transform.SetParent(actionSpacer, false);
            attackButtons.Add(magicButton);

            // Spell Buttons
            foreach (AttackData magicAttack in heroesToManage[0].GetComponent<HeroController>().hero.fireSpells)
            {
                GameObject spellBtn = Instantiate(spellButton) as GameObject;
                Text spellButtonText = spellBtn.transform.FindChild("Text").gameObject.GetComponent<Text>();
                spellButtonText.text = magicAttack.attackName;
                SpellCastButton spellCastButton = spellBtn.GetComponent<SpellCastButton>();
                spellCastButton.spellToCast = magicAttack;
                spellBtn.transform.SetParent(fireSpacer, false);
                attackButtons.Add(spellBtn);
            }
        }

        // Create water spell buttons
        if (heroesToManage[0].GetComponent<HeroController>().hero.waterSpells.Count > 0)
        {
            // Magic Button
            GameObject magicButton = Instantiate(actionButton) as GameObject;
            Text magicButtonText = magicButton.transform.FindChild("Text").gameObject.GetComponent<Text>();
            magicButtonText.text = "Water Spells";
            magicButton.GetComponent<Button>().onClick.AddListener(() => MagicInput("Water"));
            magicButton.transform.SetParent(actionSpacer, false);
            attackButtons.Add(magicButton);

            // Spell Buttons
            foreach (AttackData magicAttack in heroesToManage[0].GetComponent<HeroController>().hero.waterSpells)
            {
                GameObject spellBtn = Instantiate(spellButton) as GameObject;
                Text spellButtonText = spellBtn.transform.FindChild("Text").gameObject.GetComponent<Text>();
                spellButtonText.text = magicAttack.attackName;
                SpellCastButton spellCastButton = spellBtn.GetComponent<SpellCastButton>();
                spellCastButton.spellToCast = magicAttack;
                spellBtn.transform.SetParent(waterSpacer, false);
                attackButtons.Add(spellBtn);
            }
        }

        // Create earth spell buttons
        if (heroesToManage[0].GetComponent<HeroController>().hero.earthSpells.Count > 0)
        {
            // Magic Button
            GameObject magicButton = Instantiate(actionButton) as GameObject;
            Text magicButtonText = magicButton.transform.FindChild("Text").gameObject.GetComponent<Text>();
            magicButtonText.text = "Earth Spells";
            magicButton.GetComponent<Button>().onClick.AddListener(() => MagicInput("Earth"));
            magicButton.transform.SetParent(actionSpacer, false);
            attackButtons.Add(magicButton);

            // Spell Buttons
            foreach (AttackData magicAttack in heroesToManage[0].GetComponent<HeroController>().hero.earthSpells)
            {
                GameObject spellBtn = Instantiate(spellButton) as GameObject;
                Text spellButtonText = spellBtn.transform.FindChild("Text").gameObject.GetComponent<Text>();
                spellButtonText.text = magicAttack.attackName;
                SpellCastButton spellCastButton = spellBtn.GetComponent<SpellCastButton>();
                spellCastButton.spellToCast = magicAttack;
                spellBtn.transform.SetParent(earthSpacer, false);
                attackButtons.Add(spellBtn);
            }
        }

        // Create utility buttons
        if (heroesToManage[0].GetComponent<HeroController>().hero.utility.Count > 0)
        {
            // Magic Button
            GameObject utilButton = Instantiate(actionButton) as GameObject;
            Text utilityButtonText = utilButton.transform.FindChild("Text").gameObject.GetComponent<Text>();
            utilityButtonText.text = "Utility";
            utilButton.GetComponent<Button>().onClick.AddListener(() => UtilityInput());
            utilButton.transform.SetParent(actionSpacer, false);
            attackButtons.Add(utilButton);

            // Utility Buttons
            foreach (AttackData utilityAction in heroesToManage[0].GetComponent<HeroController>().hero.utility)
            {
                GameObject utilityBtn = Instantiate(utilityButton) as GameObject;
                Text utilityActionButtonText = utilityBtn.transform.FindChild("Text").gameObject.GetComponent<Text>();
                utilityActionButtonText.text = utilityAction.attackName;
                UtilityButton utilityActionButton = utilityBtn.GetComponent<UtilityButton>();
                utilityActionButton.utilityToUse = utilityAction;
                utilityBtn.transform.SetParent(utilitySpacer, false);
                attackButtons.Add(utilityBtn);
            }
        }

        // TODO: Create item buttons


        // Create defend button
        if (heroesToManage[0].GetComponent<HeroController>().hero.defend.Count > 0)
        {
            GameObject defendButton = Instantiate(actionButton) as GameObject;
            Text defendButtonText = defendButton.transform.FindChild("Text").gameObject.GetComponent<Text>();
            defendButtonText.text = "Defend";
            defendButton.GetComponent<Button>().onClick.AddListener(() => DefendInput());
            defendButton.transform.SetParent(actionSpacer, false);
            attackButtons.Add(defendButton);
        }
    }

    // Open the melee attack panel
    public void AttackInput()
    {
        ResetAttackPanels();
        attackPanel.SetActive(true);
    }

    // Input the selected melee attack
    public void MeleeInput(AttackData chosenAttack)
    {
        heroChoice.activeAgent = heroesToManage[0].name;
        heroChoice.agentGO = heroesToManage[0];
        heroChoice.chosenAttack = chosenAttack;

        enemySelectPanel.SetActive(true);
        enemyButtonsControl();
    }

    // Open the appropriate elemental magic panel
    public void MagicInput(string _magicType)
    {
        ResetAttackPanels();

        if (_magicType == "Fire")
        {
            firePanel.SetActive(true);
        }
        else if (_magicType == "Water")
        {
            waterPanel.SetActive(true);
        }
        else if (_magicType == "Earth")
        {
            earthPanel.SetActive(true);
        }
    }

    // Input the selected spell
    public void SpellInput(AttackData chosenSpell)
    {
        heroChoice.activeAgent = heroesToManage[0].name;
        heroChoice.agentGO = heroesToManage[0];
        heroChoice.chosenAttack = chosenSpell;

        if (heroChoice.chosenAttack.attackType == AttackData.AttackType.RESTORE)
        {
            heroChoice.targetGO = heroesToManage[0];
            heroInput = HeroUI.DONE;
        }
        else if (heroChoice.chosenAttack.partyTargeting)
        {
            heroSelectPanel.SetActive(true);
        }
        else
        {
            enemySelectPanel.SetActive(true);
            enemyButtonsControl();
        }
    }

    // Open the utility action panel
    public void UtilityInput()
    {
        ResetAttackPanels();
        utilityPanel.SetActive(true);
    }

    // Input the selected utility action
    public void SetUtilityInput(AttackData _chosenUtility)
    {
        heroChoice.activeAgent = heroesToManage[0].name;
        heroChoice.agentGO = heroesToManage[0];
        heroChoice.chosenAttack = _chosenUtility;

        //utilityPanel.SetActive(false);
        if (heroChoice.chosenAttack.partyTargeting)
        {
            heroSelectPanel.SetActive(true);
        }
        else
        {
            enemySelectPanel.SetActive(true);
            enemyButtonsControl();
        }
    }

    // Input the defend action
    public void DefendInput()
    {
        AttackData _chosenDefend = heroesToManage[0].GetComponent<HeroController>().hero.defend[0];

        heroChoice.activeAgent = heroesToManage[0].name;
        heroChoice.agentGO = heroesToManage[0];
        heroChoice.chosenAttack = _chosenDefend;
        heroChoice.targetGO = heroesToManage[0];
        heroInput = HeroUI.DONE;
    }

    // Activate and deactivate the enemy select button panel as needed
    public void enemyButtonsControl()
    {
        for (int i = 0; i < enemiesInBattle.Count; i++)
        {
            enemiesInBattle[i].GetComponent<EnemyController>().enemyActionControl.EnemyPanelButtonOn();
        }

        for (int i = 0; i < deadEnemies.Count; i++)
        {
            deadEnemies[i].GetComponent<EnemyController>().enemyActionControl.EnemyPanelButtonOff();
        }
    }

    // Put the heroes into their idle animation and display the victory panel
    void WinBattle()
    {
        for (int i = 0; i < heroesInBattle.Count; i++)
        {
            heroesInBattle[i].GetComponent<HeroController>().currentState = HeroController.HeroState.IDLE;
        }   

        victoryPanel.SetActive(true);
    }

    // Put the enemies into their idle animation and display the defeat panel
    void LoseBattle()
    {
        for (int i = 0; i < enemiesInBattle.Count; i++)
        {
            enemiesInBattle[i].GetComponent<EnemyController>().currentState = EnemyController.EnemyState.IDLE;
        }

        defeatPanel.SetActive(true);
    }
}

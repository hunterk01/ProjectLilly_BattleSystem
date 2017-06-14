using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HeroController : MonoBehaviour
{
    protected CameraController cameraControl;
    private BattleController battleControl;
    public BaseHero hero;

    public IHeroActionControl heroActionControl;

    // Hero state machine
    public enum HeroState
    {
        WAITING,    // Waiting for ATB bar to fill
        ACTIONLIST,  // Add hero to list
        IDLE,       // Make hero idle in between actions
        SELECT,     // Hero is choosing action
        ACTION,     // Process hero actions
        DEAD        // Hero is dead, waiting for things to happen
    }
    public HeroState currentState;

    // Variables for handling UI info bars
    private float ATB_Timer = 0;
    private float ATB_MaxDelay = 5;
    private float ResourceBarTimer = 0;
    private float ResourceBarMaxDelay = 1;
    private float barSpeed = 45;
    private float newHealth;
    private float newEnergy;
    private Image ATB_Bar;
    private Image HP_Bar;
    private Image Resource_Bar;

    public GameObject selector;
    public GameObject enemyToAttack;

    public bool isAlive = true;
    public bool canDefend;
    public bool isBlocking = false;

    // Hero panel variables
    private HeroPanelInfo panelInfo;
    public GameObject heroPanel;
    private Transform heroPanelSpacer;

    // Weapon draw delay timer
    private float weaponDrawDelay = .65f;
    private float weaponDrawTimer;

    private bool battleCameraSet = false;

    void Start()
    {
        // Create panel and add info
        heroPanelSpacer = GameObject.Find("BattleCanvas").transform.FindChild("HeroPanel").transform.FindChild("HeroPanelSpacer");
        CreateHeroPanel();
        //UpdateHeroPanel();

        ATB_Timer = Random.Range(0, 2.5f);
        weaponDrawTimer = weaponDrawDelay;
        currentState = HeroState.WAITING;
        battleControl = GameObject.Find("BattleManager").GetComponent<BattleController>();
        cameraControl = GameObject.Find("MainCamera").GetComponent<CameraController>();

        selector.SetActive(false);
        heroActionControl = gameObject.GetComponent<IHeroActionControl>();

        heroActionControl.HeroAwake();
        InitializeHeroPanel();
    }

    void Update()
    {
        if (battleControl.startBattle) CheckState();

        if (weaponDrawTimer <= 0)
        {
            heroActionControl.DrawWeapon();
        }
        else
        {
            weaponDrawTimer -= Time.deltaTime;
        }

    }

    void CheckState()
    {
        //Debug.Log(currentState);

        switch (currentState)
        {
            case (HeroState.WAITING):
                UpdateATB();
                break;
            case (HeroState.ACTIONLIST):
                AddToActionList();
                break;
            case (HeroState.IDLE):
                
                break;
            case (HeroState.SELECT):

                break;
            case (HeroState.ACTION):
                PerformAction();
                break;
            case (HeroState.DEAD):
                HeroDeath();
                break;
        }
    }


    // Increase active time battlesystem meter and trigger hero action control when full
    void UpdateATB()
    {
        if (ATB_Timer >= ATB_MaxDelay)
        {
            currentState = HeroState.ACTIONLIST;
        }
        else
        {
            // TODO: Add Speed stat to ATB fill rate
            // TODO: Add ambush condition

            ATB_Timer += Time.deltaTime;
            float ATB_FillPercentage = ATB_Timer / ATB_MaxDelay;
            ATB_Bar.transform.localScale = new Vector3(Mathf.Clamp(ATB_FillPercentage, 0, 1), ATB_Bar.transform.localScale.y, ATB_Bar.transform.localScale.z);
        }
    }

    void AddToActionList()
    {
        battleControl.heroesToManage.Add(this.gameObject);
        currentState = HeroState.IDLE;
    }

    // Set the target position (enemy / party / self) and perform action
    private void PerformAction()
    {
        // Set battle camera type
        //if (!battleCameraSet)
        //{
        //    cameraControl.BattleCamInput(transform, enemyToAttack.transform, 1);
        //    battleCameraSet = true;
        //}

        Vector3 targetPosition;

        if (battleControl.activeAgentList[0].chosenAttack.attackType == AttackData.AttackType.RESTORE)
        {
            targetPosition = transform.position;
            heroActionControl.RestoreInput(battleControl.activeAgentList[0].chosenAttack, targetPosition);
        }
        else if (battleControl.activeAgentList[0].chosenAttack.attackType == AttackData.AttackType.HEAL || battleControl.activeAgentList[0].chosenAttack.attackType == AttackData.AttackType.BUFF || 
                 battleControl.activeAgentList[0].chosenAttack.attackType == AttackData.AttackType.DEBUFF)
        {
            targetPosition = new Vector3(enemyToAttack.transform.position.x, transform.position.y, enemyToAttack.transform.position.z);
            heroActionControl.AttackInput(battleControl.activeAgentList[0].chosenAttack, targetPosition);
        }
        else
        {
            targetPosition = new Vector3(enemyToAttack.transform.position.x - 3.5f, transform.position.y, enemyToAttack.transform.position.z);
            heroActionControl.AttackInput(battleControl.activeAgentList[0].chosenAttack, targetPosition);
        }
    }

    // Perform wrapup actions and reset ATB after the hero has performed an action
    public void EndAction()
    {
        // Handle resource management
        ApplyActionCost();

        // Remove from the active agent list
        battleControl.activeAgentList.RemoveAt(0);

        if (battleControl.actionState != BattleController.ActionState.WIN && battleControl.actionState != BattleController.ActionState.LOSE)
        {
            // Reset the battle controller to WAIT
            battleControl.actionState = BattleController.ActionState.WAITING;

            // Reset hero state
            ATB_Timer = 0;
            currentState = HeroState.WAITING;
        }
        else
        {
            currentState = HeroState.IDLE;
        }

        cameraControl.BattleCamReset();
        battleCameraSet = false;
    }

    // Perform wrapup after a simple hero action
    public void EndSimpleAction()
    {
        battleControl.HeroInputDone();

        // Reset the battle controller to WAIT
        battleControl.actionState = BattleController.ActionState.WAITING;
    }

    // Get action cost from hero's chosen action and updtate the appropriate meter
    public void ApplyActionCost()
    {
        if (battleControl.activeAgentList[0].chosenAttack.chargeCost > 0)
        {
            if (battleControl.activeAgentList[0].chosenAttack.damageType == AttackData.DamageType.FIRE)
            {
                hero.CurrentFireCharges -= battleControl.activeAgentList[0].chosenAttack.chargeCost;

                if (hero.CurrentFireCharges < 0)
                {
                    hero.CurrentFireCharges = 0;
                }
            }
            else if (battleControl.activeAgentList[0].chosenAttack.damageType == AttackData.DamageType.WATER)
            {
                hero.CurrentWaterCharges -= battleControl.activeAgentList[0].chosenAttack.chargeCost;

                if (hero.CurrentWaterCharges < 0)
                {
                    hero.CurrentWaterCharges = 0;
                }
            }
            else if (battleControl.activeAgentList[0].chosenAttack.damageType == AttackData.DamageType.EARTH)
            {
                hero.CurrentEarthCharges -= battleControl.activeAgentList[0].chosenAttack.chargeCost;

                if (hero.CurrentEarthCharges < 0)
                {
                    hero.CurrentEarthCharges = 0;
                }
            }
        }
        else
        {
            hero.CurrentEnergy -= battleControl.activeAgentList[0].chosenAttack.energyCost;

            if (hero.CurrentEnergy < 0)
            {
                hero.CurrentEnergy = 0;
            }
        }

        UpdateHeroPanel();
    }

    // Update the current health value and meter
    public void TakeDamage(int _damage)
    {
        float newHealth;

        if (isBlocking)
        {
            newHealth = hero.CurrentHealth - _damage / 2;
        }
        else
        {
            newHealth = hero.CurrentHealth - _damage;
        }

        // Play hit animation
        heroActionControl.HitReaction();

        if (newHealth <= 0)
        {
            newHealth = 0;
            currentState = HeroState.DEAD;
        }

        StartCoroutine(LowerHealthBar(newHealth));
    }

    // Update the current health value and UI meter
    public void TakeHealing(int _healing)
    {
        float newHealth = hero.CurrentHealth + _healing;
        
        if (newHealth > hero.baseHealth)
        {
            newHealth = hero.baseHealth;
        }

        StartCoroutine(RaiseHealthBar(newHealth));
    }

    // Update the corruption/element value and UI meter
    public void DoCleansing()
    {
        int calculatedDamage = hero.CurrentAttackPower + battleControl.activeAgentList[0].chosenAttack.attackDamage;
        enemyToAttack.GetComponent<EnemyController>().TakeCleansing(calculatedDamage);
    }

    // Send chosen attack damage to the target so they can process taken damage
    public void DoDamage()
    {
        int calculatedDamage = hero.CurrentAttackPower + battleControl.activeAgentList[0].chosenAttack.attackDamage;
        enemyToAttack.GetComponent<EnemyController>().TakeDamage(calculatedDamage);
    }

    // Send chosen heal action value to the target so they can process the healing done
    public void DoHealing()
    {
        enemyToAttack.GetComponent<HeroController>().TakeHealing(battleControl.activeAgentList[0].chosenAttack.healthChange);
    }

    // Peform hero death clean up and play the death animation
    void HeroDeath()
    {
        if (!isAlive)
        {
            return;
        }
        else
        {
            // Change tag and remove gameObject from appropriate lists
            this.gameObject.tag = "DeadHero";
            battleControl.heroesInBattle.Remove(this.gameObject);
            battleControl.heroesToManage.Remove(this.gameObject);

            // Set selector, action panel, and enemy select panel to inactive
            selector.SetActive(false);
            battleControl.actionPanel.SetActive(false);
            battleControl.enemySelectPanel.SetActive(false);

            // Remove from active agent list
            if (battleControl.heroesInBattle.Count > 0)
            {
                for (int i = 0; i < battleControl.activeAgentList.Count; i++)
                {
                    if (i != 0)
                    {
                        if (battleControl.activeAgentList[i].agentGO == this.gameObject)
                        {
                            battleControl.activeAgentList.Remove(battleControl.activeAgentList[i]);
                        }

                        if (battleControl.activeAgentList[i].targetGO == this.gameObject)
                        {
                            battleControl.activeAgentList[i].targetGO = battleControl.heroesInBattle[Random.Range(0, battleControl.heroesInBattle.Count)];
                        }
                    }
                }
            }

            // Trigger death animation
            heroActionControl.DeathReaction();

            // Reset hero input
            battleControl.actionState = BattleController.ActionState.CHECKFORDEAD;

            isAlive = false;
        }
    }

    // At the end of battle, bring dead heroes back to life with one hit point
    public void EndBattleRevive()
    {
        hero.CurrentHealth = 1;
        this.gameObject.tag = "Hero";
        battleControl.heroesInBattle.Add(this.gameObject);
    }

    // Initial setup of the hero UI panel info
    void CreateHeroPanel()
    {
        panelInfo = heroPanel.GetComponent<HeroPanelInfo>();

        // Add info to hero panel
        panelInfo.heroName.text = name;
        panelInfo.heroHP.text = "HP: " + hero.CurrentHealth + " / " + hero.baseHealth;

        ATB_Bar = panelInfo.ATB_Bar;
        HP_Bar = panelInfo.HP_Bar;
        Resource_Bar = panelInfo.Resource_Bar;
    }
    
    // Initial setup of hero panel values and bars
    public void InitializeHeroPanel()
    {
        // Setup HP bar and text
        float HP_FillPercentage = hero.CurrentHealth / hero.baseHealth;
        HP_Bar.transform.localScale = new Vector3(Mathf.Clamp(HP_FillPercentage, 0, 1), HP_Bar.transform.localScale.y, HP_Bar.transform.localScale.z);
        panelInfo.heroHP.text = "HP: " + hero.CurrentHealth + " / " + hero.baseHealth;

        // Setup energy bar and text
        if (hero.baseEnergy > 0)
        {
            float Resource_FillPercentage = hero.CurrentEnergy / hero.baseEnergy;
            Resource_Bar.transform.localScale = new Vector3(Mathf.Clamp(Resource_FillPercentage, 0, 1), Resource_Bar.transform.localScale.y, Resource_Bar.transform.localScale.z);
            panelInfo.heroResource.text = "Energy: " + hero.CurrentEnergy + " / " + hero.baseEnergy;
        }

        // Setup elemental charges
        if (hero.maxEarthCharges > 0)
        {
            panelInfo.heroFireCharges.text = "Fire: " + hero.CurrentFireCharges + " / " + hero.maxFireCharges;
            panelInfo.heroWaterCharges.text = "Water: " + hero.CurrentWaterCharges + " / " + hero.maxWaterCharges;
            panelInfo.heroEarthCharges.text = "Earth: " + hero.CurrentEarthCharges + " / " + hero.maxEarthCharges;
        }
    }

    // Update the hero UI panel info
    public void UpdateHeroPanel()
    {
        // Update elemental charges
        if (hero.maxEarthCharges > 0)
        {
            panelInfo.heroFireCharges.text = "Fire: " + hero.CurrentFireCharges + " / " + hero.maxFireCharges;
            panelInfo.heroWaterCharges.text = "Water: " + hero.CurrentWaterCharges + " / " + hero.maxWaterCharges;
            panelInfo.heroEarthCharges.text = "Earth: " + hero.CurrentEarthCharges + " / " + hero.maxEarthCharges;
        }
    }

    // Raise health bar and update text
    private IEnumerator RaiseHealthBar(float _newHealth)
    {
        float Heath_FillPercentage;

        panelInfo.heroHP.text = "HP: " + _newHealth + " / " + hero.baseHealth;

        while (_newHealth > hero.CurrentHealth)
        {
            hero.CurrentHealth += barSpeed * Time.deltaTime;
            Heath_FillPercentage = hero.CurrentHealth / hero.baseHealth;
            HP_Bar.transform.localScale = new Vector3(Mathf.Clamp(Heath_FillPercentage, 0, 1), HP_Bar.transform.localScale.y, HP_Bar.transform.localScale.z);

            yield return null;
        }

        hero.CurrentHealth = _newHealth;
        Heath_FillPercentage = hero.CurrentHealth / hero.baseHealth;
        HP_Bar.transform.localScale = new Vector3(Mathf.Clamp(Heath_FillPercentage, 0, 1), HP_Bar.transform.localScale.y, HP_Bar.transform.localScale.z);

        yield return null;
    }

    // Lower health bar and update text
    private IEnumerator LowerHealthBar(float _newHealth)
    {
        float Heath_FillPercentage;

        panelInfo.heroHP.text = "HP: " + _newHealth + " / " + hero.baseHealth;

        while (_newHealth < hero.CurrentHealth)
        {
            hero.CurrentHealth -= barSpeed * Time.deltaTime;
            Heath_FillPercentage = hero.CurrentHealth / hero.baseHealth;
            HP_Bar.transform.localScale = new Vector3(Mathf.Clamp(Heath_FillPercentage, 0, 1), HP_Bar.transform.localScale.y, HP_Bar.transform.localScale.z);

            yield return null;
        }

        hero.CurrentHealth = _newHealth;
        Heath_FillPercentage = hero.CurrentHealth / hero.baseHealth;
        HP_Bar.transform.localScale = new Vector3(Mathf.Clamp(Heath_FillPercentage, 0, 1), HP_Bar.transform.localScale.y, HP_Bar.transform.localScale.z);

        yield return null;

    }

    // Raise resource bar and update text
    public IEnumerator RaiseResourceBar(float _newEnergy)
    {
        float Energy_FillPercentage;

        panelInfo.heroResource.text = "Energy: " + _newEnergy + " / " + hero.baseEnergy;

        while (_newEnergy > hero.CurrentEnergy)
        {
            hero.CurrentEnergy += barSpeed * Time.deltaTime;
            Energy_FillPercentage = hero.CurrentEnergy / hero.baseEnergy;
            Resource_Bar.transform.localScale = new Vector3(Mathf.Clamp(Energy_FillPercentage, 0, 1), Resource_Bar.transform.localScale.y, Resource_Bar.transform.localScale.z);

            yield return null;
        }

        hero.CurrentEnergy = _newEnergy;
        Energy_FillPercentage = hero.CurrentEnergy / hero.baseEnergy;
        Resource_Bar.transform.localScale = new Vector3(Mathf.Clamp(Energy_FillPercentage, 0, 1), Resource_Bar.transform.localScale.y, Resource_Bar.transform.localScale.z);

        yield return null;
    }

    // Lower resource bar and update text
    public IEnumerator LowerResourceBar(float _newEnergy)
    {
        float Energy_FillPercentage;

        panelInfo.heroResource.text = "Energy: " + _newEnergy + " / " + hero.baseEnergy;

        while (_newEnergy < hero.CurrentEnergy)
        {
            hero.CurrentEnergy -= barSpeed * Time.deltaTime;
            Energy_FillPercentage = hero.CurrentEnergy / hero.baseEnergy;
            Resource_Bar.transform.localScale = new Vector3(Mathf.Clamp(Energy_FillPercentage, 0, 1), Resource_Bar.transform.localScale.y, Resource_Bar.transform.localScale.z);

            yield return null;
        }

        hero.CurrentEnergy = _newEnergy;
        Energy_FillPercentage = hero.CurrentEnergy / hero.baseEnergy;
        Resource_Bar.transform.localScale = new Vector3(Mathf.Clamp(Energy_FillPercentage, 0, 1), Resource_Bar.transform.localScale.y, Resource_Bar.transform.localScale.z);

        yield return null;
    }

    private IEnumerator UpdateChargesUI()
    {
        yield return null;
    }
}
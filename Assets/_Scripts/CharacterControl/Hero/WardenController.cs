using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WardenController : MonoBehaviour, IHeroActionControl
{
    protected Animator animator;
    HeroController heroControl;
    BattleController battleControl;

    // Warden stance
    public enum Stance
    {
        FIRE, WATER, EARTH, WOOD, METAL
    }
    public Stance stance;

    // Variables for weapon draw delay
    private float weaponDrawTimer = 0.0f;
    private float weaponDrawDelay = .75f;

    // Variables for performing timed actions
    private Vector3 startPosition;
    private float moveSpeed = 20;
    private bool actionStarted = false;

    private bool isAlive = true;
    public bool isBlocking = false;

    // Weapon Select
    public enum Weapon
    {
        UNARMED = 0,
        TWOHANDSWORD = 1,
        TWOHANDSPEAR = 2,
        TWOHANDAXE = 3,
        TWOHANDBOW = 4,
        TWOHANDCROSSBOW = 5,
        STAFF = 6,
        ARMED = 7,
        RELAX = 8,
        RIFLE = 9
    }

    public Weapon weapon;

    private int rightWeaponType = 1;
    private int leftWeaponType = 0;
    int rightWeapon = 0;
    int leftWeapon = 0;

    // Weapon Model
    public GameObject twohandsword;

    public void HeroAwake()
    {
        animator = GetComponentInChildren<Animator>();
        heroControl = GetComponent<HeroController>();
        battleControl = GameObject.Find("BattleManager").GetComponent<BattleController>();
        startPosition = transform.position;
        heroControl.hero.baseEnergy = 100;
        heroControl.hero.CurrentEnergy = heroControl.hero.baseEnergy;

        InitilizeStats();

        // Hide weapon
        if (twohandsword != null)
        {
            twohandsword.SetActive(false);
        }
    }

    void InitilizeStats()
    {
        heroControl.hero.baseHealth = 960;
        heroControl.hero.baseEnergy = 100;

        heroControl.hero.CurrentHealth = heroControl.hero.baseHealth;
        heroControl.hero.CurrentEnergy = heroControl.hero.baseEnergy;
    }

    // At the start of the battle, check the weapon type and select correct weapon draw animation coroutine
    public void DrawWeapon()
    {
        if ((rightWeaponType > 7 && rightWeaponType < 17))
            if (leftWeapon != leftWeaponType)
            {
                StartCoroutine(_SwitchWeapon(leftWeaponType));
                leftWeapon = leftWeaponType;
            }
        if (rightWeapon != rightWeaponType)
        {
            StartCoroutine(_SwitchWeapon(rightWeaponType));
            rightWeapon = rightWeaponType;
        }
    }

    // Turns on/off weapon trail effect
    void WeaponEffect(bool _trailOn)
    {
        GameObject activeTrail;

        if (twohandsword.activeSelf == true)
        {
            activeTrail = twohandsword.transform.FindChild("Trail").gameObject;
            if (_trailOn)
                activeTrail.SetActive(true);
            else
                activeTrail.SetActive(false);
        }
    }

    // Receive attack data and choose appropriate coroutine
    public void AttackInput(AttackData _chosenAttack, Vector3 _targetPosition)
    {
        //animator.SetBool("Blocking", false);

        if (_chosenAttack.attackType == AttackData.AttackType.MELEE || _chosenAttack.attackType == AttackData.AttackType.CLEANSE)
        {
            animator.SetBool("Blocking", false);
            heroControl.isBlocking = false;
            StartCoroutine(PerformMeleeAttack(_chosenAttack, _targetPosition));
        }
        else if (_chosenAttack.attackType == AttackData.AttackType.BUFF || _chosenAttack.attackType == AttackData.AttackType.DEBUFF)
        {
            StartCoroutine(PerformUtility(_chosenAttack, _targetPosition));
        }
        else if (_chosenAttack.attackType == AttackData.AttackType.DEFEND)
        {
            StartCoroutine(PerformDefend(_chosenAttack, _targetPosition));
        }
    }

    public void RestoreInput(AttackData _chosenAttack, Vector3 _tagetPositon)
    {

    }

    // Play hit reaction animation.  If it's a melee attack add resolve value.
    public void HitReaction()
    {
        if (animator.GetBool("Blocking"))
        {
            int hits = 2;
            int hitNumber = Random.Range(0, hits);

            if (hitNumber == 0)
                animator.SetTrigger("BlockGetHit1Trigger");
            else if (hitNumber == 1)
                animator.SetTrigger("BlockGetHit2Trigger");
        }
        else
        {
            int hits = 5;
            int hitNumber = Random.Range(0, hits);
            animator.SetTrigger("GetHit" + (hitNumber + 1).ToString() + "Trigger");
        }

        // Add resolve gain on hit
        if (battleControl.activeAgentList[0].chosenAttack.attackType == AttackData.AttackType.MELEE)
        {
            AddResolve(15);
        }
    }

    public void DeathReaction()
    {
        animator.SetTrigger("Death1Trigger");
    }

    // Coroutine for handling melee attacks
    private IEnumerator PerformMeleeAttack(AttackData _chosenAttack, Vector3 _targetPosition)
    {
        if (actionStarted)
        {
            yield break;
        }

        actionStarted = true;

        // Move enemy to target
        while (MoveTowardTarget(_targetPosition))
        {
            animator.SetBool("Moving", true);
            animator.SetFloat("Velocity Z", moveSpeed);

            yield return null;
        }

        animator.SetBool("Moving", false);

        animator.SetTrigger(_chosenAttack.attackAnimation);

        yield return new WaitForSeconds(_chosenAttack.damageWaitTime);

        if (_chosenAttack.attackType == AttackData.AttackType.CLEANSE)
        {
            if (CheckTerrainElementParity()) heroControl.DoCleansing();
            else heroControl.DoDamage();
        }
        else
            heroControl.DoDamage();

        yield return new WaitForSeconds(.5f);

        LowerResolve(_chosenAttack.energyCost);

        animator.SetInteger("Jumping", 1);
        animator.SetTrigger("JumpTrigger");

        // Move enemy back to starting position
        while (MoveTowardStart(startPosition))
        {

            // Setup jump animation
            animator.SetInteger("Jumping", 2);
            animator.SetTrigger("JumpTrigger");

            yield return null;
        }

        animator.SetInteger("Jumping", 0);

        actionStarted = false;

        heroControl.EndAction();
    }

    // Coroutine for handling untility actions such as buff and debuff
    private IEnumerator PerformUtility(AttackData _chosenAttack, Vector3 _targetPosition)
    {
        if (actionStarted)
        {
            yield break;
        }

        actionStarted = true;

        animator.SetTrigger(_chosenAttack.attackAnimation);

        yield return new WaitForSeconds(_chosenAttack.attackWaitTime);

        if (!_chosenAttack.partyTargeting)
        {
            Vector3 targetHieghtOffset = new Vector3(0, 2.2f, 0);
            _targetPosition += targetHieghtOffset;
        }

        Quaternion spellRotation = Quaternion.LookRotation(_targetPosition);
        GameObject tempSpell = Instantiate(_chosenAttack.projectile, _targetPosition, spellRotation) as GameObject;

        yield return new WaitForSeconds(_chosenAttack.damageWaitTime);

        Destroy(tempSpell);

        actionStarted = false;
        heroControl.EndAction();
    }

    // Coroutine for handling defend actions
    private IEnumerator PerformDefend(AttackData _chosenAttack, Vector3 _targetPosition)
    {
        if (actionStarted)
        {
            yield break;
        }

        actionStarted = true;

        // Set defend animation
        animator.SetBool("Blocking", true);
        animator.SetTrigger("BlockTrigger");

        yield return new WaitForSeconds(.5f);

        AddResolve(battleControl.activeAgentList[0].chosenAttack.energyRestore);

        actionStarted = false;
        heroControl.EndAction();
    }

    // Used to check if the attack element matches the enemy element
    private bool CheckTerrainElementParity()
    {
        string primaryElement = battleControl.terrainElementPrimary.ToString();
        string secondaryElement = battleControl.terrainElementSecondary.ToString();
        string enemyElement = battleControl.activeAgentList[0].targetGO.GetComponent<EnemyController>().enemy.enemyType.ToString();

        if (primaryElement == enemyElement || secondaryElement == enemyElement) return true;
        else return false;
    }

    // Add resolve based on damage taken from enemy attack
    public void AddResolve(int _resolveGain)
    {
        float newResolve = heroControl.hero.CurrentEnergy + _resolveGain;

        if (newResolve > heroControl.hero.baseEnergy)
        {
            newResolve = heroControl.hero.baseEnergy;
        }

        StartCoroutine(heroControl.RaiseResourceBar(newResolve));
    }

    public void LowerResolve(float _resolveLoss)
    {
        float newResolve = heroControl.hero.CurrentEnergy - _resolveLoss;

        if (newResolve < 0)
        {
            newResolve = 0;
        }

        StartCoroutine(heroControl.LowerResourceBar(newResolve));
    }

    // Move hero toward melee target
    private bool MoveTowardTarget(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime));
    }

    // Move hero toward starting position
    private bool MoveTowardStart(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, (moveSpeed * 1.25f) * Time.deltaTime));
    }

    // Play death animation
    public void HeroDeathAnim()
    {
        animator.SetTrigger("Death1Trigger");
    }

    #region _WeaponCoroutines

    //for two-hand weapon switching
    void SwitchWeaponTwoHand(int upDown)
    {
        int weaponSwitch = (int)weapon;
        if (upDown == 0)
        {
            weaponSwitch--;
            if (weaponSwitch < 1)
            {
                StartCoroutine(_SwitchWeapon(6));
            }
            else
            {
                StartCoroutine(_SwitchWeapon(weaponSwitch));
            }
        }
        if (upDown == 1)
        {
            weaponSwitch++;
            if (weaponSwitch > 6)
            {
                StartCoroutine(_SwitchWeapon(1));
            }
            else
            {
                StartCoroutine(_SwitchWeapon(weaponSwitch));
            }
        }
    }

    //for one-hand weapon switching
    void SwitchWeaponLeftRight(int upDown)
    {
        int weaponSwitch = 0;
        if (upDown == 0)
        {
            weaponSwitch = leftWeapon;
            if (weaponSwitch < 16 && weaponSwitch != 0 && leftWeapon != 7)
            {
                weaponSwitch += 2;
            }
            else
            {
                weaponSwitch = 8;
            }
        }
        if (upDown == 1)
        {
            weaponSwitch = rightWeapon;
            if (weaponSwitch < 17 && weaponSwitch != 0)
            {
                weaponSwitch += 2;
            }
            else
            {
                weaponSwitch = 9;
            }
        }
        StartCoroutine(_SwitchWeapon(weaponSwitch));
    }

    //function to switch weapons
    //weaponNumber 0 = Unarmed
    //weaponNumber 1 = 2H Sword
    //weaponNumber 2 = 2H Spear
    //weaponNumber 3 = 2H Axe
    //weaponNumber 4 = 2H Bow
    //weaponNumber 5 = 2H Crowwbow
    //weaponNumber 6 = 2H Staff
    //weaponNumber 7 = Shield
    //weaponNumber 8 = L Sword
    //weaponNumber 9 = R Sword
    //weaponNumber 10 = L Mace
    //weaponNumber 11 = R Mace
    //weaponNumber 12 = L Dagger
    //weaponNumber 13 = R Dagger
    //weaponNumber 14 = L Item
    //weaponNumber 15 = R Item
    //weaponNumber 16 = L Pistol
    //weaponNumber 17 = R Pistol
    //weaponNumber 18 = Rifle
    public IEnumerator _SwitchWeapon(int weaponNumber)
    {
        //character is unarmed
        if (weapon == Weapon.UNARMED)
        {
            StartCoroutine(_UnSheathWeapon(weaponNumber));
        }
        //character has 2 handed weapon
        else if (weapon == Weapon.STAFF || weapon == Weapon.TWOHANDAXE || weapon == Weapon.TWOHANDBOW || weapon == Weapon.TWOHANDCROSSBOW || weapon == Weapon.TWOHANDSPEAR || weapon == Weapon.TWOHANDSWORD || weapon == Weapon.RIFLE)
        {
            StartCoroutine(_SheathWeapon(leftWeapon, weaponNumber));
            yield return new WaitForSeconds(1.1f);
            if (weaponNumber > 0)
            {
                StartCoroutine(_UnSheathWeapon(weaponNumber));
            }
            //switch to unarmed
            else
            {
                weapon = Weapon.UNARMED;
                animator.SetInteger("Weapon", 0);
            }
        }
        //character has 1 or 2 1hand weapons and/or shield
        else if (weapon == Weapon.ARMED)
        {
            //character is switching to 2 hand weapon or unarmed, put put away all weapons
            if (weaponNumber < 7 || weaponNumber > 17)
            {
                //check left hand for weapon
                if (leftWeapon != 0)
                {
                    StartCoroutine(_SheathWeapon(leftWeapon, weaponNumber));
                    yield return new WaitForSeconds(1.05f);
                    if (rightWeapon != 0)
                    {
                        StartCoroutine(_SheathWeapon(rightWeapon, weaponNumber));
                        yield return new WaitForSeconds(1.05f);
                        //and right hand weapon
                        if (weaponNumber != 0)
                        {
                            StartCoroutine(_UnSheathWeapon(weaponNumber));
                        }
                    }
                    if (weaponNumber != 0)
                    {
                        StartCoroutine(_UnSheathWeapon(weaponNumber));
                    }
                }
                //check right hand for weapon if no left hand weapon
                if (rightWeapon != 0)
                {
                    StartCoroutine(_SheathWeapon(rightWeapon, weaponNumber));
                    yield return new WaitForSeconds(1.05f);
                    if (weaponNumber != 0)
                    {
                        StartCoroutine(_UnSheathWeapon(weaponNumber));
                    }
                }
            }
            //using 1 handed weapon(s)
            else if (weaponNumber == 7)
            {
                if (leftWeapon > 0)
                {
                    StartCoroutine(_SheathWeapon(leftWeapon, weaponNumber));
                    yield return new WaitForSeconds(1.05f);
                }
                StartCoroutine(_UnSheathWeapon(weaponNumber));
            }
            //switching left weapon, put away left weapon if equipped
            else if ((weaponNumber == 8 || weaponNumber == 10 || weaponNumber == 12 || weaponNumber == 14 || weaponNumber == 16))
            {
                if (leftWeapon > 0)
                {
                    StartCoroutine(_SheathWeapon(leftWeapon, weaponNumber));
                    yield return new WaitForSeconds(1.05f);
                }
                StartCoroutine(_UnSheathWeapon(weaponNumber));
            }
            //switching right weapon, put away right weapon if equipped
            else if ((weaponNumber == 9 || weaponNumber == 11 || weaponNumber == 13 || weaponNumber == 15 || weaponNumber == 17))
            {
                if (rightWeapon > 0)
                {
                    StartCoroutine(_SheathWeapon(rightWeapon, weaponNumber));
                    yield return new WaitForSeconds(1.05f);
                }
                StartCoroutine(_UnSheathWeapon(weaponNumber));
            }
        }
        yield return null;
    }

    public IEnumerator _SheathWeapon(int weaponNumber, int weaponDraw)
    {
        if ((weaponNumber == 8 || weaponNumber == 10 || weaponNumber == 12 || weaponNumber == 14 || weaponNumber == 16))
        {
            animator.SetInteger("LeftRight", 1);
        }
        else if ((weaponNumber == 9 || weaponNumber == 11 || weaponNumber == 13 || weaponNumber == 15 || weaponNumber == 17))
        {
            animator.SetInteger("LeftRight", 2);
        }
        if (weaponDraw == 0)
        {
            //if switching to unarmed, don't set "Armed" until after 2nd weapon sheath
            if (leftWeapon == 0 && rightWeapon != 0)
            {
                animator.SetBool("Armed", false);
            }
            if (rightWeapon == 0 && leftWeapon != 0)
            {
                animator.SetBool("Armed", false);
            }
        }
        animator.SetTrigger("WeaponSheathTrigger");
        yield return new WaitForSeconds(.1f);
        if (weaponNumber < 7 || weaponNumber == 18)
        {
            leftWeapon = 0;
            animator.SetInteger("LeftWeapon", 0);
            rightWeapon = 0;
            animator.SetInteger("RightWeapon", 0);
            animator.SetBool("Shield", false);
            animator.SetBool("Armed", false);
        }
        else if (weaponNumber == 7)
        {
            leftWeapon = 0;
            animator.SetInteger("LeftWeapon", 0);
            animator.SetBool("Shield", false);
        }
        else if ((weaponNumber == 8 || weaponNumber == 10 || weaponNumber == 12 || weaponNumber == 14 || weaponNumber == 16))
        {
            leftWeapon = 0;
            animator.SetInteger("LeftWeapon", 0);
        }
        else if ((weaponNumber == 9 || weaponNumber == 11 || weaponNumber == 13 || weaponNumber == 15 || weaponNumber == 17))
        {
            rightWeapon = 0;
            animator.SetInteger("RightWeapon", 0);
        }
        //if switched to unarmed
        if (leftWeapon == 0 && rightWeapon == 0)
        {
            animator.SetBool("Armed", false);
        }
        if (leftWeapon == 0 && rightWeapon == 0)
        {
            animator.SetInteger("LeftRight", 0);
            animator.SetInteger("Weapon", 0);
            animator.SetBool("Armed", false);
            weapon = Weapon.UNARMED;
        }
        StartCoroutine(_WeaponVisibility(weaponNumber, .4f, false));
        yield return null;
    }

    public IEnumerator _UnSheathWeapon(int weaponNumber)
    {
        animator.SetInteger("Weapon", -1);
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        //two handed weapons
        if (weaponNumber < 7 || weaponNumber == 18)
        {
            leftWeapon = weaponNumber;
            animator.SetInteger("LeftRight", 3);
            if (weaponNumber == 0)
            {
                weapon = Weapon.UNARMED;
            }
            if (weaponNumber == 1)
            {
                weapon = Weapon.TWOHANDSWORD;
                StartCoroutine(_WeaponVisibility(weaponNumber, .4f, true));
            }
            else if (weaponNumber == 2)
            {
                weapon = Weapon.TWOHANDSPEAR;
                StartCoroutine(_WeaponVisibility(weaponNumber, .5f, true));
            }
            else if (weaponNumber == 3)
            {
                weapon = Weapon.TWOHANDAXE;
                StartCoroutine(_WeaponVisibility(weaponNumber, .5f, true));
            }
            else if (weaponNumber == 4)
            {
                weapon = Weapon.TWOHANDBOW;
                StartCoroutine(_WeaponVisibility(weaponNumber, .55f, true));
            }
            else if (weaponNumber == 5)
            {
                weapon = Weapon.TWOHANDCROSSBOW;
                StartCoroutine(_WeaponVisibility(weaponNumber, .5f, true));
            }
            else if (weaponNumber == 6)
            {
                weapon = Weapon.STAFF;
                StartCoroutine(_WeaponVisibility(weaponNumber, .6f, true));
            }
            else if (weaponNumber == 18)
            {
                weapon = Weapon.RIFLE;
                StartCoroutine(_WeaponVisibility(weaponNumber, .6f, true));
            }
        }
        //one handed weapons
        else
        {
            if (weaponNumber == 7)
            {
                leftWeapon = 7;
                animator.SetInteger("LeftWeapon", 7);
                animator.SetInteger("LeftRight", 1);
                StartCoroutine(_WeaponVisibility(weaponNumber, .6f, true));
                animator.SetBool("Shield", true);
            }
            else if (weaponNumber == 8 || weaponNumber == 10 || weaponNumber == 12 || weaponNumber == 14 || weaponNumber == 16)
            {
                animator.SetInteger("LeftRight", 1);
                animator.SetInteger("LeftWeapon", weaponNumber);
                StartCoroutine(_WeaponVisibility(weaponNumber, .6f, true));
                leftWeapon = weaponNumber;
                weaponNumber = 7;
            }
            else if (weaponNumber == 9 || weaponNumber == 11 || weaponNumber == 13 || weaponNumber == 15 || weaponNumber == 17)
            {
                animator.SetInteger("LeftRight", 2);
                animator.SetInteger("RightWeapon", weaponNumber);
                rightWeapon = weaponNumber;
                StartCoroutine(_WeaponVisibility(weaponNumber, .6f, true));
                weaponNumber = 7;
                //set shield to false for animator, will reset later
                if (leftWeapon == 7)
                {
                    animator.SetBool("Shield", false);
                }
            }
        }
        if (weapon == Weapon.RIFLE)
        {
            animator.SetInteger("Weapon", 8);
        }
        else
        {
            animator.SetInteger("Weapon", weaponNumber);
        }
        animator.SetTrigger("WeaponUnsheathTrigger");
        yield return new WaitForSeconds(.1f);
        if (leftWeapon == 7)
        {
            animator.SetBool("Shield", true);
        }
        if ((leftWeapon > 6 || rightWeapon > 6) && weapon != Weapon.RIFLE)
        {
            animator.SetBool("Armed", true);
            weapon = Weapon.ARMED;
        }
        //For dual blocking
        if (rightWeapon == 9 || rightWeapon == 11 || rightWeapon == 13 || rightWeapon == 15 || rightWeapon == 17)
        {
            if (leftWeapon == 8 || leftWeapon == 10 || leftWeapon == 12 || leftWeapon == 14 || leftWeapon == 16)
            {
                yield return new WaitForSeconds(.1f);
                animator.SetInteger("LeftRight", 3);
            }
        }
        if (leftWeapon == 8 || leftWeapon == 10 || leftWeapon == 12 || leftWeapon == 14 || leftWeapon == 16)
        {
            if (rightWeapon == 9 || rightWeapon == 11 || rightWeapon == 13 || rightWeapon == 15 || rightWeapon == 17)
            {
                yield return new WaitForSeconds(.1f);
                animator.SetInteger("LeftRight", 3);
            }
        }
        yield return null;
    }

    public IEnumerator _WeaponVisibility(int weaponNumber, float delayTime, bool visible)
    {
        yield return new WaitForSeconds(delayTime);

        if (weaponNumber == 1)
        {
            twohandsword.SetActive(visible);
        }
    }

    #endregion
}

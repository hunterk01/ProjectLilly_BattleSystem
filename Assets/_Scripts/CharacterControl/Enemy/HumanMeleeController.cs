using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HumanMeleeController : MonoBehaviour
{
    protected Animator animator;
    EnemyController enemyControl;
    BattleController battleControl;
    public GameObject enemyButton;

    // Variables for weapon draw delay
    private float weaponDrawTimer = 0.0f;
    private float weaponDrawDelay = .75f;

    // Variables for performing timed actions
    private Vector3 startPosition;
    private float moveSpeed = 20;
    private bool actionStarted = false;

    private bool isAlive = true;

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

    [Tooltip("Weapon Numbers: 0 = Unarmed, 1 = 2H Sword, 2 = 2H Spear, 3 = 2H Axe, 4 = 2H Bow, 5 = 2H Crossbow, 6 = 2H Staff, 9 = R Sword, 11 = R Mace, 13 = R Dagger, 17 = R Pistol, 18 = Rifle")]
    public int rightWeaponType;
    [Tooltip("Weapon Numbers: 7 = Shield, 8 = L Sword’ 10 = L Mace, 12 = L Dagger, 16 = L Pistol")]
    public int leftWeaponType;
    int rightWeapon = 0;
    int leftWeapon = 0;

    //Weapon Models
    // TODO: Remove unnecessary weapon GameObject setup
    public GameObject twohandaxe;
    public GameObject twohandsword;
    public GameObject twohandspear;
    public GameObject twohandbow;
    public GameObject twohandcrossbow;
    public GameObject staff;
    public GameObject swordL;
    public GameObject swordR;
    public GameObject maceL;
    public GameObject maceR;
    public GameObject daggerL;
    public GameObject daggerR;
    public GameObject itemL;
    public GameObject itemR;
    public GameObject shield;
    public GameObject pistolL;
    public GameObject pistolR;
    public GameObject rifle;

    public void EnemyAwake()
    {
        animator = GetComponentInChildren<Animator>();
        enemyControl = GetComponent<EnemyController>();
        startPosition = transform.position;

        // Hide all weapons
        // TODO: Remove unnecessary weapon hiding
        if (twohandaxe != null)
        {
            twohandaxe.SetActive(false);
        }
        if (twohandbow != null)
        {
            twohandbow.SetActive(false);
        }
        if (twohandcrossbow != null)
        {
            twohandcrossbow.SetActive(false);
        }
        if (twohandspear != null)
        {
            twohandspear.SetActive(false);
        }
        if (twohandsword != null)
        {
            twohandsword.SetActive(false);
        }
        if (staff != null)
        {
            staff.SetActive(false);
        }
        if (swordL != null)
        {
            swordL.SetActive(false);
        }
        if (swordR != null)
        {
            swordR.SetActive(false);
        }
        if (maceL != null)
        {
            maceL.SetActive(false);
        }
        if (maceR != null)
        {
            maceR.SetActive(false);
        }
        if (daggerL != null)
        {
            daggerL.SetActive(false);
        }
        if (daggerR != null)
        {
            daggerR.SetActive(false);
        }
        if (itemL != null)
        {
            itemL.SetActive(false);
        }
        if (itemR != null)
        {
            itemR.SetActive(false);
        }
        if (shield != null)
        {
            shield.SetActive(false);
        }
        if (pistolL != null)
        {
            pistolL.SetActive(false);
        }
        if (pistolR != null)
        {
            pistolR.SetActive(false);
        }
        if (rifle != null)
        {
            rifle.SetActive(false);
        }
    }

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

    public void Revive()
    {
        animator.SetTrigger("Revive1Trigger");
    }

    public void ThreatTracking()
    {

    }

    public void TargetSelection()
    {

    }

    // TODO: Setup RecieveAttack() function
    public void AttackInput(AttackData _chosenAttack, Vector3 _targetPosition)
    {
        StartCoroutine(PerformAttack(_targetPosition));
    }

    public void MagicInput(AttackData _chosenAttack, Vector3 _targetPosition)
    {
        StartCoroutine(PerformMagicAttack(_chosenAttack, _targetPosition));
    }

    public void FleeInput(GameObject _targetGO)
    {

    }

    // TODO: Setup ReceiveStance() function
    public void StanceInput(int _stanceID)
    {

    }

    // TODO: Setup RecieveItemUse() function
    public void ItemUseInput(int _itemID)
    {

    }

    // TODO: Setup Defend() function
    public void DefendInput()
    {
        
    }

    public void HitReaction()
    {
        // TODO: Add variable hit reaction based on damage done and defend state
        int hits = 5;
        int hitNumber = Random.Range(0, hits);
        animator.SetTrigger("GetHit" + (hitNumber + 1).ToString() + "Trigger");
    }

    public void InjuredReaction()
    {
        animator.SetTrigger("injured");
    }

    public void DeathReaction()
    {
        animator.SetTrigger("Death1Trigger");
    }

    private IEnumerator PerformAttack(Vector3 _targetPosition)
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

        // Wait for set time, then do damage
        int attackRand = Random.Range(0, 3);

        if (attackRand == 0)
            animator.SetTrigger("Attack4Trigger");
        if (attackRand == 1)
            animator.SetTrigger("Attack5Trigger");
        if (attackRand == 2)
            animator.SetTrigger("Attack6Trigger");

        yield return new WaitForSeconds(0.85f);

        // TODO: Pass damage to HeroController DoDamage() function
        enemyControl.DoDamage();

        animator.SetInteger("Jumping", 1);
        animator.SetTrigger("JumpTrigger");

        // Move enemy back to starting position
        while (MoveTowardStart(startPosition))
        {
            //animator.SetBool("Moving", true);
            //animator.SetFloat("Velocity Z", -moveSpeed);

            // Setup jump animation
            animator.SetInteger("Jumping", 2);
            animator.SetTrigger("JumpTrigger");

            yield return null;
        }

        animator.SetInteger("Jumping", 0);

        //animator.SetBool("Moving", false);

        actionStarted = false;
        enemyControl.EndAction();
    }

    private IEnumerator PerformMagicAttack(AttackData _chosenAttack, Vector3 _targetPosition)
    {
        return null;
    }

    private bool MoveTowardTarget(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime));
    }

    private bool MoveTowardStart(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, (moveSpeed * 1.25f) * Time.deltaTime));
    }


    // TODO: Setup attack animation function
    private void BowShootingAnim()
    {

    }

    // TODO: Setup stance animation function
    private void UtilityAnim()
    {

    }

    // TODO: Setup item use animation function
    private void ItemUseAnim()
    {

    }

    // TODO: Setup hit animation function
    private void HitReactionAnim()
    {

    }

    // TODO: Setup defend animation function
    private void DefendAnim()
    {

    }

    public void HeroDeathAnim()
    {
        animator.SetTrigger("Death1Trigger");
    }

    public void EnemyPanelButtonOn()
    {
        enemyButton.SetActive(true);
    }

    public void EnemyPanelButtonOff()
    {
        enemyButton.SetActive(false);
    }

    #region _Coroutines

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
        if (weaponNumber == 2)
        {
            twohandspear.SetActive(visible);
        }
        if (weaponNumber == 3)
        {
            twohandaxe.SetActive(visible);
        }
        if (weaponNumber == 4)
        {
            twohandbow.SetActive(visible);
        }
        if (weaponNumber == 5)
        {
            twohandcrossbow.SetActive(visible);
        }
        if (weaponNumber == 6)
        {
            staff.SetActive(visible);
        }
        if (weaponNumber == 7)
        {
            shield.SetActive(visible);
        }
        if (weaponNumber == 8)
        {
            swordL.SetActive(visible);
        }
        if (weaponNumber == 9)
        {
            swordR.SetActive(visible);
        }
        if (weaponNumber == 10)
        {
            maceL.SetActive(visible);
        }
        if (weaponNumber == 11)
        {
            maceR.SetActive(visible);
        }
        if (weaponNumber == 12)
        {
            daggerL.SetActive(visible);
        }
        if (weaponNumber == 13)
        {
            daggerR.SetActive(visible);
        }
        if (weaponNumber == 14)
        {
            itemL.SetActive(visible);
        }
        if (weaponNumber == 15)
        {
            itemR.SetActive(visible);
        }
        if (weaponNumber == 16)
        {
            pistolL.SetActive(visible);
        }
        if (weaponNumber == 17)
        {
            pistolR.SetActive(visible);
        }
        if (weaponNumber == 18)
        {
            rifle.SetActive(visible);
        }

        yield return null;
    }

    #endregion
}

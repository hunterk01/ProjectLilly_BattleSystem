using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChimeraController : MonoBehaviour, IEnemyActionControl
{
    protected Animator animator;
    EnemyController enemyControl;
    BattleController battleControl;
    public GameObject enemyButton;

    // Variables for performing timed actions
    private Vector3 startPosition;
    private float moveSpeed = 15;
    private bool actionStarted = false;

    private bool isAlive = true;

    public GameObject spellSpawn;

    public void EnemyAwake()
    {
        animator = GetComponentInChildren<Animator>();
        enemyControl = GetComponent<EnemyController>();
        battleControl = GameObject.Find("BattleManager").GetComponent<BattleController>();
        startPosition = transform.position;
        enemyButton.GetComponent<EnemySelectButton>().enemyPrefab = gameObject;
    }

    public void DrawWeapon()
    {
        // Nothing happening here, but the interface needs it.
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

    public void AttackInput(AttackData _chosenAttack, Vector3 _targetPosition)
    {
        if (_chosenAttack.attackType == AttackData.AttackType.MELEE)
            StartCoroutine(PerformAttack(_chosenAttack, _targetPosition));
        else if (_chosenAttack.attackType == AttackData.AttackType.SPELL)
            StartCoroutine(PerformMagicAttack(_chosenAttack, _targetPosition));
    }

    public void MagicInput(AttackData _chosenAttack, Vector3 _targetPosition)
    {
        StartCoroutine(PerformMagicAttack(_chosenAttack, _targetPosition));
    }

    public void FleeInput(GameObject _targetGO)
    {

    }

    public void ItemUseInput(int _itemID)
    {
        // Nothing happening here, but the interface needs it.
    }

    public void DefendInput()
    {
        // Nothing happening here, but the interface needs it.
    }

    // Play the appropriate hit reaction depending on if its a spell or melee attack
    public void HitReaction()
    {
        if (battleControl.activeAgentList[0].chosenAttack.attackType == AttackData.AttackType.SPELL)
        {
            animator.SetTrigger("Hit2Trigger");
        }
        else
        {
            animator.SetTrigger("Hit1Trigger");
        }
    }

    public void InjuredReaction()
    {
        animator.SetBool("injured", true);
    }

    public void DeathReaction()
    {
        animator.SetTrigger("death");
    }

    // Coroutine for handling melee attack actions and animations
    private IEnumerator PerformAttack(AttackData _chosenAttack, Vector3 _targetPosition)
    {
        _targetPosition = new Vector3(_targetPosition.x + _chosenAttack.targetOffset, _targetPosition.y, _targetPosition.z);

        if (actionStarted)
        {
            yield break;
        }

        actionStarted = true;

        // Handle attack animations and movement
        animator.SetTrigger(_chosenAttack.attackAnimation);
        yield return new WaitForSeconds(_chosenAttack.attackWaitTime);

        while (MoveTowardTarget(_targetPosition))
        {
            yield return null;
        }
        animator.SetTrigger(_chosenAttack.attackAnimation);

        while (MoveTowardTarget(_targetPosition))
        {
            yield return null;
        }

        yield return new WaitForSeconds(_chosenAttack.damageWaitTime);

        enemyControl.DoDamage();

        yield return new WaitForSeconds(_chosenAttack.attackWaitTime);

        animator.SetTrigger("run");
        yield return new WaitForSeconds(.5f);

        // Move enemy back to starting position
        while (MoveTowardStart(startPosition))
        {
            yield return null;
        }

        animator.SetTrigger("attackTurn");
        //yield return new WaitForSeconds(_chosenAttack.attackWaitTime);
        animator.SetTrigger("idle");

        actionStarted = false;
        enemyControl.EndAction();
    }

    // Coroutine for handling spellcast actions and animations
    private IEnumerator PerformMagicAttack(AttackData _chosenAttack, Vector3 _targetPosition)
    {
        if (actionStarted)
        {
            yield break;
        }

        actionStarted = true;

        animator.SetTrigger(_chosenAttack.attackAnimation);

        yield return new WaitForSeconds(_chosenAttack.attackWaitTime);

        // Shoot spell
        Vector3 relativePosition = _targetPosition - transform.position;
        Vector3 targetHieghtOffset = new Vector3(0, 1.25f, 0);
        Quaternion spellRotation = Quaternion.LookRotation(relativePosition + targetHieghtOffset);
        GameObject tempSpell = Instantiate(_chosenAttack.projectile, _targetPosition, spellRotation) as GameObject;

        yield return new WaitForSeconds(_chosenAttack.damageWaitTime);

        Destroy(tempSpell);

        enemyControl.DoDamage();

        yield return new WaitForSeconds(.5f);

        actionStarted = false;
        enemyControl.EndAction();
    }

    private bool MoveTowardTarget(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime));
    }

    private bool MoveTowardStart(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime));
    }

    public void EnemyPanelButtonOn()
    {
        enemyButton.SetActive(true);
    }

    public void EnemyPanelButtonOff()
    {
        enemyButton.SetActive(false);
    }
}

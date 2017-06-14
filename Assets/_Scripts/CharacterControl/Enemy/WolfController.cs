using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WolfController : MonoBehaviour, IEnemyActionControl
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

    public void EnemyAwake()
    {
        animator = GetComponentInChildren<Animator>();
        enemyControl = GetComponent<EnemyController>();
        battleControl = GameObject.Find("BattleManager").GetComponent<BattleController>();
        startPosition = transform.position;
        enemyButton.GetComponent<EnemySelectButton>().enemyPrefab = gameObject;

        animator.Play("Wolf Basic Idle", -1, Random.Range(0.0f, 1.0f));
    }

    public void DrawWeapon()
    {
        // Nothing happening here, but the interface needs it.
    }

    public void Revive()
    {
        //animator.SetTrigger("Revive1Trigger");
    }

    public void ThreatTracking()
    {

    }

    public void TargetSelection()
    {

    }

    // Receive attack data and run the appropriate coroutine
    public void AttackInput(AttackData _chosenAttack, Vector3 _targetPosition)
    {
        StartCoroutine(PerformAttack(_chosenAttack, _targetPosition));
    }

    public void MagicInput(AttackData _chosenAttack, Vector3 _targetPosition)
    {
        StartCoroutine(PerformMagicAttack(_chosenAttack, _targetPosition));
    }

    public void FleeInput(GameObject _targetGO)
    {
        StartCoroutine(PerformFlee(_targetGO));
    }

    public void ItemUseInput(int _itemID)
    {
        // Nothing happening here, but the interface needs it.
    }

    public void DefendInput()
    {
        // Nothing happening here, but the interface needs it.
    }

    public void HitReaction()
    {
        animator.SetTrigger("hit");
    }

    public void InjuredReaction()
    {
        animator.SetBool("injured", true);
    }

    public void DeathReaction()
    {
        animator.SetTrigger("death");
    }

    // Coroutine for handling melee attacks
    private IEnumerator PerformAttack(AttackData _chosenAttack, Vector3 _targetPosition)
    {
        _targetPosition = new Vector3(_targetPosition.x + _chosenAttack.targetOffset, _targetPosition.y, _targetPosition.z);
        Vector3 attackerPosition = battleControl.activeAgentList[0].agentGO.transform.position;
        float targetDistance = Vector3.Distance(attackerPosition, _targetPosition);
        float leapDistance = 7f;

        if (actionStarted)
        {
            yield break;
        }

        actionStarted = true;

        // Set running animation trigger
        animator.SetTrigger("run");
        yield return new WaitForSeconds(.1f);

        if (_chosenAttack.moveDuringAttack)
        {
            while (MoveTowardTarget(_targetPosition) && targetDistance > leapDistance)
            {
                attackerPosition = battleControl.activeAgentList[0].agentGO.transform.position;
                targetDistance = Vector3.Distance(attackerPosition, _targetPosition);

                yield return null;
            }

            animator.SetTrigger(_chosenAttack.attackAnimation);

            yield return new WaitForSeconds(.12f);

            while (MoveTowardTarget(_targetPosition))
            {
                yield return null;
            }
        }
        else
        {
            while (MoveTowardTarget(_targetPosition))
            {
                yield return null;
            }

            animator.SetTrigger(_chosenAttack.attackAnimation);
        }

        yield return new WaitForSeconds(_chosenAttack.damageWaitTime);

        enemyControl.DoDamage();

        yield return new WaitForSeconds(_chosenAttack.attackWaitTime);

        animator.SetTrigger("run");
        yield return new WaitForSeconds(.4f);

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

    // Coroutine for handling attack spellcasting
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

    // Coroutine for handling cleansed enemy leaving the battlefield
    private IEnumerator PerformFlee(GameObject _targetGO)
    {
        Vector3 targetPosition = new Vector3(transform.position.x + 25, transform.position.y, transform.position.z);
        Vector3 attackerPosition = battleControl.activeAgentList[0].agentGO.transform.position;

        if (actionStarted)
        {
            yield break;
        }

        actionStarted = true;

        // Pause to let hit reaction animation finish
        yield return new WaitForSeconds(1.2f);

        // Set running animation trigger
        animator.SetTrigger("fleeTurn");
        yield return new WaitForSeconds(.34f);

        animator.SetTrigger("run");
        yield return new WaitForSeconds(.4f);

        _targetGO.GetComponent<Collider>().enabled = false;

        while (MoveTowardTarget(targetPosition))
        {
            yield return null;
        }

        animator.SetTrigger("idle");

        enemyControl.enemyPanel.SetActive(false);
        
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
using UnityEngine;

public interface IEnemyActionControl
{
    void EnemyAwake();
    void DrawWeapon();
    void Revive();
    void ThreatTracking();
    void TargetSelection();
    void AttackInput(AttackData _chosenAttack, Vector3 _targetPosition);
    void MagicInput(AttackData _chosenAttack, Vector3 _targetPosition);
    void FleeInput(GameObject _targetGO);
    void ItemUseInput(int _itemID);
    void DefendInput();
    void HitReaction();
    void InjuredReaction();
    void DeathReaction();
    void EnemyPanelButtonOn();
    void EnemyPanelButtonOff();
}

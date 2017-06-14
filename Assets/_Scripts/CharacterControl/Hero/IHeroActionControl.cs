using UnityEngine;

public interface IHeroActionControl
{
    void HeroAwake();

    void DrawWeapon();
    void AttackInput(AttackData _chosenAttack, Vector3 _targetPosition);
    void RestoreInput(AttackData _chosenAttack, Vector3 _tagetPositon);
    //void ActionInput(ActionData _chosenAction, Vector3 _targetPosition);
    //void DefendInput();
    //void ItemUseInput(int _itemID);
    void HitReaction();
    void DeathReaction();

}

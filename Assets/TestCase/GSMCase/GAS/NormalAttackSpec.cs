using GAS.Runtime;
using UnityEngine;
public class NormalAttackSpec : AbilitySpec<NormalAttack>
{
    private GameplayEffectAsset _effectAsset;
    private GameObject _target;

    public NormalAttackSpec(NormalAttack ability, AbilitySystemComponent owner) : base(ability, owner)
    {
        _effectAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<GameplayEffectAsset>("Assets/GAS/Config/GameplayEffectLib/GE_NormalDamage.asset");
    }

    public override void ActivateAbility(params object[] args)
    {
        if (_effectAsset != null)
        {
            _target = args[0] as GameObject;
            var targetAsc = _target.GetComponent<AbilitySystemComponent>();
            var gameplayEffect = new GameplayEffect(_effectAsset);
            Owner.ApplyGameplayEffectTo(gameplayEffect, targetAsc);
            TryEndAbility();
        }
    }

    public override void CancelAbility()
    {
    }

    public override void EndAbility()
    {
    }
}

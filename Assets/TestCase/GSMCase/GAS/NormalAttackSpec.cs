using GAS.Runtime;
public class NormalAttackSpec : AbilitySpec<NormalAttack>
{
    public NormalAttackSpec(NormalAttack ability, AbilitySystemComponent owner) : base(ability, owner)
    {
    }

    public override void ActivateAbility(params object[] args)
    {
    }

    public override void CancelAbility()
    {
    }

    public override void EndAbility()
    {
    }
}

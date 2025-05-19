using GAS.Runtime;
public class NormalAttack : AbstractAbility<NormalAttackAsset>
{
    public NormalAttack(NormalAttackAsset abilityAsset) : base(abilityAsset)
    {
    }

    public override AbilitySpec CreateSpec(AbilitySystemComponent owner)
    {
        return new NormalAttackSpec(this, owner);
    }
}

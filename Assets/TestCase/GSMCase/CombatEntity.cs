using GAS.Runtime;
using Serialization;
using ZeroPass;

[SkipSaveFileSerialization]
public class CombatEntity : RMonoBehaviour
{
    public bool IsPlayer
    {
        get;
        set;
    }

    public bool IsDead
    {
        get;
        set;
    }

    public CombatEntity Target { get; private set; }

    public AbilitySpec SelectedAction { get; private set; }

    public bool HasSelectedAction => SelectedAction != null;

    protected override void OnPrefabInit()
    {
        base.OnPrefabInit();
    }

    protected override void OnSpawn()
    {
        base.OnSpawn();

        var asc = GetComponent<AbilitySystemComponent>();
        asc.AttrSet<AS_CombatEntity>().HP.RegisterPostCurrentValueChange(OnHpChange);
    }

    private void OnHpChange(AttributeBase attribute, float oldValue, float newValue)
    {
        var delta = oldValue - newValue;
        if (delta > 0)
        {
            var damageData = new DamageData { num = (int)delta, isCritical = false };
            Trigger((int)GameHashes.DoDamage, damageData);
        }
    }

    public void AIChooseAction()
    {
        if (IsPlayer || IsDead) return;

        // SelectedAction = availableActions[Random.Range(0, availableActions.Count)];
        Target = FindRandomTarget();
    }

    private CombatEntity FindRandomTarget()
    {
        return null;
    }

    public void OnAttackHitEvent()
    {
        Trigger((int)GameHashes.AttackContact, this);
        DealDamage();
    }

    private void DealDamage()
    {
        // Target.TakeDamage(damage);
    }
}

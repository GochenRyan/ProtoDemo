using GAS.Runtime;
using System.Collections;
using UnityEngine;
using ZeroPass;

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

using GAS.Runtime;
using UnityEngine;

public class CueSFXOneShot : GameplayCueInstant
{
    public override GameplayCueInstantSpec CreateSpec(GameplayCueParameters parameters)
    {
        return new CueSFXOneShotSpec(this, parameters);
    }
}

public class CueSFXOneShotSpec : GameplayCueInstantSpec<CueSFXOneShot>
{
    public CueSFXOneShotSpec(CueSFXOneShot cue, GameplayCueParameters parameters) : base(cue, parameters)
    {
    }

    public override void Trigger()
    {
        var abilitySpec = _parameters.sourceAbilitySpec;
        var owner = abilitySpec.Owner;
        var ownerGO = owner.gameObject;
        SpriteRenderer sr = ownerGO.GetComponent<SpriteRenderer>();
        Vector3 centerPos = sr.bounds.center;
        GSMTestRoot.Instance.OnSwordAttack(centerPos);
    }
}

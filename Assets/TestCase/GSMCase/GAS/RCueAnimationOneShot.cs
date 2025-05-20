using GAS.Runtime;

public class RCueAnimationOneShot : GameplayCueInstant
{
    public override GameplayCueInstantSpec CreateSpec(GameplayCueParameters parameters)
    {
        return new RCueAnimationOneShotSpec(this, parameters);
    }
}

public class RCueAnimationOneShotSpec : GameplayCueInstantSpec<RCueAnimationOneShot>
{
    public RCueAnimationOneShotSpec(RCueAnimationOneShot cue, GameplayCueParameters parameters) : base(cue, parameters)
    {
    }

    public override void Trigger()
    {
        var geSpec = _parameters.sourceGameplayEffectSpec;
        var target = geSpec.Owner;
        var targetGO = target.gameObject;
        var targetAnimController = targetGO.GetComponent<RAnimControllerBase>();
        targetAnimController.Play("Hit");
    }
}

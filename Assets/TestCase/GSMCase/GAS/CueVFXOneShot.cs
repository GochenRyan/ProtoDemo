using GAS.Runtime;
using UnityEngine;

public class CueVFXOneShot : GameplayCueInstant
{
    public override GameplayCueInstantSpec CreateSpec(GameplayCueParameters parameters)
    {
        return new CueVFXOneShotSpec(this, parameters);
    }
}

public class CueVFXOneShotSpec : GameplayCueInstantSpec<CueVFXOneShot>
{
    public CueVFXOneShotSpec(CueVFXOneShot cue, GameplayCueParameters parameters) : base(cue, parameters)
    {
    }

    public override void Trigger()
    {
        var geSpec = _parameters.sourceGameplayEffectSpec;
        var owner = geSpec.Source;
        var ownerGO = owner.gameObject;
        SpriteRenderer sr = ownerGO.GetComponent<SpriteRenderer>();
        Vector3 centerPos = sr.bounds.center;
        if (geSpec.GameplayEffect.GameplayEffectName == "GE_NormalDamage")
        {
            var particlePrefab = Resources.Load<GameObject>("FX/CFXR Impact Glowing HDR (Blue)");
            GameObject effect = GameObject.Instantiate(particlePrefab, centerPos, Quaternion.identity);
            var ps = effect.GetComponent<ParticleSystem>();
            var main = ps.main;
            main.stopAction = ParticleSystemStopAction.Destroy;
            ps.Play();
            float life = main.duration + main.startLifetime.constantMax;
            GameObject.Destroy(effect, life + 0.1f);
        }

        var target = geSpec.Owner;
        var targetGO = target.gameObject;
        SpriteRenderer sr1 = targetGO.GetComponent<SpriteRenderer>();
        Vector3 centerPos1 = sr1.bounds.center;
        var particlePrefab1 = Resources.Load<GameObject>("FX/CFXR2 Blood Shape Splash");
        GameObject effect1 = GameObject.Instantiate(particlePrefab1, centerPos1, Quaternion.identity);
        var ps1 = effect1.GetComponent<ParticleSystem>();
        var main1 = ps1.main;
        main1.stopAction = ParticleSystemStopAction.Destroy;
        ps1.Play();
        float life1 = main1.duration + main1.startLifetime.constantMax;
        GameObject.Destroy(effect1, life1 + 0.1f);
    }
}

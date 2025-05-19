using DG.Tweening;
using GAS.Runtime;
using UnityEngine;

public class AbilitySMComponent : StateMachineComponent<AbilitySMComponent.StatesInstance>
{
    public class StatesInstance : GameStateMachine<States, StatesInstance, AbilitySMComponent, object>.GameInstance
    {
        public StatesInstance(AbilitySMComponent master)
            : base(master)
        {

        }
    }

    public class States : GameStateMachine<States, StatesInstance, AbilitySMComponent>
    {
        public TargetParameter target;
        public TargetParameter source;
        private StringParameter ability;
        public State chooseAbility;
        public State approach;
        public State applyAbility;
        public State success;

        public override void InitializeStates(out BaseState default_state)
        {
            default_state = chooseAbility;
            serializable = true;

            chooseAbility.Enter(delegate (StatesInstance smi)
            {
                //TODO: AI
                ability.Set("NormalAttack", smi);
                smi.GoTo(approach);
            });

            approach.Enter(delegate (StatesInstance smi)
            {
                var sourceGO = source.Get(smi);
                var srcAsc = sourceGO.GetComponent<AbilitySystemComponent>();
                var abilitySpec = srcAsc.AbilityContainer.AbilitySpecs()[ability.Get(smi)];
                if (!abilitySpec.Ability.Tag.AssetTag.HasTag(GTagLib.Ability_Target_Self))
                {
                    float distanceFromTarget = 1f;
                    float duration = 0.5f;
                    var targetGO = target.Get(smi);
                    Vector3 direction = (targetGO.transform.position.x < targetGO.transform.position.x) ? Vector3.left : Vector3.right;
                    Vector3 destination = targetGO.transform.position + direction * distanceFromTarget;
                    sourceGO.transform.DOMove(destination, duration).SetEase(Ease.OutQuad).OnComplete(() =>
                    {
                        smi.GoTo(applyAbility);
                    });
                }
            });

            applyAbility.Enter(delegate (StatesInstance smi)
            {
                //TODO: weapon type, target fiction
                smi.Play("Attack");
            }).EventHandler(GameHashes.AttackContact, delegate (StatesInstance smi)
            {
                smi.GoTo(success);
            });

            success.Enter(delegate (StatesInstance smi)
            {
                
                var sourceGO = source.Get(smi);
                SpriteRenderer sr = sourceGO.GetComponent<SpriteRenderer>();
                Vector3 centerPos = sr.bounds.center;

                //TODO: GAS
                var particlePrefab = Resources.Load<GameObject>("FX/CFXR Impact Glowing HDR (Blue)");
                GameObject effect = Instantiate(particlePrefab, centerPos, Quaternion.identity);
                var ps = effect.GetComponent<ParticleSystem>();
                var main = ps.main;
                main.stopAction = ParticleSystemStopAction.Destroy;
                ps.Play();
                float life = main.duration + main.startLifetime.constantMax;
                Destroy(effect, life + 0.1f);


                //TODO: Die
                var targetGO = target.Get(smi);
                var targetAnimController = targetGO.GetComponent<RAnimControllerBase>();
                targetAnimController.Play("Hit");

                SpriteRenderer sr1 = targetGO.GetComponent<SpriteRenderer>();
                Vector3 centerPos1 = sr1.bounds.center;
                var particlePrefab1 = Resources.Load<GameObject>("FX/CFXR2 Blood Shape Splash");
                GameObject effect1 = Instantiate(particlePrefab1, centerPos1, Quaternion.identity);
                var ps1 = effect1.GetComponent<ParticleSystem>();
                var main1 = ps1.main;
                main1.stopAction = ParticleSystemStopAction.Destroy;
                ps1.Play();
                float life1 = main1.duration + main1.startLifetime.constantMax;
                Destroy(effect1, life1 + 0.1f);


                GSMTestRoot.Instance.OnSwordAttack(centerPos);

                //TODO: Batter  EventHandler GameHashes.Batter
                if (Random.value < 0.8f)
                {
                    smi.GoTo(applyAbility);
                }
                else
                {
                    float duration = 0.5f;

                    var animController = smi.GetComponent<RAnimControllerBase>();
                    animController.Queue(() =>
                    {
                        sourceGO.transform.DOMove(new Vector3(-5, 0, 0), duration).SetEase(Ease.OutQuad);
                    });

                    smi.Queue("Idle", PlayMode.Loop);
                    targetAnimController.Queue("Idle", PlayMode.Loop);
                }
            }).ReturnSuccess(); ;
        }
    }
}

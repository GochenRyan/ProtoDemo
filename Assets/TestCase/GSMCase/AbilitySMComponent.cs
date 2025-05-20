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
                var asc = sourceGO.GetComponent<AbilitySystemComponent>();
                var targetGO = target.Get(smi);

                asc.TryActivateAbility(ability.Get(smi), targetGO);

                // Try use tag to implement combo
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

                    if (targetGO != null)
                    {
                        var targetAnimController = targetGO.GetComponent<RAnimControllerBase>();
                        targetAnimController.Queue("Idle", PlayMode.Loop);
                    }
                }
            }).ReturnSuccess(); ;
        }
    }
}

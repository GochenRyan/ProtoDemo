using DG.Tweening;
using UnityEngine;

public class AttackSMComponent : StateMachineComponent<AttackSMComponent.StatesInstance>
{
    public class StatesInstance : GameStateMachine<States, StatesInstance, AttackSMComponent, object>.GameInstance
    {
        public StatesInstance(AttackSMComponent master)
            : base(master)
        {

        }
    }

    public class States : GameStateMachine<States, StatesInstance, AttackSMComponent>
    {
        public TargetParameter attackTarget;
        public TargetParameter attacker;
        public State approach;
        public State attack;
        public State success;

        public override void InitializeStates(out BaseState default_state)
        {
            default_state = approach;
            approach.Enter(delegate (StatesInstance smi)
            {
                float distanceFromTarget = 1f;
                float duration = 0.5f;
                var attackerGO = attacker.Get(smi);
                var attackTargetGO = attackTarget.Get(smi);
                Vector3 direction = (attackerGO.transform.position.x < attackTargetGO.transform.position.x) ? Vector3.left : Vector3.right;
                Vector3 destination = attackTargetGO.transform.position + direction * distanceFromTarget;
                attackerGO.transform.DOMove(destination, duration).SetEase(Ease.OutQuad).OnComplete(() =>
                {
                    smi.GoTo(attack);
                });

            });

            attack.Enter(delegate (StatesInstance smi)
            {
                smi.Play("Attack");
            }).EventHandler(GameHashes.AttackContact, delegate (StatesInstance smi)
            {
                smi.GoTo(success);
            });

            success.Enter(delegate (StatesInstance smi) 
            {
                //TODO: GAS
                var attackTargetGO = attackTarget.Get(smi);
                var attackTargetAnimController = attackTargetGO.GetComponent<RAnimControllerBase>();
                attackTargetAnimController.Play("Hit");

                //fake batter
                if (Random.value < 0.0f)
                {
                    smi.GoTo(attack);
                }
                else
                {
                    smi.Queue("Idle", PlayMode.Loop);
                    attackTargetAnimController.Queue("Idle", PlayMode.Loop);
                }
            }).ReturnSuccess(); ;
        }
    }
}

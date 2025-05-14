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
            serializable = true;
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
                var attackerGO = attacker.Get(smi);
                SpriteRenderer sr = attackerGO.GetComponent<SpriteRenderer>();
                Vector3 centerPos = sr.bounds.center;

                var particlePrefab = Resources.Load<GameObject>("FX/CFXR Impact Glowing HDR (Blue)");
                GameObject effect = Instantiate(particlePrefab, centerPos, Quaternion.identity);
                var ps = effect.GetComponent<ParticleSystem>();
                var main = ps.main;
                main.stopAction = ParticleSystemStopAction.Destroy;
                ps.Play();
                float life = main.duration + main.startLifetime.constantMax;
                Destroy(effect, life + 0.1f);

                var attackTargetGO = attackTarget.Get(smi);
                var attackTargetAnimController = attackTargetGO.GetComponent<RAnimControllerBase>();
                attackTargetAnimController.Play("Hit");

                SpriteRenderer sr1 = attackTargetGO.GetComponent<SpriteRenderer>();
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

                //fake batter
                if (Random.value < 0.8f)
                {
                    smi.GoTo(attack);
                }
                else
                {
                    float duration = 0.5f;

                    var animController = smi.GetComponent<RAnimControllerBase>();
                    animController.Queue(() =>
                    {
                        attackerGO.transform.DOMove(new Vector3(-5, 0, 0), duration).SetEase(Ease.OutQuad);
                    });

                    smi.Queue("Idle", PlayMode.Loop);
                    attackTargetAnimController.Queue("Idle", PlayMode.Loop);
                }
            }).ReturnSuccess(); ;
        }
    }
}

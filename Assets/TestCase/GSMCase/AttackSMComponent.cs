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
        public override void InitializeStates(out BaseState default_state)
        {
            default_state = null;
        }
    }
}

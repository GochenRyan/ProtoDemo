using UnityEngine;
using ZeroPass.StateMachine;

public abstract class GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType> : StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType> where StateMachineType : GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType> where StateMachineInstanceType : GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.GameInstance where MasterType : IStateMachineTarget
{
    public class GameInstance : GenericInstance
    {
        public GameInstance(MasterType master, DefType def)
            : base(master)
        {
            base.def = def;
        }

        public GameInstance(MasterType master)
            : base(master)
        {
        }

        //public void Queue(string anim, KAnim.PlayMode mode = KAnim.PlayMode.Once)
        //{
        //    StateMachineInstanceType smi = base.smi;
        //    ((Instance)smi).GetComponent<KBatchedAnimController>().Queue(anim, mode, 1f, 0f);
        //}

        //public void Play(string anim, KAnim.PlayMode mode = KAnim.PlayMode.Once)
        //{
        //    StateMachineInstanceType smi = base.smi;
        //    ((Instance)smi).GetComponent<KBatchedAnimController>().Play(anim, mode, 1f, 0f);
        //}
    }
}

public abstract class GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType> : GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, object> where StateMachineType : GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, object> where StateMachineInstanceType : GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, object>.GameInstance where MasterType : IStateMachineTarget
{
}
public abstract class GameStateMachine<StateMachineType, StateMachineInstanceType> : GameStateMachine<StateMachineType, StateMachineInstanceType, IStateMachineTarget, object> where StateMachineType : GameStateMachine<StateMachineType, StateMachineInstanceType, IStateMachineTarget, object> where StateMachineInstanceType : GameStateMachine<StateMachineType, StateMachineInstanceType, IStateMachineTarget, object>.GameInstance
{
}
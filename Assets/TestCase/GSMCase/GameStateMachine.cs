using System.Collections.Generic;
using System;
using UnityEngine;
using ZeroPass.StateMachine;
using ZeroPass;

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

        public void Play(string anim)
        {
            StateMachineInstanceType smi = base.smi;
            ((Instance)smi).GetComponent<Animator>().Play(anim);
        }
    }

    public new class State : StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State
    {
        [DoNotAutoCreate]
        private TargetParameter stateTarget;

        public State root => this;

        public State master
        {
            get
            {
                stateTarget = sm.masterTarget;
                return this;
            }
        }

        private TargetParameter GetStateTarget()
        {
            if (stateTarget == null)
            {
                if (parent != null)
                {
                    State state = (State)parent;
                    return state.GetStateTarget();
                }
                TargetParameter targetParameter = sm.stateTarget;
                if (targetParameter == null)
                {
                    return sm.masterTarget;
                }
                return targetParameter;
            }
            return stateTarget;
        }

        public int CreateDataTableEntry()
        {
            return sm.dataTableSize++;
        }

        public int CreateUpdateTableEntry()
        {
            return sm.updateTableSize++;
        }

        public State DoNothing()
        {
            return this;
        }

        private static List<Action> AddAction(string name, Callback callback, List<Action> actions, bool add_to_end)
        {
            if (actions == null)
            {
                actions = new List<Action>();
            }
            Action item = new Action(name, callback);
            if (add_to_end)
            {
                actions.Add(item);
            }
            else
            {
                actions.Insert(0, item);
            }
            return actions;
        }

        public State Target(TargetParameter target)
        {
            stateTarget = target;
            return this;
        }

        public State Update(Action<StateMachineInstanceType, float> callback, UpdateRate update_rate = UpdateRate.SIM_200ms, bool load_balance = false)
        {
            return Update(sm.name + "." + name, callback, update_rate, load_balance);
        }

        public State BatchUpdate(UpdateBucketWithUpdater<StateMachineInstanceType>.BatchUpdateDelegate batch_update, UpdateRate update_rate = UpdateRate.SIM_200ms)
        {
            return BatchUpdate(sm.name + "." + name, batch_update, update_rate);
        }

        public State Enter(Callback callback)
        {
            return Enter("Enter", callback);
        }

        public State Exit(Callback callback)
        {
            return Exit("Exit", callback);
        }

        private State InternalUpdate(string name, UpdateBucketWithUpdater<StateMachineInstanceType>.IUpdater bucket_updater, UpdateRate update_rate, bool load_balance, UpdateBucketWithUpdater<StateMachineInstanceType>.BatchUpdateDelegate batch_update = null)
        {
            int updateTableIdx = CreateUpdateTableEntry();
            if (updateActions == null)
            {
                updateActions = new List<UpdateAction>();
            }
            UpdateAction item = default(UpdateAction);
            item.updateTableIdx = updateTableIdx;
            item.updateRate = update_rate;
            item.updater = bucket_updater;
            int num = 1;
            if (load_balance)
            {
                num = Singleton<StateMachineUpdater>.Instance.GetFrameCount(update_rate);
            }
            item.buckets = new StateMachineUpdater.BaseUpdateBucket[num];
            for (int i = 0; i < num; i++)
            {
                UpdateBucketWithUpdater<StateMachineInstanceType> updateBucketWithUpdater = new UpdateBucketWithUpdater<StateMachineInstanceType>(name);
                updateBucketWithUpdater.batch_update_delegate = batch_update;
                Singleton<StateMachineUpdater>.Instance.AddBucket(update_rate, updateBucketWithUpdater);
                item.buckets[i] = updateBucketWithUpdater;
            }
            updateActions.Add(item);
            return this;
        }

        public State Update(string name, Action<StateMachineInstanceType, float> callback, UpdateRate update_rate = UpdateRate.SIM_200ms, bool load_balance = false)
        {
            return InternalUpdate(name, new BucketUpdater<StateMachineInstanceType>(callback), update_rate, load_balance, null);
        }

        public State BatchUpdate(string name, UpdateBucketWithUpdater<StateMachineInstanceType>.BatchUpdateDelegate batch_update, UpdateRate update_rate = UpdateRate.SIM_200ms)
        {
            return InternalUpdate(name, null, update_rate, false, batch_update);
        }

        public State FastUpdate(string name, UpdateBucketWithUpdater<StateMachineInstanceType>.IUpdater updater, UpdateRate update_rate = UpdateRate.SIM_200ms, bool load_balance = false)
        {
            return InternalUpdate(name, updater, update_rate, load_balance, null);
        }

        public State Enter(string name, Callback callback)
        {
            enterActions = AddAction(name, callback, enterActions, true);
            return this;
        }

        public State Exit(string name, Callback callback)
        {
            exitActions = AddAction(name, callback, exitActions, false);
            return this;
        }

        //public State Toggle(string name, Callback enter_callback, Callback exit_callback)
        //{
        //    int data_idx = CreateDataTableEntry();
        //    Enter("ToggleEnter(" + name + ")", delegate (StateMachineInstanceType smi)
        //    {
        //        smi.dataTable[data_idx] = GameStateMachineHelper.HasToggleEnteredFlag;
        //        enter_callback(smi);
        //    });
        //    Exit("ToggleExit(" + name + ")", delegate (StateMachineInstanceType smi)
        //    {
        //        if (smi.dataTable[data_idx] != null)
        //        {
        //            smi.dataTable[data_idx] = null;
        //            exit_callback(smi);
        //        }
        //    });
        //    return this;
        //}

        private void Break(StateMachineInstanceType smi)
        {
        }

        public State BreakOnEnter()
        {
            return Enter(delegate (StateMachineInstanceType smi)
            {
                Break(smi);
            });
        }

        public State BreakOnExit()
        {
            return Exit(delegate (StateMachineInstanceType smi)
            {
                Break(smi);
            });
        }

        public State ToggleAnims(Func<StateMachineInstanceType, HashedString> chooser_callback)
        {
            return this;
        }

        public State ToggleAnims(string anim_file, float priority = 0f)
        {
            return this;
        }

        public State EventHandler(GameHashes evt, Func<StateMachineInstanceType, RMonoBehaviour> global_event_system_callback, Callback callback)
        {
            return EventHandler(evt, global_event_system_callback, delegate (StateMachineInstanceType smi, object d)
            {
                callback(smi);
            });
        }

        public State EventHandler(GameHashes evt, Func<StateMachineInstanceType, RMonoBehaviour> global_event_system_callback, GameEvent.Callback callback)
        {
            if (events == null)
            {
                events = new List<StateEvent>();
            }
            TargetParameter target = GetStateTarget();
            GameEvent item = new GameEvent(evt, callback, target, global_event_system_callback);
            events.Add(item);
            return this;
        }

        public State EventHandler(GameHashes evt, Callback callback)
        {
            return EventHandler(evt, delegate (StateMachineInstanceType smi, object d)
            {
                callback(smi);
            });
        }

        public State EventHandler(GameHashes evt, GameEvent.Callback callback)
        {
            EventHandler(evt, null, callback);
            return this;
        }

        public State ReturnSuccess()
        {
            Enter("ReturnSuccess()", delegate (StateMachineInstanceType smi)
            {
                smi.SetStatus(Status.Success);
                smi.StopSM("GameStateMachine.ReturnSuccess()");
            });
            return this;
        }
    }

    public class GameEvent : StateEvent
    {
        public delegate void Callback(StateMachineInstanceType smi, object callback_data);

        private GameHashes id;

        private TargetParameter target;

        private Callback callback;

        private Func<StateMachineInstanceType, RMonoBehaviour> globalEventSystemCallback;

        public GameEvent(GameHashes id, Callback callback, TargetParameter target, Func<StateMachineInstanceType, RMonoBehaviour> global_event_system_callback)
            : base(id.ToString())
        {
            this.id = id;
            this.target = target;
            this.callback = callback;
            globalEventSystemCallback = global_event_system_callback;
        }

        public override Context Subscribe(Instance smi)
        {
            Context result = base.Subscribe(smi);
            StateMachineInstanceType cast_smi = (StateMachineInstanceType)smi;
            Action<object> handler = delegate (object d)
            {
                if (!Instance.error)
                {
                    callback(cast_smi, d);
                }
            };
            if (globalEventSystemCallback != null)
            {
                RMonoBehaviour RMonoBehaviour = globalEventSystemCallback(cast_smi);
                result.data = RMonoBehaviour.Subscribe((int)id, handler);
            }
            else
            {
                result.data = target.Get(cast_smi).Subscribe((int)id, handler);
            }
            return result;
        }

        public override void Unsubscribe(Instance smi, Context context)
        {
            StateMachineInstanceType val = (StateMachineInstanceType)smi;
            if (globalEventSystemCallback != null)
            {
                RMonoBehaviour RMonoBehaviour = globalEventSystemCallback(val);
                if (RMonoBehaviour != null)
                {
                    RMonoBehaviour.Unsubscribe(context.data);
                }
            }
            else
            {
                GameObject gameObject = target.Get(val);
                if (gameObject != null)
                {
                    gameObject.Unsubscribe(context.data);
                }
            }
        }
    }
}

public abstract class GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType> : GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, object> where StateMachineType : GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, object> where StateMachineInstanceType : GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, object>.GameInstance where MasterType : IStateMachineTarget
{
}
public abstract class GameStateMachine<StateMachineType, StateMachineInstanceType> : GameStateMachine<StateMachineType, StateMachineInstanceType, IStateMachineTarget, object> where StateMachineType : GameStateMachine<StateMachineType, StateMachineInstanceType, IStateMachineTarget, object> where StateMachineInstanceType : GameStateMachine<StateMachineType, StateMachineInstanceType, IStateMachineTarget, object>.GameInstance
{
}
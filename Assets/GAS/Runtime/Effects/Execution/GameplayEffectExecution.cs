using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GAS.Runtime
{
    [Serializable]
    public class GameplayEffectExecution
    {
        private const int LABEL_WIDTH = 70;

        [LabelText("参数修饰", SdfIconType.CpuFill)]
        [LabelWidth(LABEL_WIDTH)]
        [AssetSelector]
        [Tooltip("ExecutionCalculation，修改器，负责GAS中Attribute的数值计算逻辑。")]
        [PropertyOrder(1)]
        public ExecutionCalculation Execution;

        public void Execute(GameplayEffectSpec spec, out List<OutModifier> outModifiers)
        {
            outModifiers = new List<OutModifier>();
            Execution.Execute(spec, ref outModifiers);
        }
    }
}
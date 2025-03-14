using System.Collections.Generic;
using UnityEngine;

namespace GAS.Runtime
{
    [CreateAssetMenu(fileName = "DamageExecution", menuName = "GAS/Execution/DamageExecution")]
    public class DamageExecution : ExecutionCalculation
    {
        public override void Execute(GameplayEffectSpec spec, ref List<OutModifier> outModifiers)
        {
        }
    }
}
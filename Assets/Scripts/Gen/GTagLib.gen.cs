///////////////////////////////////
//// This is a generated file. ////
////     Do not modify it.     ////
///////////////////////////////////

using System.Collections.Generic;

namespace GAS.Runtime
{
    public static class GTagLib
    {
        public static GameplayTag Ability { get; } = new GameplayTag("Ability");
        public static GameplayTag Ability_Target { get; } = new GameplayTag("Ability.Target");
        public static GameplayTag Ability_Target_Self { get; } = new GameplayTag("Ability.Target.Self");

        public static Dictionary<string, GameplayTag> TagMap = new Dictionary<string, GameplayTag>
        {
            ["Ability"] = Ability,
            ["Ability.Target"] = Ability_Target,
            ["Ability.Target.Self"] = Ability_Target_Self,
        };
    }
}
///////////////////////////////////
//// This is a generated file. ////
////     Do not modify it.     ////
///////////////////////////////////

using System;
using System.Collections.Generic;

namespace GAS.Runtime
{
    public static class GAbilityLib
    {
        public struct AbilityInfo
        {
            public string Name;
            public string AssetPath;
            public Type AbilityClassType;
        }

        public static AbilityInfo NormalAttack = new AbilityInfo { Name = "NormalAttack", AssetPath = "Assets/GAS/Config/GameplayAbilityLib/NormalAttack.asset",AbilityClassType = typeof(NormalAttack) };


        public static Dictionary<string, AbilityInfo> AbilityMap = new Dictionary<string, AbilityInfo>
        {
            ["NormalAttack"] = NormalAttack,
        };
    }
}
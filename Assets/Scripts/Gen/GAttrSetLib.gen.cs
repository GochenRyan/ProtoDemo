///////////////////////////////////
//// This is a generated file. ////
////     Do not modify it.     ////
///////////////////////////////////

using System;
using System.Collections.Generic;

namespace GAS.Runtime
{
    public class AS_CombatEntity : AttributeSet
    {
        #region Attack

        /// <summary>
        /// 
        /// </summary>
        public AttributeBase Attack { get; } = new ("AS_CombatEntity", "Attack", 0f, CalculateMode.Stacking, (SupportedOperation)31, float.MinValue, float.MaxValue);

        public void InitAttack(float value)
        {
            Attack.SetBaseValue(value);
            Attack.SetCurrentValue(value);
        }

        public void SetCurrentAttack(float value)
        {
            Attack.SetCurrentValue(value);
        }

        public void SetBaseAttack(float value)
        {
            Attack.SetBaseValue(value);
        }

        public void SetMinAttack(float value)
        {
            Attack.SetMinValue(value);
        }

        public void SetMaxAttack(float value)
        {
            Attack.SetMaxValue(value);
        }

        public void SetMinMaxAttack(float min, float max)
        {
            Attack.SetMinMaxValue(min, max);
        }

        #endregion Attack

        #region HP

        /// <summary>
        /// 
        /// </summary>
        public AttributeBase HP { get; } = new ("AS_CombatEntity", "HP", 0f, CalculateMode.Stacking, (SupportedOperation)31, 0, float.MaxValue);

        public void InitHP(float value)
        {
            HP.SetBaseValue(value);
            HP.SetCurrentValue(value);
        }

        public void SetCurrentHP(float value)
        {
            HP.SetCurrentValue(value);
        }

        public void SetBaseHP(float value)
        {
            HP.SetBaseValue(value);
        }

        public void SetMinHP(float value)
        {
            HP.SetMinValue(value);
        }

        public void SetMaxHP(float value)
        {
            HP.SetMaxValue(value);
        }

        public void SetMinMaxHP(float min, float max)
        {
            HP.SetMinMaxValue(min, max);
        }

        #endregion HP

        #region Power

        /// <summary>
        /// 
        /// </summary>
        public AttributeBase Power { get; } = new ("AS_CombatEntity", "Power", 0f, CalculateMode.Stacking, (SupportedOperation)31, 0, float.MaxValue);

        public void InitPower(float value)
        {
            Power.SetBaseValue(value);
            Power.SetCurrentValue(value);
        }

        public void SetCurrentPower(float value)
        {
            Power.SetCurrentValue(value);
        }

        public void SetBasePower(float value)
        {
            Power.SetBaseValue(value);
        }

        public void SetMinPower(float value)
        {
            Power.SetMinValue(value);
        }

        public void SetMaxPower(float value)
        {
            Power.SetMaxValue(value);
        }

        public void SetMinMaxPower(float min, float max)
        {
            Power.SetMinMaxValue(min, max);
        }

        #endregion Power

        public override AttributeBase this[string key]
        {
            get
            {
                switch (key)
                {
                    case "HP":
                        return HP;
                    case "Power":
                        return Power;
                    case "Attack":
                        return Attack;
                }

                return null;
            }
        }

        public override string[] AttributeNames { get; } =
        {
            "HP",
            "Power",
            "Attack",
        };

        public override void SetOwner(AbilitySystemComponent owner)
        {
            _owner = owner;
            HP.SetOwner(owner);
            Power.SetOwner(owner);
            Attack.SetOwner(owner);
        }

        public static class Lookup
        {
            public const string HP = "AS_CombatEntity.HP";
            public const string Power = "AS_CombatEntity.Power";
            public const string Attack = "AS_CombatEntity.Attack";
        }
    }

    public static class GAttrSetLib
    {
        public static readonly Dictionary<string, Type> AttrSetTypeDict = new Dictionary<string, Type>()
        {
            { "CombatEntity", typeof(AS_CombatEntity) },
        };

        public static readonly Dictionary<Type, string> TypeToName = new Dictionary<Type, string>
        {
            { typeof(AS_CombatEntity), nameof(AS_CombatEntity) },
        };

        public static List<string> AttributeFullNames = new List<string>()
        {
            "AS_CombatEntity.HP",
            "AS_CombatEntity.Power",
            "AS_CombatEntity.Attack",
        };
    }
}
using Sirenix.OdinInspector;
using System.Linq;
using GAS.General;
using System;
using System.Collections.Generic;
using UnityEngine;
using static GAS.Runtime.GameplayEffectExecution;
using Codice.Client.BaseCommands;

namespace GAS.Runtime
{
    public class OutModifier
    {
        public AttributeBase Attribute;
        public GEOperation Operation;
        public float Value;
    }

    public abstract class ExecutionCalculation : ScriptableObject
    {
        protected const int WIDTH_LABEL = 70;

        [TitleGroup("Base")]
        [HorizontalGroup("Base/H1", width: 1 - 0.618f)]
        [TabGroup("Base/H1/V1", "Summary", SdfIconType.InfoSquareFill, TextColor = "#0BFFC5", Order = 1)]
        [HideLabel]
        [MultiLineProperty(10)]
        public string Description;

#if UNITY_EDITOR
        [TabGroup("Base/H1/V2", "General", SdfIconType.AwardFill, TextColor = "#FF7F00", Order = 2)]
        [TabGroup("Base/H1/V2", "Detail", SdfIconType.TicketDetailedFill, TextColor = "#BC2FDE")]
        [LabelText("类型名称", SdfIconType.FileCodeFill)]
        [LabelWidth(WIDTH_LABEL)]
        [ShowInInspector]
        [PropertyOrder(-1)]
        public string TypeName => GetType().Name;

        [TabGroup("Base/H1/V2", "Detail")]
        [LabelText("类型全名", SdfIconType.FileCodeFill)]
        [LabelWidth(WIDTH_LABEL)]
        [ShowInInspector]
        [PropertyOrder(-1)]
        public string TypeFullName => GetType().FullName;

        [TabGroup("Base/H1/V2", "Detail")]
        [ListDrawerSettings(ShowFoldout = true, ShowItemCount = false, ShowPaging = false)]
        [ShowInInspector]
        [LabelText("继承关系")]
        [LabelWidth(WIDTH_LABEL)]
        [PropertyOrder(-1)]
        public string[] InheritanceChain => GetType().GetInheritanceChain().Reverse().ToArray();
#endif

        public enum AttributeFrom
        {
            [LabelText("来源(Source)", SdfIconType.Magic)]
            Source,

            [LabelText("目标(Target)", SdfIconType.Person)]
            Target
        }

        public enum GEAttributeCaptureType
        {
            [LabelText("快照(SnapShot)", SdfIconType.Camera)]
            SnapShot,

            [LabelText("实时(Track)", SdfIconType.Speedometer2)]
            Track
        }

        [Serializable]
        public class ExecutionParameter
        {
            public AttributeFrom AttributeFrom;
            public GEAttributeCaptureType AttributeCaptureType;

            [LabelText("属性名称", SdfIconType.Fingerprint)]
            [LabelWidth(WIDTH_LABEL)]
            [OnValueChanged("OnAttributeChanged")]
            [ValueDropdown("@ValueDropdownHelper.AttributeChoices", IsUniqueList = true)]
            [InfoBox("未选择属性", InfoMessageType.Error, VisibleIf = "@string.IsNullOrWhiteSpace($value)")]
            [SuffixLabel("@ReflectionHelper.GetAttribute($value)?.CalculateMode")]
            [PropertyOrder(1)]
            public string AttributeName;

            [HideInInspector]
            public string AttributeSetName;

            [HideInInspector]
            public string AttributeShortName;

            public ExecutionParameter(AttributeFrom attributeFrom, GEAttributeCaptureType attributeCaptureType, string attributeName)
            {
                AttributeFrom = attributeFrom;
                AttributeCaptureType = attributeCaptureType;
                AttributeName = attributeName;

                var splits = attributeName.Split('.');
                AttributeSetName = splits[0];
                AttributeShortName = splits[1];
            }

            void OnAttributeChanged()
            {
                var split = AttributeName.Split('.');
                AttributeSetName = split[0];
                AttributeShortName = split[1];
            }
        }

        protected float GetParamValue(GameplayEffectSpec spec, ExecutionParameter param)
        {
            float attribute;
            if (param.AttributeFrom == AttributeFrom.Source)
            {
                if (param.AttributeCaptureType == GEAttributeCaptureType.SnapShot)
                {
                    var snapShot = spec.SnapshotSourceAttributes;
                    attribute = snapShot[param.AttributeName];
                }
                else
                {
                    attribute = (float)spec.Source.GetAttributeCurrentValue(param.AttributeSetName, param.AttributeShortName);
                }
            }
            else
            {
                if (param.AttributeCaptureType == GEAttributeCaptureType.SnapShot)
                {
                    var snapShot = spec.SnapshotTargetAttributes;
                    attribute = snapShot[param.AttributeName];
                }
                else
                {
                    attribute = (float)spec.Owner.GetAttributeCurrentValue(param.AttributeSetName, param.AttributeShortName);
                }
            }

            return attribute;
        }


        [ListDrawerSettings(ShowFoldout = true, DraggableItems = true)]
        [PropertyOrder(1)]
        public ExecutionParameter[] ExecutionParameters;

        public abstract void Execute(GameplayEffectSpec spec, ref List<OutModifier> outModifiers);
    }
}

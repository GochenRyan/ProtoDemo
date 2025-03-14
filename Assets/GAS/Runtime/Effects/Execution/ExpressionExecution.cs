using B83.LogicExpressionParser;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GAS.Runtime
{
    [CreateAssetMenu(fileName = "ExpressionExecution", menuName = "GAS/Execution/ExpressionExecution")]
    public class ExpressionExecution : ExecutionCalculation
    {
        [Serializable]
        public class OutParameter
        {
            public AttributeFrom AttributeFrom;

            [LabelText("修改属性", SdfIconType.Fingerprint)]
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

            public GEOperation Operation;

            public OutParameter(AttributeFrom attributeFrom, string attributeName, GEOperation operation)
            {
                AttributeFrom = attributeFrom;
                AttributeName = attributeName;
                var splits = attributeName.Split('.');
                AttributeSetName = splits[0];
                AttributeShortName = splits[1];
                Operation = operation;
            }

            void OnAttributeChanged()
            {
                var split = AttributeName.Split('.');
                AttributeSetName = split[0];
                AttributeShortName = split[1];
            }
        }

        public ExpressionExecution() 
        {
            ParsingContext parsingContext = new ParsingContext();
            parsingContext.AddFunction("In", (p) =>
            {
                if (p.inputs.Count != 1)
                {
                    throw new Exception("The number of parameters is not 1");
                }
                int index = (int)p[0];
                return In(index);
            });
            parsingContext.AddFunction("Out", (p) =>
            {
                if (p.inputs.Count != 1)
                {
                    throw new Exception("The number of parameters is not 1");
                }
                int index = (int)p[0];
                return Out(index);
            });
            _parser = new Parser(parsingContext);
        }

        public override void Execute(GameplayEffectSpec spec, ref List<OutModifier> outModifiers)
        {
            foreach(var param in ExecutionParameters)
            {
                float inValue = GetParamValue(spec, param);
                _inValues.Add(inValue);
            }

            foreach (var expression in Expressions)
            {
                var res = _parser.ParseNumber(expression);
                float outValue = (float)res.GetNumber();
                _outValues.Add(outValue);
            }

            for(int i = 0; i < OutParameters.Count(); ++i)
            {
                var outParameter = OutParameters[i];
                var outModifier = new OutModifier();
                outModifier.Attribute = GetAttribute(spec, outParameter);
                outModifier.Value = _outValues[i];
                outModifier.Operation = outParameter.Operation;
                outModifiers.Add(outModifier);
            }
        }

        private AttributeBase GetAttribute(GameplayEffectSpec spec, OutParameter outParameter) 
        { 
            if (outParameter.AttributeFrom == AttributeFrom.Source)
            {
                return spec.Source.AttributeSetContainer.Sets[outParameter.AttributeSetName][outParameter.AttributeShortName];
            }
            else
            {
                return spec.Owner.AttributeSetContainer.Sets[outParameter.AttributeSetName][outParameter.AttributeShortName];
            }
        }

        private float In(int index)
        {
            if (_inValues.Count <= index)
            {
                throw new InvalidOperationException(
                        $"The length of the input list is {_inValues.Count}" +
                        $"But index is {index}");
            }

            return _inValues[index];
        }

        private float Out(int index)
        {
            if (_outValues.Count <= index)
            {
                throw new InvalidOperationException(
                        $"The length of the out list is {_outValues.Count}" +
                        $"But index is {index}");
            }

            return _outValues[index];
        }

        private List<float> _inValues;
        private List<float> _outValues;
        private Parser _parser;

        [LabelText("表达式列表", SdfIconType.Pencil)]
        [ListDrawerSettings(ShowFoldout = true, DraggableItems = true)]
        [PropertyOrder(2)]
        [Tooltip("会按照顺序执行\nIn(n):获取第n个输入的Attribute的值\nOut(n):获取第n个输出的Attribute的值")]
        public string[] Expressions;

        [PropertyOrder(1)]
        public OutParameter[] OutParameters;
    }
}
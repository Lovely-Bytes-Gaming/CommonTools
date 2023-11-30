using System;
using LovelyBytes.AssetVariables;
using TNRD;
using UnityEngine;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    public class ComparisonCondition<TValue> : TransitionCondition
        where TValue : IComparable<TValue>
    {
        [SerializeField]
        private SerializableInterface<IReadOnlyView<TValue>> _value;

        [SerializeField] 
        private TValue _referenceValue;

        [SerializeField] 
        private ComparisonType _comparisonType;
        
        public override bool QueryCondition(float deltaTime)
        {
            return Compare(_value.Value.Value);
        }

        private bool Compare(TValue value)
        {
            int cmp = value.CompareTo(_referenceValue);
            
            return _comparisonType switch
            {
                ComparisonType.Equal => cmp == 0,
                ComparisonType.Less => cmp < 0,
                _ => cmp > 0,
            };
        }
    }
}
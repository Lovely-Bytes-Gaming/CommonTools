using System;
using LovelyBytes.AssetVariables;
using TNRD;
using UnityEngine;
using UnityEngine.Serialization;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    /// <summary>
    /// Compares the value of the given object with its Reference Value.
    /// </summary>
    public class ComparisonCondition<TValue> : FsmCondition
        where TValue : IComparable<TValue>
    {
        [SerializeField]
        private SerializableInterface<IReadOnlyView<TValue>> _value;

        [FormerlySerializedAs("_referenceValue")] 
        public TValue ReferenceValue;

        [SerializeField] 
        private ComparisonType _comparisonType;
        
        public override bool QueryCondition(float deltaTime)
        {
            return Compare(_value.Value.Value);
        }

        private bool Compare(TValue value)
        {
            int cmp = value.CompareTo(ReferenceValue);
            
            return _comparisonType switch
            {
                ComparisonType.Equal => cmp == 0,
                ComparisonType.Less => cmp < 0,
                _ => cmp > 0,
            };
        }
    }
}
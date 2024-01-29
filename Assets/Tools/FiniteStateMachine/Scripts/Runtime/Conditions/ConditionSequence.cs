using System.Collections.Generic;
using UnityEngine;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    /// <summary>
    /// Evaluates all its conditions in order up to the first condition that is not satisfied.
    /// Returns true when all conditions are satisfied.
    /// </summary>
    [CreateAssetMenu(menuName = "LovelyBytes/CommonTools/FiniteStateMachine/ConditionSequence")]
    public class ConditionSequence : TransitionCondition
    {
        [SerializeField] 
        private List<TransitionCondition> _conditions;

        private int _index;
        
        public override bool QueryCondition(float deltaTime)
        {
            for (; _index < _conditions.Count; ++_index)
            {
                if (!_conditions[_index].QueryCondition(deltaTime))
                    return false;
            }

            return true;
        }

        public override void ResetCondition()
        {
            _index = 0;
            
            foreach (TransitionCondition condition in _conditions)
            {
                if(condition)
                    condition.ResetCondition();
            }
        }
    }
}

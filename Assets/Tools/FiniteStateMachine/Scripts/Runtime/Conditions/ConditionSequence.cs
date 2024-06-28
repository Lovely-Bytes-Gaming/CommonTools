using System.Collections.Generic;
using UnityEngine;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    /// <summary>
    /// Evaluates all its conditions in order up to the first condition that is not satisfied.
    /// Returns true when all conditions are satisfied.
    /// </summary>
    [CreateAssetMenu(menuName = "LovelyBytes/CommonTools/FiniteStateMachine/ConditionSequence")]
    public class ConditionSequence : FsmCondition
    {
        [SerializeField] 
        private List<FsmCondition> _conditions;

        private int _index;

        public override void UpdateCondition(float deltaTime)
        {
            _conditions[_index].UpdateCondition(deltaTime);
        }
        
        protected override bool GetIsSatisfied()
        {
            while (_index < _conditions.Count &&
                _conditions[_index].IsSatisfied)
            {
                ++_index;
            }
            return _index >= _conditions.Count;
        }

        public override void ResetCondition()
        {
            _index = 0;
            
            foreach (FsmCondition condition in _conditions)
            {
                if(condition)
                    condition.ResetCondition();
            }
        }
    }
}

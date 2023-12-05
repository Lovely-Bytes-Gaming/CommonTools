using System.Collections.Generic;
using UnityEngine;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    [CreateAssetMenu(menuName = "LovelyBytes/CommonTools/FiniteStateMachine/ConditionSequence")]
    public class ConditionSequence : TransitionCondition
    {
        [SerializeField] 
        private List<TransitionCondition> _conditions;
        
        public override bool QueryCondition(float deltaTime)
        {
            foreach (TransitionCondition condition in _conditions)
            {
                if (!condition.QueryCondition(deltaTime))
                    return false;
            }
            return true;
        }

        public override void ResetCondition()
        {
            base.ResetCondition();
            
            foreach (TransitionCondition condition in _conditions)
            {
                if(condition)
                    condition.ResetCondition();
            }
        }
    }
}

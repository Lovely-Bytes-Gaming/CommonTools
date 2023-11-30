using System.Collections.Generic;
using UnityEngine;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    [CreateAssetMenu(menuName = "LovelyBytes/CommonTools/FiniteStateMachine/CompoundCondition")]
    public class CompoundCondition : TransitionCondition
    {
        [SerializeField] private List<TransitionCondition> _conditions = new();

        public override bool QueryCondition(float deltaTime)
        {
            bool result = true;

            foreach (TransitionCondition condition in _conditions)
            {
                if(condition)
                    result &= condition.QueryCondition(deltaTime);
            }
            return result;
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
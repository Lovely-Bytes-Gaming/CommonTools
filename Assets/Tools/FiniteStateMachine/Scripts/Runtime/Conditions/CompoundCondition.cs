using System.Collections.Generic;
using UnityEngine;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    /// <summary>
    /// Evaluates all its assigned conditions during each query.
    /// Returns true when all conditions are satisfied.
    /// </summary>
    [CreateAssetMenu(menuName = "LovelyBytes/CommonTools/FiniteStateMachine/CompoundCondition")]
    public class CompoundCondition : FsmCondition
    {
        [SerializeField] private List<FsmCondition> _conditions = new();

        public override bool QueryCondition(float deltaTime)
        {
            bool result = true;

            foreach (FsmCondition condition in _conditions)
            {
                if(condition)
                    result &= condition.QueryCondition(deltaTime);
            }
            return result;
        }

        public override void ResetCondition()
        {
            base.ResetCondition();

            foreach (FsmCondition condition in _conditions)
            {
                if(condition)
                    condition.ResetCondition();
            }
        }
    }
}
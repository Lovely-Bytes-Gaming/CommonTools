using UnityEngine;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    [CreateAssetMenu(menuName = "LovelyBytes/CommonTools/FiniteStateMachine/InverseCondition")]
    public class InverseCondition : TransitionCondition
    {
        public TransitionCondition Condition;        
        
        public override bool QueryCondition(float deltaTime)
        {
            return !Condition.QueryCondition(deltaTime);
        }

        public override void ResetCondition()
        {
            Condition.ResetCondition();
        }
    }
}
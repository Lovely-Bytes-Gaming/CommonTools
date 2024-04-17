using UnityEngine;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    /// <summary>
    /// Returns true when its nested condition is false.
    /// </summary>
    [CreateAssetMenu(menuName = "LovelyBytes/CommonTools/FiniteStateMachine/InverseCondition")]
    public class InverseCondition : FsmCondition
    {
        public FsmCondition Condition;        
        
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
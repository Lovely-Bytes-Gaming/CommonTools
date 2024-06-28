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

        public override void UpdateCondition(float deltaTime)
        {
            Condition.UpdateCondition(deltaTime);
        }
        
        protected override bool GetIsSatisfied()
        {
            return !Condition.IsSatisfied;
        }

        public override void ResetCondition()
        {
            Condition.ResetCondition();
        }
    }
}
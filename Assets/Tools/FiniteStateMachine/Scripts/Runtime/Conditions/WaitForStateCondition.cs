using UnityEngine;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    [CreateAssetMenu(menuName = "LovelyBytes/CommonTools/FiniteStateMachine/WaitForStateCondition")]
    public class WaitForStateCondition : TransitionCondition
    {
        [SerializeField] 
        private FsmState _state;

        public override bool QueryCondition(float deltaTime)
        {
            return _state.IsActive;
        }
    }
}
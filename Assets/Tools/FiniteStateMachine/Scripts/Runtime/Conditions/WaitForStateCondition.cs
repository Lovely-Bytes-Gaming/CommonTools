using UnityEngine;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    /// <summary>
    /// Returns true as soon as the referenced state is active.
    /// </summary>
    [CreateAssetMenu(menuName = "LovelyBytes/CommonTools/FiniteStateMachine/WaitForStateCondition")]
    public class WaitForStateCondition : FsmCondition
    {
        [SerializeField] 
        private FsmState _state;

        protected override bool GetIsSatisfied()
        {
            return _state.IsActive;
        }
    }
}
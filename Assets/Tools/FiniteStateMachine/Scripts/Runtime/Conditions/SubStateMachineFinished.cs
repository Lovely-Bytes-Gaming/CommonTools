using UnityEngine;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    [CreateAssetMenu(menuName = "LovelyBytes/CommonTools/FiniteStateMachine/SubStateMachineFinished")]
    public class SubStateMachineFinished : FsmCondition
    {
        [SerializeField] private FsmState _state;
        
        protected override bool GetIsSatisfied()
        {
            FsmStateMachine subFsm = _state.SubStateMachine;
            
            if (!subFsm)
                return true;

            return subFsm.CurrentState == subFsm.ExitState;
        }
    }
}